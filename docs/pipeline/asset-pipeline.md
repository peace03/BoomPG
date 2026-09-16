# 에셋 파이프라인 (Asset Pipeline)

> 현재 상태: **뼈대**. 아직 반입된 게임 에셋이 없습니다.

## 1. 전제

3D 모델은 **BARCO AI**로 생성합니다 (README 참조).

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

### 2.1 실루엣 테스트 (캐릭터 모델 반입 전 필수)

BARCO AI 산출물을 반입하기 전에 아래를 통과해야 합니다. **색으로 보완하지 마십시오.**

- [ ] 세 캐릭터를 단색(검정)으로 렌더한다
- [ ] 정면 · 측면 · 후면 세 각도에서 비교한다
- [ ] 색 정보 없이 어느 캐릭터인지 **즉시** 구분된다
- [ ] 30 m 거리(교전 거리 하한)에서 보이는 크기로 축소해도 구분된다

통과하지 못하면 형태를 수정합니다. 카운터 삼각관계(gdd 5.2)는 상대 클래스를 즉시 알아야
성립하므로, 이 테스트는 게임플레이 요구사항이지 아트 취향의 문제가 아닙니다.

AI 생성 모델은 폴리곤 수·머티리얼 구성·스케일·피벗이 제각각이므로, 반입 규격을 정하지 않으면
런타임 성능과 애니메이션이 모두 무너집니다. 첫 모델을 넣기 전에 규격을 확정해야 합니다.

## 2. 결정해야 할 것

**[D-012 — 확정]** 반입 규격은 아래와 같습니다.
근거와 검토 과정은 [ADR-0007](../decisions/ADR-0007-캐릭터-에셋-반입-규격.md)에 있습니다.

| 항목 | 값 | 비고 |
|---|---|---|
| 파일 포맷 | **FBX (Binary)** | glTF는 기본 임포터가 없다 |
| 단위 스케일 | **1 unit = 1 m** | 내보낼 때 스케일 100 배율에 주의 |
| 피벗 | **발밑 원점** | (0,0,0)에서 +Y로 선다 |
| 정면 축 | **+Z** | |
| 폴리곤 | **8,000 tris 이하** (1체) | 8인 합계 64,000. LOD 없이 간다 |
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

### 2.2 BARCO AI 출력 특성 (2026-09-16 실측)

로카(`Rocca.fbx`, 14.5 MB)를 분석해 확인한 내용입니다.

| 항목 | 실측값 | 대응 |
|---|---|---|
| 포맷 | FBX Binary (Kaydara) | 규격 일치 |
| 리깅 | **포함됨** — Mixamo 표준 본 22개 | **자동 리깅 후처리 불필요** |
| 본 명명 | `Hips`·`Spine/1/2`·`Neck`·`Head`·`LeftArm`·`LeftForeArm`·`LeftUpLeg` 등 | Unity Humanoid 자동 매핑 규칙 |
| 스키닝 | Skin 1 + Cluster 22 + BindPose 3 | 정상 바인딩 |
| 본 대칭 | Left/Right 짝 완전 일치 | ADR-0006 요건 충족 |
| 손가락 본 | 없음 | Humanoid 옵션이므로 무방 |
| 텍스처 | **FBX에 임베드** — 2048×2048 PNG 3장 | 규격 일치 |
| 텍스처 구성 | `base_color` · `normal` · **`orm`** | 아래 주의 |

> **`orm.png` 는 그대로 쓸 수 없습니다.**
> ORM은 glTF·Unreal 표준으로 **R=Occlusion, G=Roughness, B=Metallic** 이지만,
> URP Lit의 MaskMap은 **R=Metallic, G=Occlusion, B=Detail, A=Smoothness** 입니다.
> 그대로 연결하면 금속감과 거칠기가 뒤집혀 보입니다.
> 쓰려면 채널 셔플과 Roughness → Smoothness 반전(1 - R)이 필요합니다.
>
> **카툰 렌더링(gdd 16장)에서는 Metallic·Smoothness의 비중이 낮으므로,
> `base_color` + `normal` 만 쓰고 ORM을 버리는 편이 간단합니다.**
> 어느 쪽으로 갈지는 첫 캐릭터를 씬에 세워 보고 정합니다.

FBX에 텍스처가 임베드되어 있으므로, Unity 임포트 시 **Materials 탭에서 텍스처를 추출**해
해당 캐릭터의 `Textures/` 폴더(2.3 절)에 배치하십시오.
임베드 상태로 두면 텍스처 임포트 설정을 개별 조정할 수 없습니다.

## 2.3 Art 폴더 구조 (확정)

