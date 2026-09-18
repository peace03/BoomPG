# 스파크 (Spark) — 생성 프롬프트

> 작성 규칙은 [image-prompt-guide.md](../image-prompt-guide.md).
> 캐릭터 설정의 정본은 [worldbuilding 3.1](../../design/worldbuilding.md).
> **프롬프트를 고치면 아래 개정 이력에 무엇을 왜 바꿨는지 한 줄 남기십시오.**

## 캐릭터 원화 프롬프트 (v13 — 전신 기준 그림 · 4시점)

**이것이 스파크의 "정답 그림"입니다.** 파츠를 따로 생성·보정하는 공정에서
[blender-parts-workflow](../blender-parts-workflow.md)가 비교 대상으로 삼는 원화가
여기서 나옵니다. 아래 파츠 프롬프트들은 **이 그림을 조각낸 것**이지 별개의 디자인이
아닙니다. 설정의 정본은 [worldbuilding 3.0~3.1](../../design/worldbuilding.md),
불변 실루엣은 [ADR-0006](../../decisions/ADR-0006-캐릭터-실루엣-불변.md)입니다.

### A. 정면 (기본)

```
FEMALE speed-runner game character, full body front view. Lean athletic build, inverted-triangle torso, narrow waist, long legs. A shield-shaped booster shell caps the back of each calf, roughly twice the thickness of the bare lower leg, its outer edge clearly breaking the leg outline on both sides. A compact backpack jetpack sits high on the upper back, its top edge visible above the shoulder line. Matte BLACK skintight bodysuit with angular armor plates on the chest, shoulders, hips and thighs, and chunky armored boots. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. Wide black wraparound visor goggles cover the whole eye area, with a YELLOW strap. Long thick RED ponytail tied high on the head with YELLOW streaks, one solid connected volume, no loose strands. COLOR: black and dark charcoal cover almost the whole figure and the silhouette reads DARK IN VALUE; vivid RED appears only as glowing rings around the nozzles, vivid YELLOW only as thin stripes along a few panel edges plus a small triangle emblem on the chest; the jetpack stays NEUTRAL DARK GREY with off-white panels. Empty open hands, five clearly separated fingers. T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

### B. 후면 (같은 시드·같은 캐릭터로 이어서 생성)

```
The SAME FEMALE speed-runner game character seen from directly BEHIND, full body rear view, T-pose, both arms straight out horizontally at equal height. Matte BLACK skintight bodysuit with angular armor plates and chunky armored boots. A compact jetpack worn high on the upper back with a harness frame, shoulder straps and two thruster nozzles angled downward and backward. A large shield-shaped booster shell caps the back of each calf, layered over two smaller plates, with a nozzle at its lower end angled downward and backward. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. Long thick RED ponytail with YELLOW streaks hanging down the back, one solid connected volume, no loose strands. A YELLOW goggle strap crosses the back of the head. COLOR: black and dark charcoal cover almost the whole figure and the silhouette reads DARK IN VALUE; RED glowing rings inside every nozzle, thin YELLOW stripes along a few panel edges, jetpack NEUTRAL DARK GREY with off-white panels. The back carries no logo, no text and no extra pattern beyond these. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet.
```

### C. 좌전방 3/4 (앞대각 · 캐릭터의 왼쪽 앞에서)

```
The SAME FEMALE speed-runner game character in a FRONT THREE-QUARTER view, the body turned about 45 degrees so that the character's LEFT side and the front are both visible at once, full body, T-pose, both arms straight out horizontally at equal height. Lean athletic build, inverted-triangle torso, narrow waist, long legs. Matte BLACK skintight bodysuit with angular armor plates on the chest, shoulders, hips and thighs, and chunky armored boots. The shield-shaped booster shell on the back of the left calf is seen edge-on, its outer edge clearly standing out past the leg outline. The compact jetpack on the upper back shows its left flank, a shoulder strap and one thruster nozzle angled downward and backward. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. Wide black wraparound visor goggles cover the whole eye area, the YELLOW strap running back past the left temple. Long thick RED ponytail tied high with YELLOW streaks, one solid connected volume, no loose strands. COLOR: black and dark charcoal cover almost the whole figure and the silhouette reads DARK IN VALUE; vivid RED appears only as glowing rings around the nozzles, vivid YELLOW only as thin stripes along a few panel edges plus a small triangle emblem on the chest; the jetpack stays NEUTRAL DARK GREY with off-white panels. Empty open hands, five clearly separated fingers. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

