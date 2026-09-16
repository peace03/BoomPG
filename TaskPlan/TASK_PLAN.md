# TASK_PLAN — 넉백 중 좌우 공중 제어

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 0. 시작 전 반드시 읽을 것

1. `docs/rules/code-style.md` — 네이밍, 인스펙터 필드 규칙(6.1)
2. `docs/design/gdd.md` 23.3절 — 이번 변경의 게임 디자인 근거

### 작업 성격

기존 파일 3개 수정 + 테스트 추가. Unity 에디터가 열려 있어도 무방합니다.
**씬·프리팹·`.asset` 파일을 수정하지 마십시오.**

### 멈춰야 할 때와 계속해야 할 때

- **멈춰야 하는 경우**: 이 계획서에 없는 파일을 고쳐야 할 때, 지시가 서로 모순될 때
- **멈추지 말 것**: 환경 상태 확인, 도구가 특정 명령을 거부할 때(그 항목만 건너뛰고 계속)

## 1. 목표 (Goal)

지금은 넉백으로 날아가는 중에도 `AirControl`(0.4) 이 전 방향에 적용되어, 밀려나는 방향의
**반대로 입력하면 브레이크가 걸린다.** 낙사가 주 처치 수단인 게임(P1)에서 이것은 넉백의 위협을
스스로 없애버린다. 반대로 공중 제어를 완전히 막으면 날아가는 동안 할 수 있는 것이 없어
[P2](../docs/design/gdd.md)("아슬아슬한 복귀")의 판단 여지가 사라진다.

**넉백 중에는 날아가는 방향으로는 가속도 감속도 할 수 없게 하고, 그 방향의 좌우로만 움직일 수
있게 한다.** 밀려나는 거리는 상대가 정하고, 착지 지점은 자신이 고른다.

## 2. 현재 구조 분석 (Current State)

`PlayerMotor.Update()` 는 속도를 세 성분으로 나눠 관리한다.

```csharp
_externalVelocity = Vector3.MoveTowards(_externalVelocity, Vector3.zero, drag * deltaTime);
_inputVelocity = _moveDirection * _movementConfig.MoveSpeed *
    (grounded ? 1f : _movementConfig.AirControl);
if (_hasHorizontalAssist && !grounded)
{
    _inputVelocity = _horizontalAssist;   // 제트팩이 공중 수평 이동을 덮어쓴다
}
```

- `_moveDirection` — `PlayerInputRelay` 가 카메라 기준으로 변환해 넘긴 월드 XZ 단위 방향
- `_externalVelocity` — 넉백·견인으로 주어진 외부 속도
- 제트팩이 추진 중이면 `_horizontalAssist` 가 입력 속도를 **통째로 덮어쓴다.** 이 우선순위는 유지한다
  (연료를 써서 전 방향 제어를 사는 것이 제트팩의 값어치다)

`KnockbackCalculator` 는 `MonoBehaviour` 가 아닌 정적 클래스이며 넉백 계산을 순수 함수로 담고 있다.
EditMode 테스트 12개가 이 클래스를 검증한다.

### 지켜야 할 컨벤션

- 네이밍: private 필드 `_camelCase` / 프로퍼티 PascalCase
- public 필드 금지. `[SerializeField] private` + 프로퍼티
- 인스펙터 노출 필드에는 `[Header]`·`[Tooltip]`·`[Range]` (code-style 6.1)
- 속성 순서: `[Header]` → `[Tooltip]` → `[Range]` → `[SerializeField]`
- `Update` 안에서 `GetComponent`·`Find`·`Camera.main` 호출 금지
- 주석은 한국어, **무엇이 아니라 왜**를 적는다
- C# 파일 UTF-8(BOM 없음), 들여쓰기 공백 4칸

## 3. 영향 범위 (Files & Architecture)

| 파일 | 변경 유형 | 역할 |
|---|---|---|
| `Scripts/Gameplay/Combat/KnockbackCalculator.cs` | 수정 | 좌우 투영 순수 함수 추가 |
| `Scripts/Gameplay/Config/MovementConfig.cs` | 수정 | 설정값 2개 추가 |
| `Scripts/Gameplay/Player/PlayerMotor.cs` | 수정 | 공중 입력 계산 분기 |
| `Scripts/Tests/EditMode/KnockbackCalculatorTests.cs` | 수정 | 테스트 4건 추가 |

경로는 모두 `Assets/_Project/` 아래입니다.

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — `KnockbackCalculator` 에 좌우 투영 함수 추가

- 대상: `Assets/_Project/Scripts/Gameplay/Combat/KnockbackCalculator.cs`
- 기존 메서드를 건드리지 말고 아래를 추가한다.

