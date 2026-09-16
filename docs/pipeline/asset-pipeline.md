# 에셋 파이프라인 (Asset Pipeline)

> 현재 상태: 규격 확정 · 첫 캐릭터(로카) 반입 진행 중

## 1. 전제

3D 모델은 **BARCO AI**로 생성합니다 (README 참조).
AI 생성 모델은 폴리곤 수·머티리얼 구성·스케일·피벗·본 구조가 매번 다르므로,
규격 없이 반입하면 런타임 성능과 애니메이션이 모두 무너집니다.

캐릭터 모델의 외형·컬러 테마 기준은 [design/worldbuilding.md](../design/worldbuilding.md) 3장입니다.
생성 프롬프트를 쓸 때 그 문서의 외형 묘사를 기초 자료로 삼으십시오.

| 캐릭터 | 컬러 테마 | 실루엣 특징 |
|---|---|---|
| 스파크 (`Spark`) | 네온 그린 & 옐로우 | 날렵한 스포츠 웨어, 바이저 고글, 관절부 추진기 |
| 로카 (`Rocca`) | 핫 오렌지 & 레드 | 과장된 폭격기 점퍼, 탄띠, 팔목 보조 발사기 |
| 기즈모 (`Gizmo`) | 시안 & 다크 퍼플 | 비대칭 테크웨어, 오른팔 전체를 덮는 갈고리 건틀릿 |

위 실루엣 특징은 **스킨이 바꿀 수 없는 불변 요소**입니다
([ADR-0006](../decisions/ADR-0006-캐릭터-실루엣-불변.md)).
스킨은 색·텍스처·패턴·재질과 실루엣에 영향을 주지 않는 소품만 바꿉니다.
전체 비율·어깨 폭·위 표의 돌출 요소는 고정입니다.

## 2. 반입 규격 (확정 — D-012)

근거와 검토 과정은 [ADR-0007](../decisions/ADR-0007-캐릭터-에셋-반입-규격.md)에 있습니다.

| 항목 | 값 | 비고 |
|---|---|---|
| 파일 포맷 | **FBX (Binary)** | glTF는 기본 임포터가 없다 |
| 단위 스케일 | **1 unit = 1 m** | 내보낼 때 스케일 100 배율에 주의 |
| 피벗 | **발밑 원점** | (0,0,0)에서 +Y로 선다 |
| 정면 축 | **+Z** | |
| 폴리곤 | **15,000 tris 이하** (1체) | 8인 합계 120,000. LOD 없이 간다 — M2 실측 후 재판단 |
| 텍스처 해상도 | **2048 × 2048 상한** | Unity 임포트 설정으로 하향 가능 |
| 텍스처 채널 | Albedo · Normal (필수) / MaskMap (선택) | 카툰 렌더라 Metallic·Smoothness 비중이 낮다 |
| 머티리얼 슬롯 | **캐릭터당 1개** (최대 2개) | 8인 × 슬롯 수만큼 드로우콜이 늘어난다 |
| 리깅 | **Unity Humanoid** | 세 캐릭터가 애니메이션을 공유하기 위한 전제 |
| 애니메이션 | 모델 파일에 포함하지 않음 | 별도 관리하여 3종이 공유한다 |

> **Humanoid 리깅이 이 규격의 핵심입니다.** 세 캐릭터는 기본 스펙이 100% 동일하고
> (gdd 5.1) 스킬 하나만 다르므로, 걷기·점프·낙하·피격 같은 기본 동작을 전부 공유합니다.
> Generic으로 가면 애니메이션 작업량이 3배가 됩니다.
>
> **기즈모의 건틀릿은 메시로 만들고 본 구조는 좌우 대칭을 유지하십시오.**
> 극단적 비율을 본으로 만들면 리타게팅 시 어색해집니다.
> 실루엣의 비대칭([ADR-0006](../decisions/ADR-0006-캐릭터-실루엣-불변.md))은 메시가 담당합니다.

### 2.1 BARCO AI 출력 특성 (2026-09-16 실측)

로카(`SK_Rocca.fbx`, 14.5 MB)를 분석해 확인한 내용입니다.