### D. 우후방 3/4 (뒤대각 · 캐릭터의 오른쪽 뒤에서)

```
The SAME FEMALE speed-runner game character in a REAR THREE-QUARTER view, the body turned about 45 degrees away so that the character's RIGHT side and the back are both visible at once, full body, T-pose, both arms straight out horizontally at equal height. Matte BLACK skintight bodysuit with angular armor plates and chunky armored boots. The compact jetpack sits high on the upper back, showing its right flank, the harness frame, the shoulder straps and both thruster nozzles angled downward and backward. The shield-shaped booster shell on the back of the right calf is shown in full, layered over two smaller armor plates, with its nozzle at the lower end angled downward and backward. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. Long thick RED ponytail with YELLOW streaks hanging down the back, one solid connected volume, no loose strands. A YELLOW goggle strap crosses the back of the head. COLOR: black and dark charcoal cover almost the whole figure and the silhouette reads DARK IN VALUE; RED glowing rings inside every nozzle, thin YELLOW stripes along a few panel edges, jetpack NEUTRAL DARK GREY with off-white panels. The back carries no logo, no text and no extra pattern beyond these. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers.
```

> **좌우는 캐릭터 기준입니다.** "왼쪽"은 캐릭터 자신의 왼쪽(정면 원화에서 화면 오른쪽)입니다.
> 프롬프트에 `the character's LEFT side` / `the character's RIGHT side` 로 못박아 두었으니
> 관찰자 기준으로 뒤집어 해석하지 마십시오. 분석표에 방향을 적을 때도 같은 기준을 씁니다.

> **네 장이면 충분한 이유** — 스파크는 좌우 대칭입니다
> (비대칭은 기즈모의 오른팔 건틀릿뿐, [ADR-0006](../../decisions/ADR-0006-캐릭터-실루엣-불변.md)).
> A(정면) · B(후면) · C(좌전) · D(우후) 로 **네 사분면이 모두 한 번씩 덮이고**,
> 빠진 우전·좌후는 C·D 를 좌우 반전해 읽으면 됩니다. 대칭이 아닌 캐릭터라면
> 여섯 장이 필요합니다.

| 구절 | 왜 있는가 |
|---|---|
| `turned about 45 degrees` | "three-quarter view" 만 쓰면 모델이 10°~70° 사이를 멋대로 고릅니다. 각도를 숫자로 박아 네 장의 시점이 일정하게 나오게 합니다 |
| `seen edge-on, its outer edge clearly standing out past the leg outline` | 대각에서는 종아리 셸이 **두께**로 읽힙니다. 정면 원화가 못 주는 정보이자 보정에서 가장 자주 틀리는 값입니다 |
| `showing its left flank` / `showing its right flank` | 제트팩의 측면 두께와 노즐 각도를 확정합니다. 정면·후면만으로는 **앞뒤 두께가 미지수**로 남습니다 |
| C 는 정면 색 블록, D 는 후면 색 블록을 그대로 | 네 장이 같은 색 서술을 공유해야 파츠 텍스처가 시점마다 달라지지 않습니다 |

---