```csharp
/// <summary>
/// 넉백 중 이동 입력을 날아가는 방향의 좌우 성분으로만 제한한다.
/// 날아가는 축으로는 가속·감속이 불가능해지고, 착지 지점만 조정할 수 있다.
/// </summary>
/// <param name="moveDirection">카메라 기준으로 변환된 월드 XZ 이동 입력 (단위 벡터).</param>
/// <param name="knockbackVelocity">현재 외부 속도 (m/s). y 성분은 무시한다.</param>
/// <returns>좌우 축 성분만 남긴 이동 방향. 넉백의 수평 성분이 없으면 입력을 그대로 돌려준다.</returns>
public static Vector3 ProjectLateralInput(Vector3 moveDirection, Vector3 knockbackVelocity)
```

**구현 규칙**

1. `knockbackVelocity` 의 수평 성분을 뽑는다. `y` 는 0으로 둔다.
2. 수평 성분의 `sqrMagnitude` 가 `1e-6f` 미만이면 `moveDirection` 을 **그대로 반환**한다
   (수직으로만 뜬 경우에는 좌우를 정의할 수 없다).
3. 좌우 축은 `Vector3.Cross(Vector3.up, 수평넉백.normalized)` 로 구한다.
4. 입력의 좌우 성분만 남긴다: `lateralAxis * Vector3.Dot(moveDirection, lateralAxis)`.
5. `MonoBehaviour` 가 아니므로 Unity 생명주기 제약이 없다. 상태를 갖지 않는 순수 함수로 유지한다.

- 완료 기준: 컴파일 통과. Step 4의 테스트 4건이 통과한다.

### Step 2 — `MovementConfig` 에 설정값 2개 추가

- 대상: `Assets/_Project/Scripts/Gameplay/Config/MovementConfig.cs`
- `_airControl` 필드 **바로 다음에** 아래 두 필드를 추가한다. 기존 필드의 순서·기본값을 바꾸지 않는다.

| 필드 | 타입 | 기본값 | Header | Tooltip | Range |
|---|---|---|---|---|---|
| `_knockbackLateralControl` | float | `0.5f` | (없음 — `이동` 그룹에 이어짐) | `넉백으로 날아가는 중 좌우로 움직일 수 있는 정도 (비율). 날아가는 방향으로는 가속도 감속도 되지 않고, 이 값만큼 좌우로만 조정할 수 있다. 올리면 착지 지점을 고르기 쉬워져 낙사가 줄어든다.` | `[Range(0f, 1f)]` |
| `_knockbackControlThreshold` | float | `1f` | (없음) | `넉백 중으로 판정하는 수평 속도 하한 (m/s). 이 값보다 느려지면 일반 공중 조작으로 돌아간다. 높이면 넉백이 끝났다고 판단하는 시점이 빨라진다.` | — |

대응 프로퍼티도 기존 스타일대로 추가한다.

```csharp
/// <summary>넉백 중 좌우 조작 비율.</summary>
public float KnockbackLateralControl => _knockbackLateralControl;
/// <summary>넉백 판정 수평 속도 하한 (m/s).</summary>
public float KnockbackControlThreshold => _knockbackControlThreshold;
```

- 완료 기준: 컴파일 통과. 인스펙터의 `이동` 그룹에 두 항목이 보이고 슬라이더가 동작한다.

### Step 3 — `PlayerMotor` 의 공중 입력 계산 분기

- 대상: `Assets/_Project/Scripts/Gameplay/Player/PlayerMotor.cs`
- `Update()` 안에서 `_inputVelocity` 를 계산하는 아래 두 줄을 교체한다.

```csharp
// 기존 — 이 두 줄을 지운다
_inputVelocity = _moveDirection * _movementConfig.MoveSpeed *
    (grounded ? 1f : _movementConfig.AirControl);
```

```csharp
// 교체 후
if (grounded)
{
    _inputVelocity = _moveDirection * _movementConfig.MoveSpeed;
}
else
{
    // 넉백 중에는 날아가는 축으로 가속·감속할 수 없게 하고 좌우만 허용한다.
    // 반대로 입력해 브레이크를 거는 것이 가능하면 낙사의 위협이 사라진다 (gdd 23.3).
    Vector3 horizontalKnockback = new Vector3(_externalVelocity.x, 0f, _externalVelocity.z);
    if (horizontalKnockback.magnitude >= _movementConfig.KnockbackControlThreshold)
    {
        Vector3 lateral = KnockbackCalculator.ProjectLateralInput(_moveDirection, horizontalKnockback);
        _inputVelocity = lateral * (_movementConfig.MoveSpeed * _movementConfig.KnockbackLateralControl);
    }
    else
    {
        _inputVelocity = _moveDirection * _movementConfig.MoveSpeed * _movementConfig.AirControl;
    }
}
```

- 그 아래의 제트팩 분기(`if (_hasHorizontalAssist && !grounded) { _inputVelocity = _horizontalAssist; }`)
  는 **그대로 둔다.** 제트팩을 쓰는 동안에는 전 방향 제어가 유지되어야 한다 —
  연료를 지불하고 사는 값어치다.
- `KnockbackCalculator` 는 `BoomPG.Gameplay.Combat` 네임스페이스에 있다. `using` 을 추가한다.
- **`Update()` 의 다른 부분을 수정하지 마십시오.**

