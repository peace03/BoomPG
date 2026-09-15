[CmdletBinding()]
param(
    [string]$Plan    = './TaskPlan/TASK_PLAN.md',
    [string]$Cd      = '.',
    [ValidateSet('read-only', 'workspace-write', 'danger-full-access')]
    [string]$Sandbox = 'workspace-write',
    [string]$Model,
    [switch]$UnityMcp,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

# --- 인코딩: PS 5.1 기본값이면 한글이 전부 '?' 로 깨진다 ---
# $OutputEncoding 은 네이티브 exe 로 '파이프'할 때 쓰이는 인코딩이며 5.1 기본값이 ASCII 다.
# 이걸 UTF-8 로 올리지 않으면 계획서 전문이 물음표로 바뀐 채 Codex 에게 전달된다.
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$OutputEncoding = $utf8NoBom
try { [Console]::OutputEncoding = $utf8NoBom } catch { }

# --- 사전 점검 (DryRun은 codex 없이도 동작) ---
if (-not $DryRun -and -not (Get-Command codex -ErrorAction SilentlyContinue)) {
    Write-Host "codex CLI 를 찾을 수 없습니다." -ForegroundColor Red
    Write-Host "  winget install OpenAI.Codex   또는   npm install -g @openai/codex"
    Write-Host "설치 후 로그인:  codex login"
    exit 1
}

if (-not (Test-Path -LiteralPath $Plan)) {
    Write-Host "계획서를 찾을 수 없습니다: $Plan" -ForegroundColor Red
    Write-Host "먼저 Claude Code로 계획을 세우십시오 (CLAUDE.md 참고). 기본 경로는 ./TaskPlan/TASK_PLAN.md 입니다."
    exit 1
}

$planFull = (Resolve-Path -LiteralPath $Plan).Path
$cdFull   = (Resolve-Path -LiteralPath $Cd).Path
$planText = Get-Content -LiteralPath $planFull -Raw -Encoding UTF8

if ([string]::IsNullOrWhiteSpace($planText)) {
    Write-Host "계획서가 비어 있습니다: $planFull" -ForegroundColor Red
    exit 1
}

if ($planText -notmatch '(?im)^\s*#{1,6}\s') {
    Write-Warning "계획서에 섹션 제목이 없어 보입니다. 양식을 확인하세요."
}

# --- Codex에게 줄 프롬프트 조립 ---
$planRel = [System.IO.Path]::GetFileName($planFull)

$prompt = @"
당신은 구현 에이전트입니다. 이 저장소의 AGENTS.md 규칙을 먼저 따르십시오.

아래는 아키텍트가 작성한 구현 계획서($planRel)의 전문입니다.
계획서의 "구현 스텝"을 적힌 순서대로 정확히 구현하십시오.

지켜야 할 것:
- 계획서에 명시되지 않은 파일은 수정하지 마십시오.
- "범위 밖(Out of Scope)" 항목은 손대지 마십시오.
- 새로운 아키텍처를 설계하지 마십시오. 계획서가 틀렸다면 멈추고 보고하십시오.
- $planRel 파일 자체는 수정하지 마십시오.
- 구현이 끝나면 "테스트 계획"의 명령어를 실행하고, "인수 조건"을 항목별로 대조해 보고하십시오.
- 테스트가 실패하면 실패한 사실과 실제 출력을 그대로 보고하십시오.
- 커밋하지 마십시오. 변경 사항만 작업 트리에 남겨두십시오. 커밋은 사람이 합니다.
- 보고와 코드 주석은 한국어로 작성하십시오.

===== BEGIN $planRel =====
$planText
===== END $planRel =====
"@

if ($UnityMcp) {
    $prompt += @"

추가 지시 — Unity MCP:
이 세션에는 mcp__unity__* 툴이 붙어 있습니다. 실행 중인 Unity Editor에 직접 접근할 수 있습니다.
- 스크립트를 수정한 뒤 에셋 새로고침/컴파일 관련 툴로 컴파일 에러 유무를 반드시 확인하십시오.
- 테스트가 있으면 mcp__unity__run_tests 로 실행하고 mcp__unity__test_status 로 결과를 확인하십시오.
- 보고에는 툴이 돌려준 실제 출력을 그대로 넣으십시오. 추측으로 "통과"라고 쓰지 마십시오.
- MCP 툴은 샌드박스 밖에서 동작합니다. 계획서 범위 밖 파일을 MCP 쓰기 툴로 건드리지 마십시오.
"@
}

# --- codex exec 실행 ---
if ($UnityMcp) {
    # --approve-for-me 는 --sandbox 와 동시에 쓸 수 없다. 샌드박스는 workspace-write 로 고정된다.
    if ($PSBoundParameters.ContainsKey('Sandbox')) {
        Write-Warning "-UnityMcp 사용 시 -Sandbox 는 무시됩니다 (codex 가 workspace-write 로 고정)."
    }
    $sandboxLabel = 'workspace-write (--approve-for-me · MCP 호출 허용)'
    $codexArgs = @('exec', '--cd', $cdFull, '--approve-for-me')
} else {
    $sandboxLabel = $Sandbox
    $codexArgs = @('exec', '--cd', $cdFull, '--sandbox', $Sandbox)
}
if ($Model) { $codexArgs += @('--model', $Model) }
$codexArgs += '-'   # 프롬프트를 stdin 으로 받는다

Write-Host ""
Write-Host "계획서 : $planFull"
Write-Host "작업경로: $cdFull"
Write-Host "샌드박스: $sandboxLabel"
Write-Host "명령    : codex $($codexArgs -join ' ')   (프롬프트는 stdin)"
Write-Host ""

if ($DryRun) {
    Write-Host "--- DryRun: 아래가 Codex에게 전달될 프롬프트입니다. ---" -ForegroundColor Yellow
    Write-Host $prompt
    exit 0
}

if (-not $UnityMcp -and $Sandbox -eq 'danger-full-access') {
    Write-Warning "danger-full-access 로 실행합니다. Codex가 저장소 밖 파일도 건드릴 수 있습니다."
}

Write-Host "Codex 구현 시작..." -ForegroundColor Cyan
Write-Host ""

$prompt | & codex @codexArgs
$code = $LASTEXITCODE

Write-Host ""
if ($code -eq 0) {
    Write-Host "Codex 종료 (exit 0). 변경 내용을 확인하세요:" -ForegroundColor Green
    Write-Host "  git -C `"$cdFull`" status"
    Write-Host "  git -C `"$cdFull`" diff"
    Write-Host "검토 후 커밋은 사람이 직접 합니다 (Codex는 .git/ 쓰기 불가)."
} else {
    Write-Host "Codex 가 exit $code 로 종료했습니다. 위 출력을 확인하세요." -ForegroundColor Red
}
exit $code