| 구절 | 왜 있는가 |
|---|---|
| `inverted-triangle torso, narrow waist` | [ADR-0006](../../decisions/ADR-0006-캐릭터-실루엣-불변.md)의 불변 실루엣. 스킨이 못 바꾸는 요소라 원화에 못박습니다 |
| `roughly twice the thickness of the bare lower leg, its outer edge clearly breaking the leg outline` | **실루엣 요소는 전용 문장 + 배수 지정**([image-prompt-guide 5](../image-prompt-guide.md)). "돌출"이라고만 쓰면 표면 장식으로 그려집니다 |
| `its top edge visible above the shoulder line` | 제트팩은 정면에서 가려집니다. 이 구절이 없으면 정면 원화에 **아예 안 나타나** 뒤에서 튀어나온 것처럼 보입니다 |
| `the jetpack stays NEUTRAL DARK GREY` | 세 캐릭터 공용 메시입니다([worldbuilding 3.0](../../design/worldbuilding.md)). 원화에서 빨강·노랑으로 칠해 두면 파츠 보정이 그 색을 따라갑니다 |
| `the silhouette reads DARK IN VALUE` | 로카와 색 계열이 가까워 **명도로 가릅니다**([worldbuilding 4.3](../../design/worldbuilding.md)) |
| `The back carries no logo, no text and no extra pattern` | 후면은 생성 모델이 가장 많이 지어내는 부분입니다. 보정 공정이 "원화에 없는 장식"을 필수 요소로 착각하지 않게 막습니다 |

> **정면 한 장으로 끝내지 마십시오.** 스파크의 고유 요소 두 개(종아리 부스터·제트팩)가
> 모두 뒤에 있습니다. 보정 워크플로우는 8방향을 확인하는데, 원화가 정면뿐이면
> 뒤·안쪽 구조가 전부 "추정"으로 떨어집니다([blender-parts-workflow 2](../blender-parts-workflow.md)).
>
> **네 프롬프트는 각각 200 단어 안팎으로 권장 상한(80)을 크게 넘습니다.** 의도한
> 것입니다 — 원화는 `Generate3D` 의 입력이 아니라 **사람과 Codex 가 보는 합격 기준**이라
> 단어 예산보다 정보 완전성이 우선입니다. 3D 에 들어가는 것은 아래 4 파츠 프롬프트입니다.
> 다만 모델이 뒷부분을 흘릴 수 있으므로 **결과에서 배경·T포즈부터 확인**하십시오.

> **[미결정 D-031]** 원화를 어느 모델로 뽑을지 정하지 않았습니다. `GenerateImage`
> (`gpt-image-2-medium`)로 계속 갈지, 원화만 외부 툴에서 뽑아 `upload_image` 로 넣을지는
> 1차 결과를 보고 판단합니다.

---

## 통짜 프롬프트 (v4 — 채택 이미지 기반 · **구식**)

> **v8~v11 의 개정이 반영되어 있지 않습니다.** 어깨·무릎 포드 시절의 글이라
> 종아리 부스터·제트팩·하드서피스 언어가 빠져 있습니다. 전신 한 장이 필요하면
> 위의 **원화 프롬프트(v13 · 4시점)** 를 쓰십시오. 이 절은 개정 경위를 남기기 위한 기록입니다.

**2026-09-16 에 사용자가 제시한 참고 이미지가 기준입니다.** 아래 프롬프트는 그 이미지를
역으로 분석해 쓴 것입니다.

```
FEMALE speed-runner game character, lean athletic build, narrow waist, long legs. Matte BLACK skintight bodysuit with sharp geometric panel seams, a high collar and black gloves. Cylindrical thruster pods mounted on top of both shoulders and angular thruster pods on both knees, RED glowing rings set in YELLOW housings, clearly protruding from the silhouette. Wide black visor goggles covering the eyes, with a yellow strap. Long thick RED ponytail tied high on the head with YELLOW streaks, one solid volume, no loose strands. Chunky black boots with yellow V-shaped marks and red heels. COLOR: black covers almost the entire figure; vivid RED and YELLOW appear only as sharp V-shaped accents on the thighs, hips and collarbone, plus a small yellow triangle emblem on the chest. Clean defined facial features. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, clean readable silhouette, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers.
```

## 파츠 분할 생성 (v6 — 2026-09-16 채택)

**파츠마다 이미지와 3D 를 따로 만들고 블렌더/Unity 에서 조립합니다.**
구조와 폴리곤 예산은 [asset-pipeline 2.2.1.2](../asset-pipeline.md).
위의 v4 통짜 프롬프트는 **파츠 방식 실패 시의 대비책**으로 남겨 둡니다.

