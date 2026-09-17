# 기즈모 (Gizmo) — 생성 프롬프트

> 작성 규칙은 [image-prompt-guide.md](../image-prompt-guide.md).
> 캐릭터 설정의 정본은 [worldbuilding 3.3](../../design/worldbuilding.md).
> **프롬프트를 고치면 아래 개정 이력에 무엇을 왜 바꿨는지 한 줄 남기십시오.**

## 구조 — 이 캐릭터만 두 벌로 생성합니다

**건틀릿을 본체에서 분리했습니다** (2026-09-16 결정, [D-027](../../INDEX.md) 부분 채택).

| 산출물 | 내용 | 생성 방식 |
|---|---|---|
| **본체** | 양팔이 정상인 일반 인간형 기즈모 | 아래 본체 프롬프트 → `Generate3D` → `Rig` |
| **건틀릿** | 오른팔에 덮어씌우는 갈고리 건틀릿 | 아래 건틀릿 프롬프트 → `Generate3D` (리깅 없음) |

### 왜 나누는가

1. **갈고리가 발사되어야 합니다.** gdd 의 `GrappleHook` 은 12 m 사거리로 **손이 날아가는**
   기믹입니다. 본체 메시에 붙어 있으면 분리 발사를 구현할 수 없습니다.
2. **비대칭이 3D 생성을 방해합니다.** 1차 생성에서 무거운 건틀릿 쪽 팔이 아래로 처져
   좌우 높이가 어긋났습니다. 양팔이 대칭이면 그 문제가 사라집니다.
3. **리깅이 안정됩니다.** 한쪽만 부피가 두 배인 팔은 `Rig` 가 본을 잘못 심을 위험이 큽니다.

### 조립은 Unity에서 합니다 — 블렌더가 필요 없습니다

건틀릿은 **스킨드 메시가 아니라 본에 붙는 부착물**이므로, 병합·리깅·UV 베이크가 필요 없습니다.
캐릭터 프리팹의 `RightLowerArm` 본 아래에 자식 오브젝트로 넣으면 애니메이션을 따라갑니다.
발사할 때는 그 오브젝트를 본에서 떼어내 날리면 됩니다.

> 드로우콜이 캐릭터당 1 늘어납니다. [ADR-0007](../../decisions/ADR-0007-캐릭터-에셋-반입-규격.md)의
> "캐릭터당 머티리얼 1 슬롯"은 **본체 기준으로 유지**하고, 건틀릿은 무기·장비로 셉니다.
> 기즈모만 해당하므로 8인 난전에서도 최대 +8 드로우콜입니다.

## 본체 프롬프트 (v4)

```
FEMALE tech-engineer game character, slim normal human proportions, BOTH ARMS FULLY COVERED BY LONG FITTED SLEEVES down to the wrists, no bare skin on the arms, symmetrical, with no armor or gauntlet. Layered techwear: a cropped jacket over a fitted long-sleeve top, cargo pants, a few tool pouches on the belt. Keep both forearms plain and unadorned. Hair in one compact round bun, a single solid volume, no loose strands. COLOR: mostly DARK PURPLE over large areas; vivid CYAN as a few large panels on the jacket and boots. Clean defined facial features, both eyes clearly visible. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, clean readable silhouette, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

| 구절 | 왜 있는가 |
|---|---|
| `BOTH ARMS FULLY COVERED BY LONG FITTED SLEEVES ... no bare skin on the arms` | **팔에 맨살이 드러나지 않게 합니다** (2026-09-16 결정). v3 의 `BARE` 는 "장비가 없다"는 뜻이었는데 모델이 **맨팔**로 해석했습니다 |
| `with no armor or gauntlet` | 건틀릿을 **본체에서 빼는** 지시. 이제 `BARE` 대신 이 구절이 단독으로 담당합니다 |
| `Keep both forearms plain and unadorned` | 나중에 건틀릿을 **덮어씌울 자리**입니다. 소매 장식이 복잡하면 겹칠 때 지저분해집니다 |
| `a cropped jacket over a fitted long-sleeve top, cargo pants` | 건틀릿이 빠진 만큼 **의상이 실루엣을 담당**해야 합니다 |

> **`BARE` 라는 단어가 함정이었습니다.** "장비를 걸치지 않은 팔"을 뜻하려 했지만
> 이미지 모델에게는 "맨살 팔"로 읽힙니다. **원하는 것을 긍정형으로 쓰고(긴 소매)
> 배제는 따로 적는 편**이 안전합니다 — [image-prompt-guide 1](../image-prompt-guide.md)의
> "반대쪽을 지정한다" 원칙과 같은 맥락입니다.

## 건틀릿 프롬프트 (v1 — 미검증)

```
A single mechanical grappling gauntlet, a forearm-mounted device shown as an isolated object with no arm or character inside. SIMPLE shape: three or four large smooth blocky plates, clean straight edges, no small greebles, no tiny pipes, no surface clutter. A clawed grappling hook at the front end, pointing forward. COLOR: DARK PURPLE body with a few large vivid CYAN panels. Cartoon 3D game art, clean readable silhouette, three-quarter view, plain light gray background, no shadows, whole object visible, centered.
```

| 구절 | 왜 있는가 |
|---|---|
| `isolated object with no arm or character inside` | 팔이 함께 생성되면 본체와 겹칩니다 |
| `three or four large smooth blocky plates` | **면 수를 숫자로 지정합니다.** 단순한 덩어리가 3D 생성에 유리하고, 사용자 요청(단순화)과도 일치합니다 |
| `A clawed grappling hook at the front end` | 발사되는 부분. 나중에 여기만 다시 떼어낼 수 있으면 더 좋습니다 |
| `three-quarter view` | 소품은 정면보다 3/4 시점이 입체 정보를 더 줍니다 |

> **아직 생성해 보지 않았습니다.** 본체가 확정된 뒤에 시도하고, 결과를 이 문서에 기록하십시오.
> 특히 **스케일**이 문제입니다 — 팔뚝 길이에 맞게 Unity 에서 수동 조정해야 합니다.

## 개정 이력

| 버전 | 날짜 | 변경 |
|---|---|---|
| v1 | 2026-09-16 | 최초. 건틀릿이 본체에 붙은 비대칭 단일 메시 |
| v2 | 2026-09-16 | 사용자 제안 구조 채택 — 건틀릿 크기를 정량 비교(`twice the bulk`)로, `at the same height` 추가, `symmetrical` 제외 |
| v3 | 2026-09-16 | **건틀릿을 본체에서 분리.** 본체는 양팔 대칭 일반 인간형으로 전환하고 건틀릿 프롬프트를 신설 |
| v4 | 2026-09-16 | 팔을 **긴 소매로 덮음**. `BARE` 가 맨살로 해석되던 문제를 긍정형 서술로 교체 |

## 다음에 시도할 것

- [ ] 본체에서 건틀릿이 실제로 빠지는지 확인 (모델이 설정을 기억해 그릴 위험)
- [ ] 건틀릿 단독 생성 시도 — 스케일 기준을 어떻게 잡을지
- [ ] Unity 에서 `RightLowerArm` 본에 붙였을 때 크기·각도가 맞는지
- [ ] 건틀릿이 빠진 본체가 스파크와 실루엣으로 구분되는지 (**둘 다 날씬한 여성형입니다 — 위험**)