| 항목 | 실측값 | 판정 |
|---|---|---|
| 포맷 | FBX Binary (Kaydara) | 일치 |
| 리깅 | **포함됨** — Mixamo 표준 본 22개 | **자동 리깅 후처리 불필요** |
| 본 명명 | `Hips`·`Spine/1/2`·`Neck`·`Head`·`LeftArm`·`LeftForeArm`·`LeftUpLeg` 등 | Unity Humanoid 자동 매핑 규칙 |
| 스키닝 | Skin 1 + Cluster 22 + BindPose 3 | 정상 바인딩 |
| 본 대칭 | Left/Right 짝 완전 일치 | ADR-0006 요건 충족 |
| 손가락 본 | 없음 | Humanoid 옵션이므로 무방 |
| **폴리곤** | **15,000 tris** (정점 11,782) | 규격 상한과 동일 |
| 텍스처 | **FBX에 임베드** — 2048×2048 PNG 3장 | 일치 |
| 텍스처 구성 | `base_color` · `normal` · **`orm`** | 아래 주의 |

BARCO AI의 기본 출력이 **15,000 tris** 수준으로 보입니다. 최초 규격은 8,000이었으나
이 실측을 근거로 15,000으로 개정했습니다 (ADR-0007 개정 이력 참조).

### 2.1.1 산출물이 여러 벌로 온다

BARCO AI 워크플로우는 한 캐릭터에 대해 **여러 단계의 파일**을 함께 내려줍니다.
로카의 경우 다음과 같았습니다.

| 파일 | 폴리곤 | 리깅 | 용도 |
|---|---|---|---|
| `SK_Rocca.fbx` (14.5 MB) | 15,000 tris | **있음** (Mixamo 본 22개) | **← 이것을 쓴다** |
| `Roca_Retopo.fbx` (7.7 MB) | 7,752 tris | **없음** (본 0개) | 리토폴로지 중간 산출물 |
| `Roca_Retopo_Textures/` | — | — | 텍스처 5장 (아래) |

**폴리곤이 적다고 Retopo 파일을 쓰면 안 됩니다.** 본이 하나도 없어 애니메이션을 붙일 수 없습니다.

> **워크플로우 개선 여지** — 리토폴로지된 메시(7,752)에 리깅을 하면 폴리곤 절반으로
> 같은 결과를 얻습니다. BARCO AI에서 그 조합을 받을 수 있는지, 아니면 Retopo FBX를
> Mixamo 등에 올려 자동 리깅할 수 있는지 다음 캐릭터에서 시도해 보십시오.
> 성공하면 규격을 8,000으로 되돌릴 수 있습니다.

### 2.1.2 텍스처는 별도 파일로도 제공된다 — 추출하지 말 것

FBX에 임베드된 텍스처를 Unity의 `Extract Textures...` 로 빼낼 수도 있지만,
**원본 폴더에 이미 개별 PNG로 들어 있습니다.** 그쪽을 복사하는 편이 간단하고 확실합니다.

| 원본 파일 | 복사 후 이름 | URP Lit 슬롯 |
|---|---|---|
| `Roca_BaseColor.png` | `T_Rocca_Albedo.png` | Base Map |
| `Roca_Normal.png` | `T_Rocca_Normal.png` | Normal Map |
| `Roca_MetallicSmoothness.png` | `T_Rocca_MetallicSmoothness.png` | Metallic Map |
| `Roca_AO.png` | `T_Rocca_AO.png` | Occlusion Map |
| `Roca_ORM.png` | **복사하지 않음** | — |

전부 2048×2048 이라 규격을 만족합니다.

> **`MetallicSmoothness` 가 함께 제공되므로 ORM 문제는 발생하지 않습니다.**
> ORM(glTF·Unreal 표준: R=Occlusion, G=Roughness, B=Metallic)은 URP Lit의
> Metallic Gloss Map(R=Metallic, A=Smoothness)과 채널 배치가 다르고 Roughness는
> Smoothness의 반대값이라, 그대로 연결하면 거친 표면이 금속처럼 보입니다.
> **이미 변환된 `MetallicSmoothness` 를 쓰고 `ORM` 은 무시하십시오.**

복사한 뒤 Unity에서 `Ctrl+R` 로 새로고침하고, **`T_<캐릭터>_Normal` 의 Texture Type 을
`Normal map` 으로 바꾸십시오.** 이 설정은 FBX가 아니라 **PNG 파일을 선택했을 때**
Inspector 맨 위에 나타납니다.