> **2026-09-17 — 파츠 분할을 접고 통 슈트로 갑니다.** 아래 "베이스 바디 / 의상"
> 구분은 폐기되었고, **몸 · 아머 · 부츠 · 종아리 부스터가 한 덩어리**입니다
> ([ADR-0008 개정 이력](../../decisions/ADR-0008-모듈러-캐릭터-구조.md)).
> 분리되는 것은 **헤어 · 고글 · 제트팩** 뿐입니다.

### 슈트 프롬프트 (통합 · **리깅 대상**)

```
FEMALE speed-runner game character in a full black tech suit. COMPLETELY BALD with no hair, no goggles, no helmet, no backpack, face fully uncovered. Lean athletic build, narrow waist. Matte black skintight bodysuit with angular armor plates on the chest, shoulders, hips and thighs; a large shield-shaped booster shell on the back of each calf with a RED glowing nozzle pointing down and back; chunky armored boots. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. Black covers almost the whole figure; thin YELLOW accent stripes along a few panel edges and a small yellow triangle emblem on the chest. The upper back is left smooth and flat with nothing attached. Detailed face: large expressive eyes, clearly defined nose and lips, neutral expression. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

| 구절 | 왜 있는가 |
|---|---|
| `no hair, no goggles, no helmet, no backpack` | 분리 유지되는 세 파츠가 **겹치지 않게** 비웁니다 |
| `The upper back is left smooth and flat with nothing attached` | 제트팩 자리. 등에 뭔가 생기면 부착물과 충돌합니다 |
| `a large shield-shaped booster shell on the back of each calf` | 참고 이미지(건프라 다리)의 형태. **슈트에 붙박이로 들어갑니다** |
| `HARD-SURFACE MECHA STYLE: ...` | 아래 공통 디자인 언어와 같은 구절 |

> **이 프롬프트는 약 120 단어로 권장 상한(80)을 넘습니다.** 파츠를 통합한 만큼
> 정보량이 늘어난 것이라 불가피합니다. 대신 **중요한 것을 앞에 배치**했습니다 —
> 성별·체형 → 비워둘 것 → 의상·부스터·부츠 → 디자인 언어 → 색 → 얼굴·손 → 촬영.
> 뒷부분이 흘려질 경우 배경·양식 지시부터 무시되므로, 결과에서 그쪽을 먼저 보십시오.

---

<details>
<summary>폐기 — 베이스 바디 프롬프트 (민머리 · 언더수트) · 참고용 보존</summary>

### 베이스 바디 프롬프트 (민머리 · 언더수트 · **리깅 대상**)

```
FEMALE speed-runner game character base body, COMPLETELY BALD with no hair at all, no goggles, no helmet, face fully uncovered and clearly visible. Lean athletic build, narrow waist. Wearing ONLY a PLAIN MATTE BLACK skintight undersuit covering the body from neck to ankles, completely smooth with no panels, no seams, no decoration, no armor, no boots. Bare feet. Plain black gloves on the hands. The waist belt line and the backs of the calves are left smooth and flat with no attachments, no boosters and no bulges. Shoulders and knees are also smooth with no pods. Detailed face: large expressive eyes, clearly defined nose and lips, smooth clean skin, neutral expression. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

| 구절 | 왜 있는가 |
|---|---|
| `COMPLETELY BALD ... no goggles, no helmet` | 헤어·고글이 따로 오므로 몸체에 있으면 **겹칩니다** |
| `Wearing ONLY a PLAIN MATTE BLACK skintight undersuit ... no panels, no seams, no decoration` | **스킨이 바뀌어도 남는 레이어**입니다 ([ADR-0008](../../decisions/ADR-0008-모듈러-캐릭터-구조.md)). 장식이 있으면 아머 밑으로 비쳐 스킨 교체가 어색해집니다 |
| `Bare feet` | 부츠가 따로 옵니다 |
| `Plain black gloves` | 장갑은 **베이스에 포함**합니다. 손은 관절이 많아 따로 바인딩하기 까다롭습니다 |
| `The waist belt line and the backs of the calves are left smooth and flat ... Shoulders and knees are also smooth with no pods` | 부스터가 붙을 자리를 **평평하게 비웁니다.** 어깨·무릎도 함께 비워 예전 위치에 포드가 딸려 나오지 않게 합니다 |
| `Detailed face: large expressive eyes...` | 고글이 없어 얼굴이 드러나므로 **여기서 얼굴을 확정**합니다 |

