---
description: TASK_PLAN.md를 Codex에게 넘겨 구현시킨다 (핸드오프)
allowed-tools: Bash(powershell:*), Read
argument-hint: "[--UnityMcp] [--DryRun] [--Sandbox workspace-write|danger-full-access] [--Model <name>]"
---

`TaskPlan/TASK_PLAN.md` 를 Codex에게 핸드오프한다.

절차:

1. TaskPlan/TASK_PLAN.md를 읽고 아래를 먼저 점검한다.
   - 구현 스텝마다 대상 파일 경로가 특정되어 있는가
   - 인수 조건이 검증 가능한 형태인가
   - 범위 밖(Out of Scope) 항목이 적혀 있는가
   비어 있거나 모호하면 핸드오프하지 말고 무엇이 부족한지 보고한다.

2. 점검을 통과하면 아래를 실행한다. 추가 인자: $ARGUMENTS

   powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/handoff-to-codex.ps1 $ARGUMENTS

3. Codex 출력이 끝나면 git status와 git diff --stat으로 실제 변경된 파일을 확인하고,
   TaskPlan/TASK_PLAN.md의 인수 조건과 대조해 항목별로 보고한다.
   테스트가 실패했으면 실패했다고 그대로 보고한다.

4. 커밋은 하지 않는다. Codex도 못 하고 여기서도 하지 않는다.
   사용자가 diff를 보고 직접 커밋한다.
