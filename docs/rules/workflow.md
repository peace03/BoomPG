# 작업 흐름 (Workflow)

BoomPG는 역할이 분리된 다중 에이전트 워크플로우로 만듭니다.
각자 할 수 있는 일과 **할 수 없는 일**이 시스템 레이어에서 강제됩니다.

## 1. 역할

| 주체 | 역할 | 쓸 수 있는 것 | 쓸 수 없는 것 |
|---|---|---|---|
| 사람 (YunPyeongHwa) | 의사결정자 · 최종 검토 · 커밋 | 전부 | — |
| Claude Code | 아키텍트 · 기획자 | `docs/**`, `TaskPlan/**`, `*.md`, `scripts/**`, `.claude/**` | 게임 소스 (`Assets/**`, `ProjectSettings/**`, `Packages/**`) |
| Codex | 구현자 | `TaskPlan/TASK_PLAN.md`에 명시된 파일만 | 계획서 밖 파일, `docs/**`, `TaskPlan/**`, git 커밋 |
| BARCO AI | 3D 에셋 생성 | 모델 산출물 | — |

Claude의 소스 수정 차단은 `.claude/settings.json`의 PreToolUse 훅
(`scripts/guard-planner.ps1`, `scripts/guard-bash.ps1`)이 담당합니다.
편집 도구뿐 아니라 Bash를 통한 우회 쓰기도 막습니다.

Codex의 커밋 차단은 `--sandbox workspace-write`가 `.git/` 쓰기를 막아 실현됩니다.

## 2. 한 사이클

```
요구사항 (사람)
   ↓
[Claude] 코드베이스·문서 분석
   ↓
[Claude] 애매한 지점을 질문 → 사람이 답변        ← 답변은 즉시 docs/에 반영
   ↓
[Claude] TaskPlan/TASK_PLAN.md 작성 → PLAN_COMPLETED
   ↓
[사람] /handoff
   ↓
[Codex] 계획서의 구현 스텝을 순서대로 실행 → 테스트 → 보고
   ↓
[Claude] git diff와 인수 조건 대조 보고
   ↓
[사람] 검토 후 직접 커밋
   ↓
[Claude] 계획서를 TaskPlan/YYYY-MM-DD-작업명.md 로 보관, docs/ 갱신
```

## 3. 지켜야 할 것

- **계획서 없이 구현하지 않는다.** Codex는 `TaskPlan/TASK_PLAN.md`만 읽고 움직입니다.
  계획서가 모호하면 Codex는 설계 판단을 하지 않고 잘못 만듭니다. 모호함은 전부 Claude의 책임입니다.
- **계획서가 틀렸으면 Codex는 멈추고 보고한다.** 임의 수정 금지. 설계 변경 권한은 Claude에게 있습니다.
- **커밋은 사람만 한다.** 커밋 메시지는 한국어, `타입: 요약` 형식 (`CLAUDE.md` 참조).
- **문서 갱신은 Claude만 한다.** Codex는 `docs/`를 읽기만 합니다.
  구현 중 문서와 다른 점을 발견하면 문서를 고치지 말고 보고합니다.

## 4. Unity Editor 연동 (Unity MCP)

Codex 세션에 `mcp__unity__*` 툴이 보이면 실행 중인 Unity Editor에 직접 접근할 수 있습니다.
스크립트 수정 후 컴파일 에러 확인과 테스트 실행에 사용합니다.

주의: MCP 쓰기 툴은 Codex 샌드박스 제한을 받지 않습니다. 계획서 범위 밖 파일을 MCP로
건드리면 막아주는 장치가 없습니다. 상세는 [AGENTS.md](../../AGENTS.md) 참조.

## 5. 명령어

| 명령 | 용도 |
|---|---|
| `/handoff` | `TaskPlan/TASK_PLAN.md`를 Codex에게 넘겨 구현시킨다 |
| `/handoff --DryRun` | 실제 실행 없이 넘길 프롬프트만 확인 |
| `/handoff --UnityMcp` | Unity MCP 툴을 켠 채로 핸드오프 |
| `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/test-guards.ps1` | 가드 훅 회귀 테스트 |

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성 |
