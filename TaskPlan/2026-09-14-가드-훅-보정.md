# TASK_PLAN — 가드 훅 스크립트 보정 (인코딩 · 오탐 · 자기유지)

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 1. 목표 (Goal)

방금 프로젝트에 적용한 PreToolUse 가드 훅 2종(`scripts/guard-planner.ps1`, `scripts/guard-bash.ps1`)에서 실사용 중 확인된 결함 3가지를 고친다. (1) 차단 메시지가 한글 깨짐으로 전달되어 읽을 수 없다. (2) `guard-bash.ps1`이 `System.IO.File`, `0..2` 같은 비(非)경로 토큰을 파일 경로로 오탐해 정상 명령을 차단한다. (3) 가드가 워크플로우 키트 자신(`scripts/`, `.claude/`)의 수정까지 막아서, 가드를 고치는 것이 불가능해졌다. 세 문제 모두 가드를 실제로 쓸 수 없게 만드는 차단성 결함이다.

## 2. 현재 구조 분석 (Current State)

- 관련 파일과 현재 동작:
  - `scripts/guard-planner.ps1` — PreToolUse(Edit/Write/MultiEdit/NotebookEdit) 훅. stdin JSON에서 대상 파일 경로를 뽑아 `.md`/`docs/`만 허용, 나머지는 stderr 출력 후 `exit 2`.
  - `scripts/guard-bash.ps1` — PreToolUse(Bash/PowerShell) 훅. 명령 문자열에서 리다이렉션 대상·tee 대상·쓰기 명령 인자를 뽑아 같은 규칙으로 판정.
  - `.claude/settings.json` — 위 두 훅 등록. **이번 작업에서 수정하지 않는다.**
  - `CLAUDE.md` — Claude의 역할·허용 범위 문서.

- 지켜야 할 기존 컨벤션:
  - 두 스크립트 모두 **UTF-8 BOM + CRLF**로 저장한다. BOM이 없으면 PowerShell 5.1이 CP949로 읽어 한글 주석에서 파싱 에러가 난다. 수정 후 반드시 BOM을 유지할 것.
  - 주석은 한국어, `# --- 섹션명 ---` 형식.
  - 훅 규약: `exit 0` 통과 / `exit 2` 차단(stderr가 Claude에게 전달) / 그 외 경고.
  - 입력 JSON 파싱 실패 시 fail-open(`exit 1`), 작업을 막지 않는다.

## 3. 영향 범위 (Files & Architecture)

| 파일 | 변경 유형 | 역할 |
|---|---|---|
| scripts/guard-planner.ps1 | 수정 | stderr UTF-8 출력, 키트 경로 예외 추가 |
| scripts/guard-bash.ps1 | 수정 | stderr UTF-8 출력, 키트 경로 예외, 히어독 본문 제외, 경로 오탐 제거 |
| CLAUDE.md | 수정 | 허용 범위에 키트 경로 예외 명시 |

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — 두 스크립트에 UTF-8 stderr 출력 함수 추가

- 대상: `scripts/guard-planner.ps1`, `scripts/guard-bash.ps1`
- 변경: 각 파일에서 `$ErrorActionPreference = 'Stop'` 바로 다음 줄에 아래 함수를 추가한다. 두 파일에 동일한 내용으로 넣는다.

```powershell
# --- stderr 를 UTF-8 로 직접 쓴다 (PS 5.1 기본 인코딩이면 한글이 깨진다) ---
function Write-GuardError([string]$text) {
    $utf8 = New-Object System.Text.UTF8Encoding($false)
    $w = New-Object System.IO.StreamWriter([Console]::OpenStandardError(), $utf8)
    $w.AutoFlush = $true
    $w.WriteLine($text)
    $w.Flush()
}
```

- 그리고 두 파일에 있는 **모든** `[Console]::Error.WriteLine(...)` 호출을 `Write-GuardError(...)` 로 치환한다. `guard-planner.ps1`에 2곳(JSON 파싱 실패 경고, `$msg` 출력), `guard-bash.ps1`에 2곳(동일)이 있다.
- 완료 기준: 두 파일에 `[Console]::Error.WriteLine` 문자열이 하나도 남아 있지 않다. `Write-GuardError` 함수 정의가 각 파일에 정확히 1개 있다.