### 2.2 Art 폴더 구조 (확정)

**`Art/` 에는 원본 아트 에셋만** 둡니다. 조립된 프리팹은 `Assets/_Project/Prefabs/` 입니다.

```
Assets/_Project/Art/
  Characters/
    Spark/
      SK_Spark.fbx
      Materials/     M_Spark.mat
      Textures/      T_Spark_Albedo · T_Spark_Normal · T_Spark_MetallicSmoothness · T_Spark_AO
    Rocca/           같은 구성
    Gizmo/           같은 구성
  Environment/
    SkyYard/
      Models/        SM_DockingArm.fbx · SM_ControlTower.fbx ...
      Materials/
      Textures/
  Weapons/
    Rpg/             SK_Rpg.fbx (캐릭터 손에 붙으므로 스킨드)
    Rocket/          SM_Rocket.fbx
  VFX/
    Materials/       M_Explosion.mat ...
    Textures/        T_Explosion_Sheet.png ...
  Animations/
    Locomotion/      걷기 · 달리기 · 점프 · 낙하 · 착지
    Combat/          발사 · 재장전 · 피격 · 넉백
    Skills/          대시 · 예비탄 · 갈고리
```

**캐릭터는 대상별로 묶고, 애니메이션은 밖에 둡니다.**
캐릭터 폴더를 묶는 이유는 스킨(코스메틱)이 추가될 때 그 캐릭터 아래로 확장되기 때문이고,
애니메이션을 빼는 이유는 Humanoid 리깅 덕분에 **세 캐릭터가 같은 클립을 공유**하기 때문입니다.
특정 캐릭터 폴더에 넣으면 소속이 틀립니다.

> 캐릭터 FBX는 스킨드 메시이므로 **`SK_` 접두**를 씁니다 (3장 명명 규칙).
> BARCO AI 출력 파일명(`Rocca.fbx`)을 그대로 두지 말고 `SK_Rocca.fbx` 로 바꾸십시오.

## 3. 명명 규칙 (확정)

| 종류 | 규칙 | 예 |
|---|---|---|
| 모델 | `SM_<대상>` / 스킨드는 `SK_<대상>` | `SK_Rocca` |
| 머티리얼 | `M_<대상>` | `M_Rocca` |
| 텍스처 | `T_<대상>_<채널>` | `T_Rocca_Albedo` |
| 프리팹 | `P_<대상>` | `P_Rocket` |
| VFX | `VFX_<사건>` | `VFX_Explosion` |
| 오디오 | `SFX_<사건>` / `BGM_<곡명>` | `SFX_RocketLaunch` |
| 데이터 (ScriptableObject) | `SO_<대상>` | `SO_CombatConfig` |

대소문자와 구분자는 위 형식 그대로. 공백과 한글 파일명은 쓰지 않습니다.
(Unity 에셋 경로에 한글이 섞이면 일부 도구 체인에서 문제가 생깁니다)

## 4. 캐릭터 반입 절차

순서대로 진행하고, 각 단계를 통과하지 못하면 **다음으로 넘어가지 말고 모델을 고치십시오.**

### 4.1 생성

BARCO AI로 생성합니다. 프롬프트는 [worldbuilding.md](../design/worldbuilding.md) 3장의
외형 묘사와 1장 컬러 테마를 기초로 씁니다.

### 4.2 배치

`Assets/_Project/Art/Characters/<캐릭터명>/` 에 `SK_<캐릭터명>.fbx` 로 넣습니다 (2.2절 구조).

### 4.3 규격 검수

Project 창에서 FBX를 선택하면 **Inspector 맨 아래 프리뷰 영역**에 `verts / tris` 가 표시됩니다.

- [ ] 삼각형 15,000 이하
- [ ] 단위 1 unit = 1 m — 캐릭터 키가 약 1.8 m로 들어오는가
- [ ] 피벗이 발밑 (0,0,0)
- [ ] 정면이 +Z
- [ ] 머티리얼 슬롯 1개 (최대 2개)
- [ ] 텍스처 2048 이하, Albedo·Normal 존재

### 4.4 임포트 설정