</details>

---

### 공통 디자인 언어 — 하드서피스 메카 (2026-09-17 채택)

**의상·장비 파츠 전체가 같은 한 구절을 공유합니다.** 파츠를 따로 생성하면
디자인이 제각각으로 흩어지는데, 이 구절이 그걸 묶습니다.

```
HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts.
```

| 요소 | 왜 이렇게 쓰는가 |
|---|---|
| `large flat plates stacked in two or three layers` | 겹층이 메카 느낌의 핵심입니다. **층 수를 숫자로 제한**해 복잡해지는 것을 막습니다 |
| `crisp beveled edges` | 모서리가 살아야 카툰 렌더에서 면이 갈립니다 |
| `clear panel gaps between the layers` | 판 사이의 **틈**이 디테일을 대신합니다. 작은 돌기보다 3D 로 잘 살아남습니다 |
| `no small greebles, no pipes, no bolts` | **이 게임이 버리는 것.** 교전 거리 20~40 m 에서 안 보이고, 3D 생성에서 가장 먼저 뭉개집니다 |

> **참고 이미지는 건프라 다리 파츠였습니다.** 형태 언어(겹층 장갑판, 종아리 뒤 대형 셸,
> 두꺼운 아머 부츠)만 가져오고 **색은 스파크 것을 씁니다** — 참고는 흰색 위주지만
> 스파크는 검정 베이스 + 빨강·노랑 하이라이트입니다.
>
> 앞서 검토했던 아이언맨형 레퍼런스보다 이쪽이 **3D 생성에 훨씬 유리합니다.**
> 면이 크고 잡동사니가 적어 생성 모델이 덩어리를 잡기 쉽습니다.

<details>
<summary>폐기 — 의상 프롬프트(아머 세트 · 부츠) · 참고용 보존</summary>

### 의상 프롬프트 (~~스킨 교체 대상~~ — 슈트에 흡수됨)

**아머 세트**
```
A wearable armor set for a female game character, shown as an isolated hollow shell with NO BODY and NO PERSON inside, like armor on an invisible mannequin. Angular chest plate, shoulder caps, hip plates and thigh guards. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. COLOR: MATTE BLACK and dark charcoal plates, thin YELLOW accent stripes along a few panel edges, small RED markings, plus a small yellow triangle emblem on the chest. Cartoon 3D game art, front view, plain light gray background, no shadows, whole object centered and fully visible.
```

**부츠** (왼쪽 1개 생성 → 미러링)
```
A single armored boot for the left foot of a game character, shown as one isolated object with no leg, no foot and no body. A chunky armored high-top boot with a thick blocky sole and an ankle guard plate over the shin opening. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no laces, no bolts. COLOR: MATTE BLACK and dark charcoal plates, a YELLOW V-shaped mark on the front and a RED heel. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, object centered and fully visible.
```

> **`isolated hollow shell with NO BODY and NO PERSON inside, like clothing on an invisible mannequin`**
> — 이 구절이 의상 파츠의 핵심입니다. 사람이 함께 그려지면 3D 에 몸이 딸려 나와
> 베이스 바디와 겹칩니다.

</details>

---

### 장비 프롬프트 (**분리 유지**)

**헤어**
```
A single hairstyle asset for a game character, shown as an isolated object with no head, no face and no body. Long thick RED ponytail tied high, with bright YELLOW streaks running through it, plus swept side bangs. One solid connected volume, smooth clean surface, no thin flyaway strands, no individual hair fibers. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, whole object centered and fully visible.
```

**고글**
```
A single pair of sport visor goggles for a game character, shown as an isolated object with no head and no face. Wide wraparound black lens covering the whole eye area, thick YELLOW strap band, simple blocky frame. Three or four large smooth surfaces, clean edges, no small greebles. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, whole object centered and fully visible.
```

