$ErrorActionPreference = 'Stop'

# --- stderr 를 UTF-8 로 직접 쓴다 (PS 5.1 기본 인코딩이면 한글이 깨진다) ---
function Write-GuardError([string]$text) {
    $utf8 = New-Object System.Text.UTF8Encoding($false)
    $w = New-Object System.IO.StreamWriter([Console]::OpenStandardError(), $utf8)
    $w.AutoFlush = $true
    $w.WriteLine($text)
    $w.Flush()
}

# --- stdin JSON 읽기 ---
$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }

try {
    $payload = $raw | ConvertFrom-Json
} catch {
    # 입력을 못 읽으면 작업을 막지 않는다 (fail-open, 경고만)
    Write-GuardError("guard-bash: could not parse hook input JSON")
    exit 1
}

$tool = [string]$payload.tool_name
if (@('Bash', 'PowerShell') -notcontains $tool) { exit 0 }

$cmd = [string]$payload.tool_input.command
if ([string]::IsNullOrWhiteSpace($cmd)) { exit 0 }

# --- 히어독 본문은 검사에서 제외한다 (계획서에 코드 예시를 적는 것을 막지 않기 위해) ---
$scan = $cmd
$hdOpen = [regex]::Match($scan, '<<-?\s*[''"]?(?<tag>[A-Za-z_][A-Za-z0-9_]*)[''"]?')
if ($hdOpen.Success) {
    $tag   = $hdOpen.Groups['tag'].Value
    $tagRe = '<<-?\s*[''"]?' + [regex]::Escape($tag) + '[''"]?'
    $kept  = New-Object System.Collections.Generic.List[string]
    $skip  = $false
    foreach ($ln in ($scan -split "`n")) {
        if ($skip) {
            if ($ln.Trim() -eq $tag) { $skip = $false }
            continue
        }
        $kept.Add($ln)
        if ($ln -match $tagRe) { $skip = $true }
    }
    $scan = ($kept -join "`n")
}

# --- 프로젝트 루트 확정 ---
$projRoot = $env:CLAUDE_PROJECT_DIR
if ([string]::IsNullOrWhiteSpace($projRoot)) { $projRoot = [string]$payload.cwd }
if ([string]::IsNullOrWhiteSpace($projRoot)) { $projRoot = (Get-Location).Path }
$projRoot = $projRoot.Replace([char]92, [char]47).TrimEnd([char]47)

# --- 경로 판정 헬퍼 ---
function ConvertTo-NormalPath([string]$p) {
    if ($null -eq $p) { return '' }
    $p = $p.Trim()
    $p = $p.Trim([char[]]@(34, 39))          # 앞뒤 따옴표 제거
    $p = $p.Replace([char]92, [char]47)      # backslash -> slash
    if ($p -match '^/([A-Za-z])/') {         # git-bash 스타일 /c/... -> c:/...
        $p = $p.Substring(1, 1) + ':' + $p.Substring(2)
    }
    return $p
}

function Test-NullSink([string]$p) {
    $l = $p.ToLowerInvariant()
    if ($l.StartsWith('&')) { return $true }              # 2>&1
    return (@('/dev/null', 'nul', 'null', '$null', 'con', 'out-null') -contains $l)
}

function Test-InsideProject([string]$p) {
    $abs = $p
    if ($abs -notmatch '^[A-Za-z]:/' -and -not $abs.StartsWith('/') -and -not $abs.StartsWith('~')) {
        $abs = $projRoot + '/' + ($abs -replace '^\./', '')
    }
    return $abs.ToLowerInvariant().StartsWith($projRoot.ToLowerInvariant() + '/')
}

# --- 확장자 스캔 결과가 '진짜 파일 경로처럼 보이는가' 판정 ---
$script:KnownFileExt = @(
    '.cs','.js','.mjs','.cjs','.ts','.tsx','.jsx','.py','.ps1','.psm1','.psd1',
    '.sh','.bash','.bat','.cmd','.json','.xml','.yaml','.yml','.toml','.ini',
    '.cfg','.config','.csproj','.sln','.slnx','.asmdef','.asmref','.shader',
    '.cginc','.hlsl','.compute','.unity','.prefab','.asset','.mat','.anim',
    '.controller','.meta','.txt','.csv','.tsv','.log','.html','.htm','.css',
    '.scss','.sql','.java','.kt','.c','.h','.hpp','.cpp','.cc','.go','.rs',
    '.rb','.php','.lua','.png','.jpg','.jpeg','.gif','.tga','.psd','.wav',
    '.mp3','.ogg','.fbx','.obj','.blend','.dll','.exe','.gitignore','.editorconfig','.gitattributes'
)

