# TASK_PLAN — 설정 에셋 인스펙터 가독성 개선 (Header · Tooltip · Range)

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 0. 시작 전 반드시 읽을 것

`docs/rules/code-style.md` 의 **6.1 인스펙터에 노출되는 필드** 절. 이번 작업은 그 규칙을
기존 코드에 적용하는 것입니다.

### 작업 성격

**속성(attribute) 추가만 합니다.** 로직·시그니처·기본값·필드 순서를 바꾸지 마십시오.
Unity 에디터가 열려 있어도 무방합니다.

### 멈춰야 할 때와 계속해야 할 때

- **멈춰야 하는 경우**: 이 계획서에 없는 파일을 고쳐야 할 때, 지시가 서로 모순될 때
- **멈추지 말 것**: 환경 상태 확인, 도구가 특정 명령을 거부할 때(그 항목만 건너뛰고 계속)

## 1. 목표 (Goal)

`SO_CombatConfig` 등 설정 에셋을 인스펙터에서 열면 숫자만 나열되어 있어, 어떤 값이 어떤 동작을
바꾸는지 알 수 없다. 밸런스 수치를 플레이 중에 조정하며 체감하기로 한 [ADR-0005](../docs/decisions/ADR-0005-프로젝트-코드-구조.md)의
의도가 실현되지 않는 상태다. 설정 클래스 3종의 모든 `[SerializeField]` 필드에 `[Header]` 로
그룹을, `[Tooltip]` 으로 한국어 설명을, 범위가 정해진 값에는 `[Range]` 슬라이더를 붙여
**인스펙터만 보고도 무엇을 만질지 판단할 수 있게** 한다.

## 2. 현재 구조 분석 (Current State)

- 대상 3파일은 모두 `namespace BoomPG.Gameplay.Config` 에 있고 `ScriptableObject` 를 상속한다.
- 모든 필드가 `[SerializeField] private` 이며 읽기 전용 프로퍼티로 노출된다. **이 구조를 유지한다.**
- 각 프로퍼티에는 이미 `///` XML 주석이 달려 있다. **그대로 둔다** — `///` 는 코드를 읽는
  사람을 위한 것이고 `[Tooltip]` 은 인스펙터에서 값을 만지는 사람을 위한 것이라 역할이 다르다.
- `CombatConfig.cs` 에는 `SplashBand` 구조체와 `KnockbackBlendMode` 열거형이 같은 파일에 있다.

### 지켜야 할 컨벤션

- 속성 순서: `[Header]` → `[Tooltip]` → `[Range]` → `[SerializeField]`
- `[Header]` 는 그룹의 **첫 필드에만** 붙인다
- 툴팁은 한국어. 단위(`m`, `m/s`, `초`, `HP`, `배율`, `비율`)를 반드시 포함한다
- C# 파일 UTF-8(BOM 없음), 들여쓰기 공백 4칸

## 3. 영향 범위 (Files & Architecture)

| 파일 | 변경 유형 |
|---|---|
| `Assets/_Project/Scripts/Gameplay/Config/CombatConfig.cs` | 수정 — 속성 추가 |
| `Assets/_Project/Scripts/Gameplay/Config/MovementConfig.cs` | 수정 — 속성 추가 |
| `Assets/_Project/Scripts/Gameplay/Config/JetpackConfig.cs` | 수정 — 속성 추가 |

다른 파일은 건드리지 않는다. `.asset` 파일도 수정하지 않는다
(속성 추가는 직렬화 데이터에 영향을 주지 않는다).

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — `CombatConfig.cs`

- 대상: `Assets/_Project/Scripts/Gameplay/Config/CombatConfig.cs`

**1-1. `SplashBand` 구조체의 3개 필드에 `[Tooltip]` 을 붙인다.** `[Header]` 는 붙이지 않는다.

| 필드 | Tooltip |
|---|---|
| `_maxDistance` | `이 구간이 적용되는 폭심으로부터의 상한 거리 (m). 거리가 이 값 이하면 아래 피해·넉백이 적용된다. 배열은 반드시 오름차순이어야 한다.` |
| `_damage` | `이 구간의 피해량 (HP). 플레이어 체력은 100이다.` |
| `_knockbackSpeed` | `이 구간의 넉백 속도 (m/s). 폭심에 가까울수록 커야 '발밑 조준'이 의미를 갖는다.` |

**1-2. `CombatConfig` 필드에 속성을 붙인다. 필드 순서는 현재 그대로 두고**, 아래 표의 그룹
첫 필드에만 `[Header]` 를 붙인다.