**제트팩** (**세 캐릭터 공용 파츠** — 색 중립)
```
A single jetpack backpack unit for a game character, shown as one isolated object with no body and no person. A compact backpack worn high on the upper back, with two thruster nozzles angled downward and backward, a harness frame and shoulder straps. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. COLOR: NEUTRAL DARK GREY body with off-white panels and pale glowing nozzle rings, no strong accent color. Cartoon 3D game art, three-quarter rear view, plain light gray background, no shadows, object centered and fully visible.
```

> **색을 중립으로 고정하는 것이 이 파츠의 핵심입니다.** 세 캐릭터가 같은 메시를
> 공유하므로 스파크의 빨강·노랑을 구우면 로카·기즈모에 못 씁니다. 캐릭터색은
> Unity 머티리얼에서 입힙니다 — 카툰 렌더라 단색 틴트가 잘 먹습니다.

**종아리 부스터** (1종 생성 → 좌우 복제)
```
A single calf armor booster unit for a game character, shown as one isolated object with no leg, no foot and no body. A large rounded shield-shaped shell panel that caps the back of the calf, layered over two smaller armor plates beneath it, with a thruster nozzle emerging at the lower end angled downward and backward. HARD-SURFACE MECHA STYLE: large flat plates stacked in two or three layers, crisp beveled edges, clear panel gaps between the layers, no small greebles, no pipes, no bolts. COLOR: MATTE BLACK and dark charcoal plates, a RED glowing ring around the nozzle, thin YELLOW accent stripes along a few panel edges. Cartoon 3D game art, three-quarter rear view, plain light gray background, no shadows, object centered and fully visible.
```

> **노즐이 아래·뒤를 향합니다.** 제트팩은 상승과 전진, 종아리 부스터는
> 달리기와 대시(`Dash`)의 가속을 담당합니다. 어깨 포드였을 때는 추진 방향이
> 애매했습니다.
>
> **허리춤 부스터는 폐기했습니다** (2026-09-17) — 등 제트팩과 기능이 겹칩니다.

> **부착물 이미지는 `one isolated object with no body` 로 고정합니다.**
> 사람이나 팔다리가 함께 그려지면 3D 에 그 조각이 딸려 나옵니다.
>
> **어깨와 무릎 포드를 한 이미지에 담지 않습니다.** 둘이 한 메시로 붙어 나오면
> 블렌더에서 갈라내야 합니다. 따로 뽑는 편이 그 작업을 없앱니다.

### 파츠 목록과 조립 (4종 — 2026-09-17 개정)

| 레이어 | 파츠 | 폴리곤 | 개수 | 조립 방법 |
|---|---|---|---|---|
| 슈트 | **슈트** (몸 + 아머 + 부츠 + 종아리 부스터) | 10,000 | 1 | **`Rig` 로 Humanoid 리깅.** 나머지가 여기 붙는다 |
| 장비 | 헤어 | 2,000 | 1 | `Head` 본에 강체 부착 |
| 장비 | 고글 | 1,200 | 1 | `Head` 본에 강체 부착 |
| 장비 | **제트팩** (공용) | 1,500 | 1 | `Spine2` 또는 `Chest` |

**합계 약 14,700 tris 로 규격(15,000) 안에 들어옵니다.** 7 파츠 구성일 때는
21,700 으로 초과해 `Remesh` 로 깎아야 했는데, 통합하면서 그 작업이 사라졌습니다.

> 이미 생성한 **헤어는 5,000 tris 로 나왔습니다.** 2,000 으로 낮추거나
> `Remesh`(0 크레딧)로 깎으십시오. 그대로 두면 합계가 17,700 이 됩니다.

### 1차 생성 결과 (2026-09-16 · 장비 4종)

