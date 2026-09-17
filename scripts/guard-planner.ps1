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
    Write-GuardError("guard-planner: could not parse hook input JSON")
    exit 1
}

$tool = [string]$payload.tool_name

# 편집 계열 도구만 검사한다
$editTools = @('Edit', 'Write', 'MultiEdit', 'NotebookEdit')
if ($editTools -notcontains $tool) { exit 0 }

# --- 대상 경로 수집 ---
$paths = New-Object System.Collections.Generic.List[string]

if ($payload.tool_input.file_path)     { $paths.Add([string]$payload.tool_input.file_path) }
if ($payload.tool_input.notebook_path) { $paths.Add([string]$payload.tool_input.notebook_path) }
if ($payload.tool_input.edits) {
    foreach ($e in $payload.tool_input.edits) {
        if ($e.file_path) { $paths.Add([string]$e.file_path) }
    }
}

if ($paths.Count -eq 0) { exit 0 }

# --- 허용 규칙: 마크다운 문서와 docs/ 아래만 ---

# --- 저장소 살림용 설정 파일. 게임 소스가 아니므로 허용한다 ---
$script:AllowedConfigFiles = @('.gitignore', '.gitattributes', '.editorconfig', '.graphifyignore')

function Test-PlannerAllowed([string]$p) {
    if ([string]::IsNullOrWhiteSpace($p)) { return $true }
    $norm = $p.Replace([char]92, [char]47)   # backslash -> slash (no regex)
    $ext  = [System.IO.Path]::GetExtension($norm).ToLowerInvariant()

    # --- 게임 소스 트리에는 키트 예외를 적용하지 않는다 ---
    # Unity 스크립트 폴더 이름이 Scripts 라서 scripts/ 예외에 걸리던 구멍을 막는다.
    if ($norm -match '(^|/)(assets|projectsettings|packages)/') {
        if ($ext -eq '.md' -or $ext -eq '.markdown') { return $true }
        return $false
    }
    if ($ext -eq '.md' -or $ext -eq '.markdown') { return $true }
    if ($norm -match '(^|/)docs/')               { return $true }
    if ($norm -match '(^|/)scripts/')             { return $true }
    if ($norm -match '(^|/)\.claude/')            { return $true }
    # graphify 생성물 폴더. gitignore 대상이고 도구가 스스로 관리한다
    if ($norm -match '(^|/)graphify-out/') { return $true }
    $leaf = [System.IO.Path]::GetFileName($norm).ToLowerInvariant()
    if ($script:AllowedConfigFiles -contains $leaf) { return $true }
    return $false
}

$blocked = @($paths | Where-Object { -not (Test-PlannerAllowed $_) })

if ($blocked.Count -gt 0) {
    $list = ($blocked | ForEach-Object { "  - $_" }) -join "`n"
    $msg = @"
[PLANNER MODE] 소스 파일 수정이 차단되었습니다.

차단된 대상:
$list

이 워크스페이스에서 당신의 역할은 아키텍트/기획자입니다 (CLAUDE.md 참고).
코드를 직접 쓰지 말고 다음을 하십시오:

  1. 이 변경을 TaskPlan/TASK_PLAN.md의 "구현 스텝"에 파일 경로까지 특정해서 적으십시오.
  2. 계획이 끝나면 PLAN_COMPLETED 를 출력하십시오.
  3. 구현은 Codex가 합니다:
     powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/handoff-to-codex.ps1

문서(.md) 및 docs/ 아래 파일 작성은 허용되어 있습니다.
"@
    Write-GuardError($msg)
    exit 2
}

exit 0
