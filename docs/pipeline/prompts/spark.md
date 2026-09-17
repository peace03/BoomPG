# 스파크 (Spark) — 생성 프롬프트

> 작성 규칙은 [image-prompt-guide.md](../image-prompt-guide.md).
> 캐릭터 설정의 정본은 [worldbuilding 3.1](../../design/worldbuilding.md).
> **프롬프트를 고치면 아래 개정 이력에 무엇을 왜 바꿨는지 한 줄 남기십시오.**

## 현재 프롬프트 (v4 — 채택 이미지 기반)

**2026-09-16 에 사용자가 제시한 참고 이미지가 기준입니다.** 아래 프롬프트는 그 이미지를
역으로 분석해 쓴 것입니다.

```
FEMALE speed-runner game character, lean athletic build, narrow waist, long legs. Matte BLACK skintight bodysuit with sharp geometric panel seams, a high collar and black gloves. Cylindrical thruster pods mounted on top of both shoulders and angular thruster pods on both knees, RED glowing rings set in YELLOW housings, clearly protruding from the silhouette. Wide black visor goggles covering the eyes, with a yellow strap. Long thick RED ponytail tied high on the head with YELLOW streaks, one solid volume, no loose strands. Chunky black boots with yellow V-shaped marks and red heels. COLOR: black covers almost the entire figure; vivid RED and YELLOW appear only as sharp V-shaped accents on the thighs, hips and collarbone, plus a small yellow triangle emblem on the chest. Clean defined facial features. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, clean readable silhouette, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers.
```

## 파츠 분할 생성 (v6 — 2026-09-16 채택)

**파츠마다 이미지와 3D 를 따로 만들고 블렌더/Unity 에서 조립합니다.**
구조와 폴리곤 예산은 [asset-pipeline 2.2.1.2](../asset-pipeline.md).
위의 v4 통짜 프롬프트는 **파츠 방식 실패 시의 대비책**으로 남겨 둡니다.

### 베이스 바디 프롬프트 (민머리 · 언더수트 · **리깅 대상**)