- 완료 기준: 컴파일 통과. 넉백으로 날아가는 중 전진·후진 입력이 속도를 바꾸지 못하고,
  좌우 입력으로만 궤도가 휘어진다.

### Step 4 — EditMode 테스트 4건 추가

- 대상: `Assets/_Project/Scripts/Tests/EditMode/KnockbackCalculatorTests.cs`
- 기존 테스트를 수정하지 말고 아래를 추가한다.

| # | 메서드명 | 입력 | 기대 |
|---|---|---|---|
| 1 | `ProjectLateralInput_PerpendicularInput_PassesThrough` | 넉백 `(0,0,18)`, 입력 `(1,0,0)` | 결과가 `(1,0,0)` (크기 1, 방향 유지) |
| 2 | `ProjectLateralInput_ForwardInput_ReturnsZero` | 넉백 `(0,0,18)`, 입력 `(0,0,1)` | 결과 크기가 0 — **날아가는 방향으로는 조작이 먹지 않는다** |
| 3 | `ProjectLateralInput_BackwardInput_ReturnsZero` | 넉백 `(0,0,18)`, 입력 `(0,0,-1)` | 결과 크기가 0 — **브레이크를 걸 수 없다** |
| 4 | `ProjectLateralInput_NoHorizontalKnockback_ReturnsInput` | 넉백 `(0,12,0)`, 입력 `(1,0,0)` | 결과가 입력과 동일 `(1,0,0)` |

- 부동소수 비교는 `Assert.AreEqual(expected, actual, 0.001f)` 를 쓴다.
- 3번은 이번 변경의 핵심이다. 이것이 통과해야 "넉백을 브레이크로 취소할 수 없다"가 보장된다.
- 완료 기준: 기존 12건 + 신규 4건 = **총 16건이 통과**한다.

### Step 5 — 컴파일 확인과 테스트 실행

- `mcp__unity__*` 툴이 보이면 에셋 새로고침 → 컴파일 에러 확인 → EditMode 테스트 실행
  → 결과를 **실제 출력 그대로** 보고한다.
- 툴이 없으면 "컴파일·테스트 확인 불가"라고 보고에 적는다. 배치 모드를 시도하지 않는다
  (이 환경에서는 라이선스 오류로 실패한다).

## 5. 인수 조건 (Acceptance Criteria)

- [ ] `KnockbackCalculator.ProjectLateralInput` 이 계획서 시그니처대로 존재한다
- [ ] 이 함수가 상태를 갖지 않고 Unity 생명주기에 의존하지 않는다
- [ ] `MovementConfig` 에 `KnockbackLateralControl`(기본 0.5) 과
      `KnockbackControlThreshold`(기본 1.0) 프로퍼티가 있다
- [ ] 두 필드에 `[Tooltip]` 이 있고, `_knockbackLateralControl` 에 `[Range(0f, 1f)]` 가 있다
- [ ] `MovementConfig` 의 기존 필드 순서와 기본값이 바뀌지 않았다
- [ ] `PlayerMotor` 가 접지 / 넉백 중 / 일반 공중의 세 갈래로 입력을 계산한다
- [ ] 제트팩 분기(`_hasHorizontalAssist`)가 여전히 입력 속도를 덮어쓴다
- [ ] EditMode 테스트 **16건 전부 통과** (기존 12 + 신규 4)
- [ ] 특히 `ProjectLateralInput_BackwardInput_ReturnsZero` 가 통과한다
- [ ] 컴파일 에러가 없다
- [ ] `git status --short` 에 위 4개 파일 외의 변경이 없다
      (`Assets/_Project/Data/SO_CombatConfig.asset` 은 사람이 조정 중이므로 예외)

## 6. 테스트 계획 (Test Plan)

| 확인 | 기대 결과 |
|---|---|
| Unity MCP 로 에셋 새로고침 + 컴파일 확인 | 에러 0건 |
| Unity MCP 로 EditMode 테스트 실행 | `16 passed, 0 failed` |
| `git diff --stat` | 지정한 4개 파일만 변경 |

플레이 감각 확인은 사람이 한다. Codex 는 컴파일과 테스트까지만 검증하고 보고한다.

## 7. 범위 밖 (Out of Scope)

- **`SO_MovementConfig.asset` 수정** — 새 필드는 Unity 가 기본값으로 직렬화한다.
  `.asset` 파일을 직접 편집하지 마라
- **제트팩 동작 변경** — 추진 중 전 방향 제어는 의도된 것이다
- **`AirControl` 기본값 변경** — 넉백이 없는 일반 점프에는 기존 값을 그대로 쓴다
- 견인(`isPull`) 중의 제어 — 갈고리는 M3 범위다. 지금은 넉백과 같은 경로를 타도 무방하다
- 씬·프리팹 수정
- `docs/`, `TaskPlan/`, `scripts/`, `.claude/` 아래 전부
- git 커밋 · 브랜치 조작 — 커밋은 사람이 한다