| 파츠 | 판정 | 메모 |
|---|---|---|
| 헤어 | **양호** | 빨강·노랑 스트릭이 살아 있고 가닥이 분리되지 않은 깔끔한 덩어리 |
| 고글 | **양호 · 색 확인 필요** | 형태는 단순하고 좋으나 **프레임까지 노란색**으로 나왔다. 참고 이미지는 검정 프레임 + 노란 스트랩이었다 |
| ~~어깨 포드~~ | **폐기** | 형태는 양호했으나 **부스터 위치 변경(2026-09-17)으로 버림** |
| ~~무릎 포드~~ | **폐기** | 〃 |

> **파츠 분할이 통한다는 것이 이 단계에서 확인되었습니다.** 단일 오브젝트라 형태가 명확하고,
> 여러 부위의 UV 가 한 텍스처에 섞이지 않아 **"얼굴에 머리카락 텍스처" 류의 어긋남이
> 구조적으로 발생할 수 없습니다.**
>
> **다만 노란색이 과합니다.** 고글·어깨 포드·무릎 포드가 모두 노랑 위주라, 몸체가 검정이어도
> 노란 면적이 [worldbuilding](../../design/worldbuilding.md)의 "검정 베이스 + 빨강·노랑
> 하이라이트"보다 커집니다. **베이스 바디와 아머가 나온 뒤 전체 배색을 다시 보십시오.**

## 참고 이미지에서 읽어낸 것

| 요소 | 이미지의 처리 | 프롬프트 구절 |
|---|---|---|
| ~~어깨·무릎 추진기~~ | 참고 이미지에서는 어깨 위 원통형 포드와 무릎 각진 포드였다 | **2026-09-17 에 허리춤·종아리로 옮겼습니다.** 배색(빨간 발광 링 + 노란 하우징)만 그대로 가져옵니다 |
| 수트 | 무광 검정. **날카로운 기하학적 분할선**이 가슴·허벅지·종아리를 가름 | `Matte BLACK skintight bodysuit with sharp geometric panel seams` |
| 고글 | 눈을 완전히 덮는 **넓은 검정 바이저**, 노란 스트랩이 머리 뒤로 | `Wide black visor goggles covering the eyes, with a yellow strap` |
| 머리 | **길고 굵은 빨간 포니테일**을 높게 묶고 노란 가닥이 섞임 | `Long thick RED ponytail tied high on the head with YELLOW streaks` |
| 부츠 | 볼륨 있는 두꺼운 스니커즈형, 노란 V 마크, 빨간 뒤꿈치 | `Chunky black boots with yellow V-shaped marks and red heels` |
| 액센트 배치 | 노랑·빨강이 **V 자·화살표 형태**로 허벅지·골반·쇄골에만. 가슴 중앙에 작은 노란 삼각형 | `sharp V-shaped accents on the thighs, hips and collarbone, plus a small yellow triangle emblem` |
| 명도 | **검정이 전체를 덮고** 포인트만 고채도 | `black covers almost the entire figure` |

## 주의 두 가지

### 1. 추진기가 돌출형으로 되돌아왔습니다

v3 에서 사용자 요청에 따라 **매립형**(`INSET FLUSH`)으로 바꿨으나, 채택된 이미지는
**명확히 돌출된 포드**입니다. 이미지를 기준으로 삼아 v4 에서 되돌렸습니다.

결과적으로 [worldbuilding 4.3](../../design/worldbuilding.md)의 원래 기술
**"관절부 추진기 돌출"** 과 다시 일치하며, 스파크의 실루엣 구분 요소가 체형 하나로
줄어들 뻔했던 문제([D-029](../../INDEX.md))도 완화됩니다.

### 2. 긴 포니테일은 3D 생성의 위험 요소입니다

D-028 은 머리카락을 **하나의 덩어리**로 만들라고 정했고, 이 이미지는 그 조건을 지킵니다
(가닥이 흩날리지 않고 굵게 묶여 있음). 다만 **길이가 길수록 3D 변환에서 뭉개질 확률이
올라갑니다.**

- 3D 결과에서 포니테일이 머리에 들러붙거나 뭉치면, **길이를 줄이는 것이 첫 번째 수단**입니다
- 그래도 안 되면 헤어를 **별도 부착물로 분리**합니다 (기즈모 건틀릿과 같은 방식)

## 개정 이력