| 필드 | Header | Tooltip | Range |
|---|---|---|---|
| `_rocketSpeed` | `로켓` | `로켓의 초기 비행 속도 (m/s). 낮을수록 예측 조준이 필요해져 실력 차이가 드러난다. 높이면 맞히기 쉬워지는 대신 회피가 어려워진다.` | — |
| `_rocketGravityScale` | | `로켓에 적용되는 중력 배율. 0이면 직선으로 날아가고, 높일수록 포물선이 가팔라져 원거리 조준이 어려워진다.` | `[Range(0f, 2f)]` |
| `_reloadSeconds` | | `재장전 시간 (초). 줄이면 교전 템포가 빨라지지만 한 발의 무게감과 빗나갔을 때의 처벌이 약해진다.` | — |
| `_explosionRadius` | `폭발 · 직격` | `폭발 판정 반경 (m). 이 안에 있는 대상이 스플래시를 받는다. 넓히면 빗나가도 효과가 닿아 쉬워지고, 좁히면 정확도 요구가 올라간다.` | — |
| `_directHitDamage` | | `로켓이 몸에 직접 맞았을 때의 피해량 (HP). 체력 100 기준 35면 3방 처치다. 40으로 올리면 스플래시 한 번을 섞어도 처치가 되어 템포가 빨라진다.` | — |
| `_directHitKnockback` | | `직격 시 넉백 속도 (m/s). 근접 스플래시(18)보다 낮게 두어야 '죽일까(직격) vs 떨어뜨릴까(발밑)'의 선택이 성립한다.` | — |
| `_splashBands` | `스플래시 구간 (거리 오름차순)` | `폭심 거리에 따른 피해·넉백 표. 위에서부터 순서대로 검사해 거리가 상한 이하인 첫 구간을 적용한다. 반드시 거리 오름차순으로 유지할 것.` | — |
| `_knockbackUpMin` | `넉백 방향` | `넉백 방향의 y 성분 하한 (비율). 항상 살짝 위로 띄워 미끄러지듯 밀리게 한다. 높이면 위로 크게 떠서 공중 체류가 길어지고, 0이면 지면을 따라 밀린다.` | `[Range(0f, 1f)]` |
| `_selfDamageRatio` | `로켓 점프 (자가 피해)` | `자기 폭발에 받는 피해 비율. 0.5면 스플래시 피해의 절반을 받는다. 로켓 점프를 남발하면 올려서 억제한다.` | `[Range(0f, 1f)]` |
| `_selfKnockbackRatio` | | `자기 폭발에 받는 넉백 비율. 1.0이면 남에게 주는 것과 같은 힘으로 자신이 밀린다. 올리면 로켓 점프 도달 거리가 늘어난다.` | `[Range(0f, 2f)]` |
| `_blendMode` | `넉백 합성 (D-022)` | `폭발이 겹쳤을 때 기존 넉백과 새 넉백을 합치는 규칙. Additive=합산 후 상한 제한(반대 방향은 상쇄), KeepStronger=더 강한 쪽만 유지, AdditiveDamped=기존을 절반으로 줄인 뒤 합산. 대입 방식이 아니어야 약한 폭발이 강한 넉백을 지우지 않는다.` | — |
| `_maxKnockbackSpeed` | | `합성된 넉백 속도의 상한 (m/s). 낮추면 연쇄 폭발로 과도하게 날아가는 것을 막지만 극적인 장면도 줄어든다. 단일 최대 넉백은 18 m/s다.` | — |

- 완료 기준: 12개 필드 전부에 `[Tooltip]` 이 있고, 표의 6개 `[Header]` 와 4개 `[Range]` 가
  지정한 필드에 붙어 있다. 필드 순서와 기본값이 변경되지 않았다.

### Step 2 — `MovementConfig.cs`

- 대상: `Assets/_Project/Scripts/Gameplay/Config/MovementConfig.cs`
- 필드 순서를 그대로 두고 아래 속성을 붙인다.

| 필드 | Header | Tooltip | Range |
|---|---|---|---|
| `_moveSpeed` | `이동` | `지상에서의 최대 이동 속도 (m/s).` | — |
| `_jumpHeight` | | `점프로 도달하는 최고 높이 (m). 초기 속도는 이 값과 중력으로 계산된다.` | — |
| `_airControl` | | `공중에서의 조작력 비율. 지상 대비 이 비율만큼만 방향을 바꿀 수 있다. 높이면 넉백당한 뒤 스스로 복귀하기 쉬워져 낙사가 줄어든다 — 이 게임의 핵심 재미(P1)에 직접 영향을 준다.` | `[Range(0f, 1f)]` |
| `_gravity` | `물리 · 감쇠` | `중력 가속도 (m/s²). 음수다. 절댓값을 키우면 낙하가 빨라져 제트팩으로 복귀할 여유가 줄어든다.` | — |
| `_groundDrag` | | `지면에 붙어 있을 때 넉백 속도가 줄어드는 비율 (m/s per s). 높이면 밀려나도 금방 멈춘다.` | — |
| `_airDrag` | | `공중에서 넉백 속도가 줄어드는 비율 (m/s per s). 낮을수록 오래 날아가지만 그만큼 조작 불가 시간이 길어진다. 18 m/s 넉백은 이 값이 2.5면 0이 되기까지 약 7초가 걸린다.` | — |
| `_capsuleRadius` | `캐릭터 캡슐` | `충돌 캡슐의 반지름 (m). 바꾸면 CharacterController 컴포넌트 설정과 함께 맞춰야 한다.` | — |
| `_capsuleHeight` | | `충돌 캡슐의 높이 (m). 바꾸면 CharacterController 컴포넌트 설정과 함께 맞춰야 한다.` | — |