**`Art/` 에는 원본 아트 에셋만** 둡니다. 조립된 프리팹은 `Assets/_Project/Prefabs/` 입니다.

```
Assets/_Project/Art/
  Characters/
    Spark/
      SK_Spark.fbx
      Materials/     M_Spark.mat
      Textures/      T_Spark_Albedo.png · T_Spark_Normal.png · T_Spark_ORM.png
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
애니메이션을 빼는 이유는 Humanoid 리깅([ADR-0007](../decisions/ADR-0007-캐릭터-에셋-반입-규격.md))
덕분에 **세 캐릭터가 같은 클립을 공유**하기 때문입니다. 특정 캐릭터 폴더에 넣으면 소속이 틀립니다.

> 캐릭터 FBX는 스킨드 메시이므로 **`SK_` 접두**를 씁니다 (3장 명명 규칙).
> BARCO AI 출력 파일명(`Rocca.fbx`)을 그대로 두지 말고 `SK_Rocca.fbx` 로 바꾸십시오.

## 3. 명명 규칙 (확정)

| 종류 | 규칙 | 예 |
|---|---|---|
| 모델 | `SM_<대상>` / 스킨드는 `SK_<대상>` | `SK_PlayerBase` |
| 머티리얼 | `M_<대상>` | `M_PlayerBase` |
| 텍스처 | `T_<대상>_<채널>` | `T_PlayerBase_Albedo` |
| 프리팹 | `P_<대상>` | `P_Rocket` |
| VFX | `VFX_<사건>` | `VFX_Explosion` |
| 오디오 | `SFX_<사건>` / `BGM_<곡명>` | `SFX_RocketLaunch` |
| 데이터 (ScriptableObject) | `SO_<대상>` | `SO_KnockbackTable` |

대소문자와 구분자는 위 형식 그대로. 공백과 한글 파일명은 쓰지 않습니다.
(Unity 에셋 경로에 한글이 섞이면 일부 도구 체인에서 문제가 생깁니다)

## 4. 캐릭터 반입 절차

순서대로 진행하고, 각 단계를 통과하지 못하면 **다음으로 넘어가지 말고 모델을 고치십시오.**

### 4.1 생성

BARCO AI로 생성합니다. 프롬프트는 [worldbuilding.md](../design/worldbuilding.md) 3장의
외형 묘사와 2장 컬러 테마를 기초로 씁니다.

### 4.2 규격 검수

- [ ] 삼각형 8,000 이하
- [ ] 단위 1 unit = 1 m — 캐릭터 키가 약 1.8 m로 들어오는가
- [ ] 피벗이 발밑 (0,0,0)
- [ ] 정면이 +Z
- [ ] 머티리얼 슬롯 1개 (최대 2개)
- [ ] 텍스처 2048 이하, Albedo·Normal 존재

### 4.3 실루엣 테스트 (2.1 절)

색을 빼고 형태만으로 세 캐릭터가 구분되는지 확인합니다. **색으로 보완하지 마십시오.**

### 4.4 리깅 확인

- [ ] Unity Humanoid로 임포트되는가 (Rig → Animation Type = **Humanoid**)
- [ ] Avatar 설정에서 **필수 본이 모두 매핑**되는가 (Configure에서 빨간 표시가 없어야 함)
- [ ] 본 구조가 좌우 대칭인가 — 기즈모의 건틀릿은 **메시**여야 하고 본이 아니다
- [ ] T-pose 또는 A-pose로 들어오는가

리깅이 없거나 매핑이 실패하면 후처리(자동 리깅)를 거친 뒤 다시 확인합니다.

### 4.5 배치와 임포트 설정

`Assets/_Project/Art/Characters/<캐릭터명>/` 에 `SK_<캐릭터명>.fbx` 로 배치하고
(2.3 절 구조 참조) 아래 설정을 적용합니다.

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

### 4.6 프리팹화

`Assets/_Project/Prefabs/` 에 `P_<캐릭터명>` 으로 저장합니다 (예: `P_Spark`).

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성(뼈대). 명명 규칙 확정, 미결정 D-012 등록 |
| 2026-09-16 | ScriptableObject 명명(`SO_`) 추가 |
| 2026-09-16 | 캐릭터 3인의 컬러 테마·실루엣 특징 표 추가 (worldbuilding 반입) |
| 2026-09-16 | ADR-0006 채택. 2.1 실루엣 테스트를 반입 전 필수 절차로 추가 |
| 2026-09-16 | ADR-0007 채택. D-012 해소 — 반입 규격 확정, 4장 반입 절차를 체크리스트로 상세화 |