```
FEMALE speed-runner game character base body, COMPLETELY BALD with no hair at all, no goggles, no helmet, face fully uncovered and clearly visible. Lean athletic build, narrow waist. Wearing ONLY a PLAIN MATTE BLACK skintight undersuit covering the body from neck to ankles, completely smooth with no panels, no seams, no decoration, no armor, no boots. Bare feet. Plain black gloves on the hands. Shoulders and knees are smooth and flat with no pods and no bulges. Detailed face: large expressive eyes, clearly defined nose and lips, smooth clean skin, neutral expression. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

| 구절 | 왜 있는가 |
|---|---|
| `COMPLETELY BALD ... no goggles, no helmet` | 헤어·고글이 따로 오므로 몸체에 있으면 **겹칩니다** |
| `Wearing ONLY a PLAIN MATTE BLACK skintight undersuit ... no panels, no seams, no decoration` | **스킨이 바뀌어도 남는 레이어**입니다 ([ADR-0008](../../decisions/ADR-0008-모듈러-캐릭터-구조.md)). 장식이 있으면 아머 밑으로 비쳐 스킨 교체가 어색해집니다 |
| `Bare feet` | 부츠가 따로 옵니다 |
| `Plain black gloves` | 장갑은 **베이스에 포함**합니다. 손은 관절이 많아 따로 바인딩하기 까다롭습니다 |
| `Shoulders and knees are smooth and flat with no pods and no bulges` | 포드가 붙을 자리를 **평평하게 비웁니다** |
| `Detailed face: large expressive eyes...` | 고글이 없어 얼굴이 드러나므로 **여기서 얼굴을 확정**합니다 |

### 의상 프롬프트 (**스킨 교체 대상**)

**아머 세트**
```
A wearable armor set for a female game character, shown as an isolated hollow shell with NO BODY and NO PERSON inside, like clothing on an invisible mannequin. Angular MATTE BLACK armor panels covering the chest, shoulders, hips and thighs, with sharp geometric seams and clean straight edges. Vivid YELLOW and RED sharp V-shaped accents on the thighs, hips and collarbone, plus a small yellow triangle emblem on the chest. Cartoon 3D game art, front view, plain light gray background, no shadows, whole object centered and fully visible.
```

**부츠** (왼쪽 1개 생성 → 미러링)
```
A single boot for the left foot of a game character, shown as one isolated object with no leg, no foot and no body. A chunky black high-top sport boot with a thick bulky sole, a YELLOW V-shaped mark on the front and a RED heel. Simple blocky shape, large smooth surfaces, clean edges, no small greebles, no laces. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, object centered and fully visible.
```

> **`isolated hollow shell with NO BODY and NO PERSON inside, like clothing on an invisible mannequin`**
> — 이 구절이 의상 파츠의 핵심입니다. 사람이 함께 그려지면 3D 에 몸이 딸려 나와
> 베이스 바디와 겹칩니다.

### 장비 프롬프트 (**스킨 불변**)

**헤어**
```
A single hairstyle asset for a game character, shown as an isolated object with no head, no face and no body. Long thick RED ponytail tied high, with bright YELLOW streaks running through it, plus swept side bangs. One solid connected volume, smooth clean surface, no thin flyaway strands, no individual hair fibers. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, whole object centered and fully visible.
```

**고글**
```
A single pair of sport visor goggles for a game character, shown as an isolated object with no head and no face. Wide wraparound black lens covering the whole eye area, thick YELLOW strap band, simple blocky frame. Three or four large smooth surfaces, clean edges, no small greebles. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, whole object centered and fully visible.
```

**어깨 포드** (1종 생성 → 좌우 복제)
```
A single cylindrical shoulder thruster pod for a game character, shown as one isolated object with no body and no arm. A short thick cylinder lying horizontally on a flat dark grey mounting bracket, with a RED glowing ring on the outer end face and a YELLOW housing shell around it. Simple blocky shape, three or four large smooth plates, clean straight edges, no small greebles, no pipes. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, object centered and fully visible.
```

**무릎 포드** (1종 생성 → 좌우 복제)
```
A single angular knee thruster pod for a game character, shown as one isolated object with no body and no leg. An angular wedge-shaped armor piece that caps the front of a knee, with a RED glowing ring on the upper end and a YELLOW housing shell over a dark grey base. Simple blocky shape, three or four large smooth plates, clean straight edges, no small greebles, no pipes. Cartoon 3D game art, three-quarter view, plain light gray background, no shadows, object centered and fully visible.
```

> **부착물 이미지는 `one isolated object with no body` 로 고정합니다.**
> 사람이나 팔다리가 함께 그려지면 3D 에 그 조각이 딸려 나옵니다.
>
> **어깨와 무릎 포드를 한 이미지에 담지 않습니다.** 둘이 한 메시로 붙어 나오면
> 블렌더에서 갈라내야 합니다. 따로 뽑는 편이 그 작업을 없앱니다.

### 파츠 목록과 조립 (7종)

| 레이어 | 파츠 | 폴리곤 | 개수 | 조립 방법 |
|---|---|---|---|---|
| 베이스 | 베이스 바디 | 6,000 | 1 | **`Rig` 로 Humanoid 리깅.** 나머지가 여기 붙는다 |
| 의상 | 아머 세트 | 4,000 | 1 | 블렌더 `Automatic Weights` 로 아마추어에 바인딩 |
| 의상 | 부츠 | 1,500 | 2 (미러링) | 〃 또는 `Foot` 본에 강체 부착 |
| 장비 | 헤어 | 5,000 | 1 | `Head` 본에 강체 부착 |
| 장비 | 고글 | 1,200 | 1 | `Head` 본에 강체 부착 |
| 장비 | 어깨 포드 | 1,000 | 2 | `LeftShoulder` · `RightShoulder` |
| 장비 | 무릎 포드 | 1,000 | 2 | `LeftLowerLeg` · `RightLowerLeg` |

**화면상 합계 약 21,700 tris 로 규격(15,000)을 넘습니다.** `Remesh` 가 0 크레딧이므로
포드(각 1,000 → 300)와 헤어(5,000 → 2,500)를 깎아 맞춥니다.

### 1차 생성 결과 (2026-09-16 · 장비 4종)

| 파츠 | 판정 | 메모 |
|---|---|---|
| 헤어 | **양호** | 빨강·노랑 스트릭이 살아 있고 가닥이 분리되지 않은 깔끔한 덩어리 |
| 고글 | **양호 · 색 확인 필요** | 형태는 단순하고 좋으나 **프레임까지 노란색**으로 나왔다. 참고 이미지는 검정 프레임 + 노란 스트랩이었다 |
| 어깨 포드 | **양호** | 노란 원통 + 빨간 발광 링 + 회색 마운트 |
| 무릎 포드 | **양호** | 각진 웨지 + 빨간 링 |

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
| 어깨 추진기 | **원통형 포드가 어깨 위에 얹혀 돌출.** 빨간 발광 링 + 노란 하우징 | `Cylindrical thruster pods mounted on top of both shoulders ... RED glowing rings set in YELLOW housings` |
| 무릎 추진기 | 각진 포드가 무릎을 덮으며 돌출. 같은 배색 | `angular thruster pods on both knees` |
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

## 개정 이력

| 버전 | 날짜 | 변경 |
|---|---|---|
| v1 | 2026-09-16 | 최초. 산문형 서술 |
| v2 | 2026-09-16 | 사용자 제안 구조 채택 — 성별 대문자, `COLOR SCHEME:` 블록 분리, 밝은 회색 배경, `symmetrical` |
| v3 | 2026-09-16 | 길이를 80단어대로 압축. 의상을 구조로 서술. 손·얼굴 지시와 배제 문구 추가. **추진기를 돌출형 → 매립형으로 변경** |

## 다음에 시도할 것

- [ ] **참고 이미지를 직접 `ImageInput` 에 넣어 3D 생성**하는 쪽이 재생성보다 확실하다.
      이미지 파일 경로를 받으면 `upload_image` 로 올려 바로 넘길 수 있다 (240 크레딧 절약)
- [ ] 긴 포니테일이 3D 로 살아남는지 확인 (위 주의 2)
- [ ] 어깨·무릎 포드가 뭉개지지 않는지 확인
- [ ] 빨강·노랑 하이라이트가 3D 텍스처로 넘어갈 때 살아남는지 확인
