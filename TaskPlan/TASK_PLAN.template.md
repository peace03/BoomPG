# TASK_PLAN — <작업 이름>

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 1. 목표 (Goal)

<무엇을, 왜. 한 문단.>

## 2. 현재 구조 분석 (Current State)

- 관련 파일과 현재 동작:
  - path/to/file.cs — <지금 하는 일>
- 지켜야 할 기존 컨벤션:
  - 네이밍: <예: private 필드는 _camelCase>
  - 에러 처리: <예: 예외를 던지지 않고 bool + out 반환>
  - 로깅: <예: Debug.Log 대신 GameLogger.Info>

## 3. 영향 범위 (Files & Architecture)

| 파일 | 변경 유형 | 역할 |
|---|---|---|
| path/to/A.cs | 수정 | <무엇을> |
| path/to/B.cs | 신규 | <무엇을> |

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — <제목>

- 대상: path/to/file.cs
- 변경: <시그니처·타입·함수명까지 특정>
- 완료 기준: <예: 컴파일 통과 + 빈 id에 false 반환>

### Step 2 — <제목>

- 대상:
- 변경:
- 완료 기준:

## 5. 인수 조건 (Acceptance Criteria)

- [ ] <검증 가능한 조건 1>
- [ ] <검증 가능한 조건 2>
- [ ] 기존 테스트가 모두 통과한다

## 6. 테스트 계획 (Test Plan)

| 명령어 | 기대 결과 |
|---|---|
| <실행할 명령> | <기대 출력> |

## 7. 범위 밖 (Out of Scope)

- <손대면 안 되는 것>
- 이 계획서(TASK_PLAN.md) 자체의 수정