| 탭 | 항목 | 값 |
|---|---|---|
| Model | Scale Factor | 1 |
| Model | Convert Units | 체크 |
| Model | Mesh Compression | Medium |
| Model | Read/Write | **해제** (런타임 메시 수정이 없다. 메모리 절감) |
| Model | Optimize Mesh | 체크 |
| Rig | Animation Type | **Humanoid** |
| Rig | Avatar Definition | Create From This Model |
| Animation | Import Animation | **해제** (애니메이션은 별도 관리) |
| Materials | Material Creation Mode | Standard |

### 4.5 리깅 확인

- [ ] Rig 탭에서 Humanoid로 설정하고 Apply 했는가
- [ ] **Configure** 를 열어 필수 본이 모두 매핑되는가 (빨간 표시가 없어야 함)
- [ ] 본 구조가 좌우 대칭인가 — 기즈모의 건틀릿은 **메시**여야 하고 본이 아니다
- [ ] T-pose 또는 A-pose로 들어오는가

매핑이 실패하면 Configure에서 수동으로 맞추거나, 후처리(자동 리깅)를 거칩니다.

### 4.6 텍스처와 머티리얼

**FBX에서 추출하지 말고 원본 텍스처 폴더에서 복사합니다** (2.1.2절).

- [ ] 텍스처 4장을 `Textures/` 에 `T_<캐릭터>_<채널>` 이름으로 복사한다 (ORM은 제외)
- [ ] Unity에서 `Ctrl+R` 로 새로고침한다
- [ ] `T_<캐릭터>_Normal` 을 선택하고 **Inspector 맨 위 Texture Type 을 `Normal map`** 으로
      바꾼 뒤 Apply — 이 설정은 PNG를 선택했을 때만 보이며 FBX에는 없다
- [ ] `Materials/` 에서 우클릭 → `Create > Material` → `M_<캐릭터>` 생성
- [ ] Shader를 `Universal Render Pipeline/Lit` 으로 두고 아래를 연결한다

| URP Lit 슬롯 | 텍스처 |
|---|---|
| Base Map | `T_<캐릭터>_Albedo` |
| Normal Map | `T_<캐릭터>_Normal` |
| Metallic Map | `T_<캐릭터>_MetallicSmoothness` |
| Occlusion Map | `T_<캐릭터>_AO` |

- [ ] 씬의 캐릭터 또는 프리팹의 Mesh Renderer Material 슬롯에 `M_<캐릭터>` 를 지정한다

### 4.7 실루엣 테스트

색을 빼고 형태만으로 세 캐릭터가 구분되는지 확인합니다. **색으로 보완하지 마십시오.**

- [ ] 세 캐릭터를 단색(검정)으로 렌더한다
- [ ] 정면 · 측면 · 후면 세 각도에서 비교한다
- [ ] 색 정보 없이 어느 캐릭터인지 **즉시** 구분된다
- [ ] 30 m 거리(교전 거리 하한)에서 보이는 크기로 축소해도 구분된다

카운터 삼각관계(gdd 5.2)는 상대 클래스를 즉시 알아야 성립하므로,
이 테스트는 게임플레이 요구사항이지 아트 취향의 문제가 아닙니다.
**세 캐릭터가 모두 반입된 뒤에 한 번에 수행합니다.**

### 4.8 프리팹화

`Assets/_Project/Prefabs/` 에 `P_<캐릭터명>` 으로 저장합니다 (예: `P_Rocca`).

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성(뼈대). 명명 규칙 확정, 미결정 D-012 등록 |
| 2026-09-16 | ScriptableObject 명명(`SO_`) 추가 |
| 2026-09-16 | 캐릭터 3인의 컬러 테마·실루엣 특징 표 추가 (worldbuilding 반입) |
| 2026-09-16 | ADR-0006 채택. 실루엣 테스트를 반입 절차에 편입 |
| 2026-09-16 | ADR-0007 채택. D-012 해소 — 반입 규격 확정 |
| 2026-09-16 | 로카 실측 반영. 폴리곤 규격 8,000 → 15,000 개정, 문서 구조 정리 |
| 2026-09-16 | 2.1.1·2.1.2 추가 — 산출물이 여러 벌로 오는 점, 텍스처는 추출 대신 복사할 것 |