### Step 2 — `guard-planner.ps1`: 워크플로우 키트 경로 예외

- 대상: `scripts/guard-planner.ps1`
- 변경: 함수 `Test-PlannerAllowed` 안에서, 기존의 `if ($norm -match '(^|/)docs/') { return $true }` 줄 **바로 다음에** 아래 두 줄을 추가한다.

```powershell
    if ($norm -match '(^|/)scripts/')             { return $true }
    if ($norm -match '(^|/)\.claude/')            { return $true }
```

- 이유: 이 두 경로는 기획 워크플로우 키트 자신이며 게임 소스가 아니다. 예외가 없으면 가드가 자기 자신을 수정 불가로 만든다.
- 완료 기준: `Test-PlannerAllowed`가 `scripts/guard-bash.ps1`, `.claude/settings.json`에 대해 `$true`를, `BoomPG/Assets/Player.cs`에 대해 `$false`를 반환한다.

### Step 3 — `guard-bash.ps1`: 같은 키트 경로 예외

- 대상: `scripts/guard-bash.ps1`
- 변경: 함수 `Test-BashTargetAllowed` 안에서, 기존의 `if ($p -match '(^|/)docs/') { return $true }` 줄 **바로 다음에** 아래 두 줄을 추가한다.

```powershell
    if ($p -match '(^|/)scripts/')  { return $true }
    if ($p -match '(^|/)\.claude/') { return $true }
```

- 완료 기준: Step 6의 케이스 7, 8이 통과한다.

### Step 4 — `guard-bash.ps1`: 히어독 본문을 검사 대상에서 제외

- 대상: `scripts/guard-bash.ps1`
- 문제: 명령 문자열에 히어독(`cat > TASK_PLAN.md <<'EOF' ... EOF`)이 들어오면 **본문 텍스트까지** 검사 대상이 되어, 계획서에 코드 예시를 적는 것만으로 차단된다.
- 변경: `$cmd` 유효성 검사 직후(`if ([string]::IsNullOrWhiteSpace($cmd)) { exit 0 }` 다음)에 아래 블록을 추가하고, **이후의 모든 검사에서 `$cmd` 대신 `$scan` 을 사용**하도록 바꾼다. 단 차단 메시지에 실행 명령을 보여주는 부분은 원본 `$cmd`를 그대로 쓴다.

```powershell
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
```

- 구체적으로 `$cmd` 를 `$scan` 으로 바꿔야 하는 위치는 다음 4곳이다.
  1. `foreach ($h in $hardBlock) { if ($cmd -match $h.p) ... }` 의 `$cmd`
  2. 리다이렉션 추출 `[regex]::Matches($cmd, ...)` 의 `$cmd`
  3. tee 추출 `[regex]::Matches($cmd, ...)` 의 `$cmd`
  4. `foreach ($ind in $pathWriteIndicators) { if ($cmd -match $ind) ... }` 의 `$cmd`
  5. 확장자 스캔 `[regex]::Matches($cmd, ...)` 의 `$cmd`
- 완료 기준: Step 6의 케이스 9가 통과한다.

### Step 5 — `guard-bash.ps1`: 경로 오탐 제거

- 대상: `scripts/guard-bash.ps1`
- 문제: 확장자 스캔이 `System.IO.File`(→ 확장자 `.File`), `0..2`(→ `.2`) 같은 비경로 토큰을 파일 경로로 잡아 정상 명령을 차단한다.
- 변경 5-1: 파일 상단의 경로 판정 헬퍼들(`Test-BashTargetAllowed` 정의 앞)에 아래 함수를 추가한다.