- 완료 기준: 8개 필드 전부에 `[Tooltip]`, 3개 `[Header]`, 1개 `[Range]` 가 붙어 있다.

### Step 3 — `JetpackConfig.cs`

- 대상: `Assets/_Project/Scripts/Gameplay/Config/JetpackConfig.cs`
- 필드 순서를 그대로 두고 아래 속성을 붙인다.

| 필드 | Header | Tooltip | Range |
|---|---|---|---|
| `_maxFuel` | `연료` | `연료 최대치. 아래 소모 속도와 함께 총 비행 시간을 결정한다 (100 / 25 = 4초).` | — |
| `_consumePerSecond` | | `추진 중 초당 연료 소모량. 키우면 비행 시간이 짧아져 '지금 쓸까 아껴둘까'의 압박이 커진다.` | — |
| `_ascendSpeed` | `비행 성능` | `추진 중 상승 속도 (m/s). 낙하 속도를 이겨야 복귀가 가능하다.` | — |
| `_horizontalSpeed` | | `추진 중 공중 수평 이동 속도 (m/s). 비행 시간과 곱해 도달 거리가 나온다 (4 m/s × 4초 = 16 m). 맵의 발판 간격이 이 거리 안에 들어와야 복귀할 수 있다.` | — |
| `_refillDelay` | `회복` | `착지 후 연료 회복이 시작되기까지의 대기 시간 (초). 늘리면 계속 도망 다니는 플레이가 억제된다.` | — |
| `_refillPerSecond` | | `초당 연료 회복량. 완충까지 걸리는 시간을 결정한다.` | — |
| `_hitLockSeconds` | `피격 잠금` | `피격 직후 제트팩을 쓸 수 없는 시간 (초). 넉백당한 직후 곧바로 복귀하지 못하게 만드는 '떨어지는 공포'의 길이다. 줄이면 낙사가 크게 줄어든다.` | — |

- 완료 기준: 7개 필드 전부에 `[Tooltip]`, 4개 `[Header]` 가 붙어 있다. `[Range]` 는 없다.

### Step 4 — 컴파일 확인

- `mcp__unity__*` 툴이 보이면 에셋 새로고침 후 **컴파일 에러가 없는지 확인**하고 실제 출력을 보고한다.
- 툴이 없으면 "컴파일 확인 불가"라고 보고에 적는다. 배치 모드를 시도하지 않는다
  (이 환경에서는 라이선스 오류로 실패한다).
- EditMode 테스트는 이번 변경과 무관하므로 다시 돌리지 않아도 된다.

## 5. 인수 조건 (Acceptance Criteria)

- [ ] `CombatConfig.cs` — `SplashBand` 3개 + 본체 12개, 총 15개 필드에 `[Tooltip]` 이 있다
- [ ] `MovementConfig.cs` — 8개 필드 전부에 `[Tooltip]` 이 있다
- [ ] `JetpackConfig.cs` — 7개 필드 전부에 `[Tooltip]` 이 있다
- [ ] `[Header]` 가 계획서 표대로 13개 붙어 있다 (Combat 6 · Movement 3 · Jetpack 4)
- [ ] `[Range]` 가 5개 붙어 있다 (`_rocketGravityScale`, `_knockbackUpMin`, `_selfDamageRatio`,
      `_selfKnockbackRatio`, `_airControl`)
- [ ] 속성 순서가 `[Header]` → `[Tooltip]` → `[Range]` → `[SerializeField]` 이다
- [ ] **필드 순서가 바뀌지 않았다**
- [ ] **기본값이 바뀌지 않았다** (`= 28f`, `= 0.35f` 등 전부 그대로)
- [ ] 프로퍼티와 `///` XML 주석이 삭제·변경되지 않았다
- [ ] 컴파일 에러가 없다
- [ ] `git status --short` 에 위 3개 파일 외의 변경이 없다

## 6. 테스트 계획 (Test Plan)

| 확인 | 기대 결과 |
|---|---|
| Unity MCP 로 에셋 새로고침 + 컴파일 확인 | 에러 0건 |
| `git diff --stat` | `Config/` 아래 3개 파일만 변경 |
| `git diff` 에서 `- ` 로 시작하는 줄 | 기존 `[SerializeField]` 줄이 속성과 함께 다시 쓰인 것 외에 삭제된 코드가 없다 |

## 7. 범위 밖 (Out of Scope)

- **필드 추가·삭제·이름 변경·순서 변경** — 속성만 붙인다
- **기본값 변경** — 밸런스는 사람이 플레이하며 조정 중이다. 코드 기본값을 건드리지 마라
- `Assets/_Project/Data/*.asset` 파일 — 직렬화 데이터는 손대지 않는다
- 다른 `MonoBehaviour` 의 인스펙터 필드 (`PlayerMotor` 등) — 이번 범위가 아니다.
  같은 규칙을 적용할 가치가 있지만 별도 작업으로 분리한다
- `docs/`, `TaskPlan/`, `scripts/`, `.claude/` 아래 전부
- git 커밋 · 브랜치 조작 — 커밋은 사람이 한다
