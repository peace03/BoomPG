$ErrorActionPreference = 'Stop'

# --- 저장소 경로와 검증 케이스 ---
$projRoot = Split-Path -Parent $PSScriptRoot
$cases = @(
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'Assets/Player.cs' }; Expected = 2 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'TASK_PLAN.md' }; Expected = 0 },
    @{ Guard = 'guard-planner'; Tool = 'Bash'; Input = @{ command = 'ls' }; Expected = 0 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'scripts/guard-bash.ps1' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'PowerShell'; Input = @{ command = '[System.IO.File]::ReadAllBytes($f)[0..2]' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'cat Assets/Player.cs > /dev/null' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'PowerShell'; Input = @{ command = 'Set-Content -LiteralPath scripts/guard-bash.ps1 -Value $c' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'sed -i s/a/b/ Assets/Player.cs' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = "cat > TASK_PLAN.md <<'EOF'`nOut-File Assets/Player.cs`nEOF" }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'echo x > Assets/Player.cs' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'git apply fix.patch' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'git diff --stat' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'cp README.md Assets/Player.cs' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'rm C:/Users/mbc/AppData/Local/Temp/x.cs' }; Expected = 0 },

    # --- 저장소 살림용 설정 파일은 허용한다 (2026-09-17) ---
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = '.gitignore' }; Expected = 0 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = '.editorconfig' }; Expected = 0 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = '.gitattributes' }; Expected = 0 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'echo x >> .gitignore' }; Expected = 0 },

    # --- 예외가 넓어지지 않았는지 확인한다 ---
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = '.env' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'echo SECRET > .env' }; Expected = 2 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'Assets/_Project/Scripts/Foo.gitignore' }; Expected = 2 },

    # --- Unity 스크립트 폴더가 scripts/ 예외에 걸리던 구멍 (2026-09-17 수정) ---
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'Assets/_Project/Scripts/Gameplay/Player/PlayerMotor.cs' }; Expected = 2 },
    @{ Guard = 'guard-bash'; Tool = 'Bash'; Input = @{ command = 'echo x > Assets/_Project/Scripts/Core/Logging/GameLog.cs' }; Expected = 2 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'ProjectSettings/TagManager.asset' }; Expected = 2 },
    @{ Guard = 'guard-planner'; Tool = 'Write'; Input = @{ file_path = 'Assets/_Project/README.md' }; Expected = 0 }
)

# --- 명령을 실행하지 않고 가드에 JSON 입력만 전달한다 ---
$failed = 0
$caseNumber = 0
$previousProjectDir = $env:CLAUDE_PROJECT_DIR
$previousInputEncoding = [Console]::InputEncoding
try {
    [Console]::InputEncoding = New-Object System.Text.UTF8Encoding($false)
    foreach ($case in $cases) {
        $caseNumber++
        $env:CLAUDE_PROJECT_DIR = $projRoot
        $process = New-Object System.Diagnostics.Process
        try {
            $guardPath = Join-Path $PSScriptRoot ($case.Guard + '.ps1')
            $process.StartInfo.FileName = 'powershell.exe'
            $process.StartInfo.Arguments = '-NoProfile -ExecutionPolicy Bypass -File "' + $guardPath + '"'
            $process.StartInfo.WorkingDirectory = $projRoot
            $process.StartInfo.UseShellExecute = $false
            $process.StartInfo.CreateNoWindow = $true
            $process.StartInfo.RedirectStandardInput = $true
            $process.StartInfo.RedirectStandardOutput = $true
            $process.StartInfo.RedirectStandardError = $true
            $process.StartInfo.StandardErrorEncoding = New-Object System.Text.UTF8Encoding($false)
            [void]$process.Start()
            $stdout = $process.StandardOutput.ReadToEndAsync()
            $stderr = $process.StandardError.ReadToEndAsync()
            $json = @{ tool_name = $case.Tool; tool_input = $case.Input } | ConvertTo-Json -Compress -Depth 5
            # PS 5.1의 기본 stdin writer가 붙이는 BOM을 제외한다.
            $inputWriter = New-Object System.IO.StreamWriter($process.StandardInput.BaseStream, (New-Object System.Text.UTF8Encoding($false)))
            $inputWriter.WriteLine($json)
            $inputWriter.Close()
            $process.WaitForExit()
            $outputText = $stdout.GetAwaiter().GetResult()
            $errorText = $stderr.GetAwaiter().GetResult()
            if ($process.ExitCode -eq $case.Expected) {
                Write-Output ("PASS {0}: {1} (exit {2})" -f $caseNumber, $case.Guard, $process.ExitCode)
            } else {
                $failed++
                Write-Output ("FAIL {0}: {1} (expected {2}, actual {3})" -f $caseNumber, $case.Guard, $case.Expected, $process.ExitCode)
                Write-Output $outputText
                Write-Output $errorText
            }
        } catch {
            $failed++
            Write-Output ("FAIL {0}: {1}" -f $caseNumber, $_.Exception.Message)
        } finally {
            $process.Dispose()
        }
    }
} finally {
    $env:CLAUDE_PROJECT_DIR = $previousProjectDir
    [Console]::InputEncoding = $previousInputEncoding
}

Write-Output ("FAILED: {0}" -f $failed)
if ($failed -gt 0) { exit 1 }
exit 0