```powershell
# --- 확장자 스캔 결과가 '진짜 파일 경로처럼 보이는가' 판정 ---
$script:KnownFileExt = @(
    '.cs','.js','.mjs','.cjs','.ts','.tsx','.jsx','.py','.ps1','.psm1','.psd1',
    '.sh','.bash','.bat','.cmd','.json','.xml','.yaml','.yml','.toml','.ini',
    '.cfg','.config','.csproj','.sln','.slnx','.asmdef','.asmref','.shader',
    '.cginc','.hlsl','.compute','.unity','.prefab','.asset','.mat','.anim',
    '.controller','.meta','.txt','.csv','.tsv','.log','.html','.htm','.css',
    '.scss','.sql','.java','.kt','.c','.h','.hpp','.cpp','.cc','.go','.rs',
    '.rb','.php','.lua','.png','.jpg','.jpeg','.gif','.tga','.psd','.wav',
    '.mp3','.ogg','.fbx','.obj','.blend','.dll','.exe','.gitignore','.editorconfig'
)

function Test-LooksLikePath([string]$token) {
    if ([string]::IsNullOrWhiteSpace($token)) { return $false }
    $t = $token.Trim().Trim([char[]]@(34, 39)).Replace([char]92, [char]47)
    if ($t -match '^\d+(\.\d+)*\.?\.?\d*$') { return $false }   # 0..2, 1.2.3 같은 숫자/범위
    if ($t.Contains('/')) { return $true }                       # 슬래시가 있으면 경로로 본다
    $ext = ''
    try { $ext = [System.IO.Path]::GetExtension($t).ToLowerInvariant() } catch { return $false }
    return ($script:KnownFileExt -contains $ext)
}
```

- 변경 5-2: 확장자 스캔 루프에서 `Test-LooksLikePath`를 통과한 토큰만 `$targets`에 넣는다. 기존 코드

```powershell
    $extM = [regex]::Matches($cmd, '(?<![\w.])(?<t>(?:[A-Za-z]:[\\/])?[\w.\-/\\~]*\.[A-Za-z0-9]{1,10})(?![\w.])')
    foreach ($m in $extM) { $targets.Add($m.Groups['t'].Value) }
```

를 아래로 바꾼다. (`$cmd` → `$scan` 치환은 Step 4와 동일하게 적용된 상태여야 한다.)

```powershell
    $extM = [regex]::Matches($scan, '(?<![\w.])(?<t>(?:[A-Za-z]:[\\/])?[\w.\-/\\~]*\.[A-Za-z0-9]{1,10})(?![\w.])')
    foreach ($m in $extM) {
        $tok = $m.Groups['t'].Value
        if (Test-LooksLikePath $tok) { $targets.Add($tok) }
    }
```

- 변경 5-3: 차단 메시지에 명령 전문을 그대로 넣으면 너무 길어진다. `$msg` 히어스트링에서 `  $cmd` 로 쓰인 부분을, 그 위에서 미리 만들어 둔 `$cmdShown` 변수로 바꾼다. `$msg` 조립 직전에 아래를 추가한다.

```powershell
$cmdShown = $cmd
if ($cmdShown.Length -gt 400) { $cmdShown = $cmdShown.Substring(0, 400) + ' ...(생략)' }
```

- 완료 기준: Step 6의 케이스 5, 6이 통과한다.

### Step 6 — 검증 스크립트 작성 및 실행

- 대상: `scripts/test-guards.ps1` (신규)
- 변경: 아래 케이스를 stdin JSON으로 각 가드에 넘겨 종료 코드를 비교하고, `PASS`/`FAIL`을 케이스별로 출력한 뒤 전체 실패 개수를 마지막 줄에 `FAILED: <n>` 형식으로 출력하는 PowerShell 스크립트를 만든다. 각 케이스 실행 전 `$env:CLAUDE_PROJECT_DIR` 를 이 저장소의 절대 경로로 설정한다.

| # | 가드 | tool_name | 입력 | 기대 exit |
|---|---|---|---|---|
| 1 | guard-planner | Write | file_path `BoomPG/Assets/Player.cs` | 2 |
| 2 | guard-planner | Write | file_path `TASK_PLAN.md` | 0 |
| 3 | guard-planner | Bash | command `ls` | 0 |
| 4 | guard-planner | Write | file_path `scripts/guard-bash.ps1` | 0 |
| 5 | guard-bash | PowerShell | command `[System.IO.File]::ReadAllBytes($f)[0..2]` | 0 |
| 6 | guard-bash | Bash | command `cat BoomPG/Assets/Player.cs > /dev/null` | 0 |
| 7 | guard-bash | PowerShell | command `Set-Content -LiteralPath scripts/guard-bash.ps1 -Value $c` | 0 |
| 8 | guard-bash | Bash | command `sed -i s/a/b/ BoomPG/Assets/Player.cs` | 2 |
| 9 | guard-bash | Bash | 히어독으로 `TASK_PLAN.md` 쓰기. 본문 안에 `Out-File BoomPG/Assets/Player.cs` 라는 줄을 포함시킨다 | 0 |
| 10 | guard-bash | Bash | command `echo x > BoomPG/Assets/Player.cs` | 2 |
| 11 | guard-bash | Bash | command `git apply fix.patch` | 2 |
| 12 | guard-bash | Bash | command `git diff --stat` | 0 |
| 13 | guard-bash | Bash | command `cp README.md BoomPG/Assets/Player.cs` | 2 |
| 14 | guard-bash | Bash | command `rm C:/Users/mbc/AppData/Local/Temp/x.cs` | 0 |