| 버전 | 날짜 | 변경 |
|---|---|---|
| v1 | 2026-09-16 | 최초. 산문형 서술 |
| v2 | 2026-09-16 | 사용자 제안 구조 채택 — 성별 대문자, `COLOR SCHEME:` 블록 분리, 밝은 회색 배경, `symmetrical` |
| v3 | 2026-09-16 | 길이 압축, 의상을 구조로 서술, 손·얼굴 지시 추가. 추진기를 돌출형 → **매립형**으로 변경 |
| v4 | 2026-09-16 | **채택 이미지를 역분석해 전면 재작성.** 추진기를 다시 **돌출형**으로. 고글·부츠·액센트 배치·머리 색을 이미지대로 명시 |
| v5 | 2026-09-16 | 파츠를 따로 뽑아 `reference` 로 **이미지 합성**하는 구조 (폐기) |
| v6 | 2026-09-16 | **파츠별 3D 생성으로 전환.** 이미지 합성 제거. 몸체를 민머리·고글 없음·맨얼굴로 재작성하고 붙을 자리를 평평하게 비움. 포드를 어깨/무릎 2종으로 분리 |
| v7 | 2026-09-16 | **모듈러 구조 채택([ADR-0008](../../decisions/ADR-0008-모듈러-캐릭터-구조.md)).** 몸체를 무지 언더수트 베이스 바디로 바꾸고 의상을 아머 세트·부츠로 분리. 총 7 파츠 |
| v8 | 2026-09-17 | **부스터를 어깨·무릎 → 허리춤·종아리로 이동.** 노즐을 후방으로 지정. 베이스 바디의 비워둘 자리도 함께 변경. 어깨·무릎 포드 3D 2종은 폐기 |
| v9 | 2026-09-17 | **등에 공용 제트팩 추가, 허리춤 부스터 폐기.** gdd 기본 장비인데 외형에 없던 공백을 메움. 제트팩은 색 중립으로 만들어 세 캐릭터가 공유한다 |
| v10 | 2026-09-17 | **하드서피스 메카 디자인 언어를 의상·장비 전 파츠에 공통 적용.** 종아리 부스터를 참고 이미지(건프라 다리)의 대형 셸 + 겹층 판 형태로 재작성. 아머 세트·부츠·제트팩도 같은 언어로 통일 |
| v11 | 2026-09-17 | **파츠 분할 철회 — 통 슈트로 전환.** 몸·아머·부츠·종아리 부스터를 한 덩어리로 합쳤다. 7 파츠 → **4 파츠**, 폴리곤 21,700 → **14,700(규격 충족)**, 생성 비용 1,400 → **800 크레딧**. 폐기된 프롬프트는 접어서 보존 |
| v12 | 2026-09-18 | **전신 원화 프롬프트 신설(정면·후면 2장).** 파츠 보정이 비교할 기준 그림이 v4(어깨·무릎 포드 시절)뿐이던 공백을 메움. 종아리 부스터·제트팩·하드서피스 언어·명도 방향을 반영하고, 뒤에 몰린 고유 요소를 위해 후면을 추가했다. v4 는 구식 표시 후 보존. D-031 신규 등록 |
| v13 | 2026-09-18 | **원화에 대각 2시점 추가(C 좌전방 · D 우후방).** 정면·후면만으로는 종아리 셸과 제트팩의 두께가 미지수로 남아 보정이 추정에 의존했다. 회전 각도를 45°로 못박고 좌우 기준을 캐릭터 기준으로 명시했다 |

## 다음에 시도할 것

- [ ] **참고 이미지를 직접 `ImageInput` 에 넣어 3D 생성**하는 쪽이 재생성보다 확실하다.
      이미지 파일 경로를 받으면 `upload_image` 로 올려 바로 넘길 수 있다 (240 크레딧 절약)
- [ ] 긴 포니테일이 3D 로 살아남는지 확인 (위 주의 2)
- [ ] 어깨·무릎 포드가 뭉개지지 않는지 확인
- [ ] 빨강·노랑 하이라이트가 3D 텍스처로 넘어갈 때 살아남는지 확인