function Test-LooksLikePath([string]$token) {
    if ([string]::IsNullOrWhiteSpace($token)) { return $false }
    $t = $token.Trim().Trim([char[]]@(34, 39)).Replace([char]92, [char]47)
    if ($t -match '^\d+(\.\d+)*\.?\.?\d*$') { return $false }   # 0..2, 1.2.3 같은 숫자/범위
    # 확장자만 있는 조각은 경로가 아니다 (f-string 등에서 잘려 나온 '.json' 같은 것).
    # 이름이 곧 확장자인 실제 파일(.gitignore 등)은 AllowedConfigFiles 에서 따로 허용한다.
    if ($t -match '^.[A-Za-z0-9]+$') { return $false }
    if ($t.Contains('/')) { return $true }                       # 슬래시가 있으면 경로로 본다
    $ext = ''
    try { $ext = [System.IO.Path]::GetExtension($t).ToLowerInvariant() } catch { return $false }
    if (-not ($script:KnownFileExt -contains $ext)) { return $false }
    # 슬래시가 없으면 현재 폴더의 파일을 뜻한다. 실제로 존재할 때만 경로로 본다 —
    # console.log, System.IO.File 처럼 점 찍힌 식별자를 파일로 오인하지 않기 위함이다.
    # 새 파일 생성은 리다이렉션 추출기가 따로 잡으므로 이 완화로 뚫리지 않는다.
    try { return (Test-Path -LiteralPath (Join-Path $projRoot $t)) } catch { return $false }
}


# --- 저장소 살림용 설정 파일. 게임 소스가 아니므로 허용한다 ---
$script:AllowedConfigFiles = @('.gitignore', '.gitattributes', '.editorconfig', '.graphifyignore')

function Test-BashTargetAllowed([string]$rawPath) {
    $p = ConvertTo-NormalPath $rawPath
    if ([string]::IsNullOrWhiteSpace($p)) { return $true }
    if (Test-NullSink $p) { return $true }
    if ($p -match '^[A-Za-z][A-Za-z0-9+.\-]*://') { return $true }   # URL
    if (-not (Test-InsideProject $p)) { return $true }               # 프로젝트 밖은 관여 안 함

    $ext = ''
    try { $ext = [System.IO.Path]::GetExtension($p).ToLowerInvariant() } catch { $ext = '' }
    # --- 게임 소스 트리에는 키트 예외를 적용하지 않는다 ---
    # Unity 스크립트 폴더 이름이 Scripts 라서 scripts/ 예외에 걸리던 구멍을 막는다.
    if ($p -match '(^|/)(assets|projectsettings|packages)/') {
        if ($ext -eq '.md' -or $ext -eq '.markdown') { return $true }
        return $false
    }
    if ($ext -eq '.md' -or $ext -eq '.markdown') { return $true }
    if ($p -match '(^|/)docs/') { return $true }
    if ($p -match '(^|/)scripts/')  { return $true }
    if ($p -match '(^|/)\.claude/') { return $true }
    # graphify 생성물 폴더. gitignore 대상이고 도구가 스스로 관리한다
    if ($p -match '(^|/)graphify-out/') { return $true }
    $leaf = [System.IO.Path]::GetFileName($p).ToLowerInvariant()
    if ($script:AllowedConfigFiles -contains $leaf) { return $true }
    return $false
}

$targets = New-Object System.Collections.Generic.List[string]
$reasons = New-Object System.Collections.Generic.List[string]

# --- 1. 무조건 차단: 패치 적용 계열 ---
$hardBlock = @(
    @{ p = '\bgit\s+apply\b';        why = 'git apply (패치 적용)' },
    @{ p = '\bgit\s+am\b';           why = 'git am (패치 적용)' },
    @{ p = '(^|[\s|;&])patch\s+[-<]'; why = 'patch (패치 적용)' }
)
foreach ($h in $hardBlock) {
    if ($scan -match $h.p) { $reasons.Add($h.why) }
}