- 완료 기준: `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/test-guards.ps1` 실행 시 마지막 줄이 `FAILED: 0` 이다.

### Step 7 — 인코딩 확인

- 대상: `scripts/guard-planner.ps1`, `scripts/guard-bash.ps1`, `scripts/test-guards.ps1`
- 변경: 세 파일이 모두 UTF-8 **BOM**(`EF BB BF`)으로 시작하는지 확인하고, 아니면 BOM을 붙여 다시 저장한다. 줄바꿈은 CRLF로 맞춘다.
- 완료 기준: 세 파일 각각 첫 3바이트가 `239 187 191`이다.

### Step 8 — `CLAUDE.md` 예외 명시

- 대상: `CLAUDE.md`
- 변경: "## 핵심 규칙" 섹션의 첫 번째 불릿(`- **소스 코드 파일을 직접 수정하지 마십시오.** ...`) 바로 아래에 아래 불릿을 추가한다.

```markdown
- 예외: `scripts/` 와 `.claude/` 아래는 기획 워크플로우 키트 자신이므로 수정이 허용됩니다.
  게임 소스(`BoomPG/Assets/**`)는 어떤 경우에도 직접 쓰지 마십시오.
```

- 완료 기준: `CLAUDE.md`에 위 문장이 포함되어 있다.

## 5. 인수 조건 (Acceptance Criteria)

- [ ] `scripts/guard-planner.ps1`, `scripts/guard-bash.ps1` 에 `[Console]::Error.WriteLine` 이 남아 있지 않다.
- [ ] 두 스크립트의 차단 메시지가 UTF-8로 출력되어 한글이 깨지지 않는다.
- [ ] `scripts/test-guards.ps1` 실행 결과 마지막 줄이 `FAILED: 0` 이다.
- [ ] `guard-bash.ps1` 이 `[System.IO.File]::ReadAllBytes($f)[0..2]` 를 더 이상 차단하지 않는다 (케이스 5).
- [ ] `guard-bash.ps1` 이 `echo x > BoomPG/Assets/Player.cs` 를 여전히 차단한다 (케이스 10).
- [ ] 세 스크립트가 UTF-8 BOM으로 저장되어 있다.
- [ ] `CLAUDE.md` 에 키트 경로 예외가 명시되어 있다.
- [ ] `.claude/settings.json` 은 변경되지 않았다.

## 6. 테스트 계획 (Test Plan)

| 명령어 | 기대 결과 |
|---|---|
| `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/test-guards.ps1` | 케이스 1~14 전부 PASS, 마지막 줄 `FAILED: 0` |
| `powershell -NoProfile -ExecutionPolicy Bypass -Command "([System.IO.File]::ReadAllBytes('scripts/guard-bash.ps1')[0..2]) -join ','"` | `239,187,191` |
| `powershell -NoProfile -ExecutionPolicy Bypass -Command "([System.IO.File]::ReadAllBytes('scripts/guard-planner.ps1')[0..2]) -join ','"` | `239,187,191` |
| `git status --short` | `scripts/`, `CLAUDE.md` 외의 파일이 변경되어 있지 않음 |

## 7. 범위 밖 (Out of Scope)

- `.claude/settings.json` 수정 — 훅 등록은 이미 정상이다. 건드리지 말 것.
- `scripts/handoff-to-codex.ps1` 수정.
- `AGENTS.md`, `TASK_PLAN.template.md`, `README.md` 수정.
- `BoomPG/` 아래 Unity 프로젝트 파일 일체.
- git 커밋 / 브랜치 조작 — 커밋은 사람이 한다.
- 이 계획서(`TASK_PLAN.md`) 자체의 수정.