# --- 2. 리다이렉션 대상 ( > file, >> file, 2> file ) ---
$redir = [regex]::Matches($scan, '(?<![-=!<>])>>?\s*(?<t>"[^"]*"|''[^'']*''|[^\s|&;<>()]+)')
foreach ($m in $redir) {
    $rt = $m.Groups['t'].Value
    # awk/셸의 (getline ...) 숫자 비교를 리다이렉션 대상으로 오인하지 않는다
    $asNum = 0
    if ([int]::TryParse($rt, [ref]$asNum)) { continue }
    $targets.Add($rt)
}

# --- 3. tee 대상 ---
$teeM = [regex]::Matches($scan, '\btee\b(?:\s+-a)?\s+(?<t>"[^"]*"|''[^'']*''|[^\s|&;<>]+)')
foreach ($m in $teeM) { $targets.Add($m.Groups['t'].Value) }

# --- 4. 경로를 인자로 받는 쓰기 명령이 있으면, 명령문 안의 파일 경로를 전부 검사 ---
$pathWriteIndicators = @(
    '\bsed\b[^|;]*\s-i',
    '\b(cp|mv|rm|touch|truncate|unlink|shred)\b',
    '\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item|Rename-Item|Export-Csv|Export-Clixml|Set-ItemProperty)\b',
    '\[System\.IO\.File\]::(WriteAll|AppendAll|Create|Copy|Move|Delete|Replace)',
    '\[System\.IO\.Directory\]::(Create|Delete|Move)',
    '\bpython[0-9.]*\s+-c\b',
    '\bnode\s+-e\b',
    '\bperl\s+-[a-zA-Z]*i',
    '\bdd\b\s+if='
)
$hasPathWrite = $false
foreach ($ind in $pathWriteIndicators) {
    if ($scan -match $ind) { $hasPathWrite = $true; break }
}

if ($hasPathWrite) {
    $extM = [regex]::Matches($scan, '(?<![\w.])(?<t>(?:[A-Za-z]:[\\/])?[\w.\-/\\~]*\.[A-Za-z0-9]{1,10})(?![\w.])')
    foreach ($m in $extM) {
        $tok = $m.Groups['t'].Value
        if (Test-LooksLikePath $tok) { $targets.Add($tok) }
    }
}

# --- 판정 ---
$blocked = @()
foreach ($t in $targets) {
    if (-not (Test-BashTargetAllowed $t)) { $blocked += (ConvertTo-NormalPath $t) }
}
$blocked = @($blocked | Select-Object -Unique)

if ($blocked.Count -eq 0 -and $reasons.Count -eq 0) { exit 0 }

$detail = ''
if ($blocked.Count -gt 0) {
    $detail += "차단된 대상:`n" + (($blocked | ForEach-Object { "  - $_" }) -join "`n") + "`n"
}
if ($reasons.Count -gt 0) {
    $detail += "차단된 명령 유형:`n" + (($reasons | ForEach-Object { "  - $_" }) -join "`n") + "`n"
}

$cmdShown = $cmd
if ($cmdShown.Length -gt 400) { $cmdShown = $cmdShown.Substring(0, 400) + ' ...(생략)' }

$msg = @"
[PLANNER MODE] 셸을 통한 소스 수정이 차단되었습니다.

실행하려던 명령:
  $cmdShown

$detail
이 워크스페이스에서 당신의 역할은 아키텍트/기획자입니다 (CLAUDE.md 참고).
편집 도구(Edit/Write)를 셸로 우회하는 것도 같은 이유로 금지입니다.

대신 다음을 하십시오:

  1. 이 변경을 TaskPlan/TASK_PLAN.md의 "구현 스텝"에 파일 경로까지 특정해서 적으십시오.
  2. 계획이 끝나면 PLAN_COMPLETED 를 출력하십시오.
  3. 구현은 Codex가 합니다:
     powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/handoff-to-codex.ps1

허용되는 것: 읽기·검색·분석 명령 전부 (cat, grep, git log, git diff, git status ...),
프로젝트 밖 경로 쓰기(임시 폴더 등), .md 파일과 docs/ 아래 쓰기.
"@
Write-GuardError($msg)
exit 2
