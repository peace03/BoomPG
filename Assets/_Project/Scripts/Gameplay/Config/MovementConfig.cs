using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>플레이 검증에 사용할 설정값을 보관한다.</summary>
    [CreateAssetMenu(menuName = "BoomPG/MovementConfig")]
    public class MovementConfig : ScriptableObject
    {
        [Header("이동")]
        [Tooltip("지상에서의 최대 이동 속도 (m/s).")]
        [SerializeField] private float _moveSpeed = 6f;
        [Tooltip("점프로 도달하는 최고 높이 (m). 초기 속도는 이 값과 중력으로 계산된다.")]
        [SerializeField] private float _jumpHeight = 1.8f;
        [Tooltip("공중에서의 조작력 비율. 지상 대비 이 비율만큼만 방향을 바꿀 수 있다. 높이면 넉백당한 뒤 스스로 복귀하기 쉬워져 낙사가 줄어든다 — 이 게임의 핵심 재미(P1)에 직접 영향을 준다.")]
        [Range(0f, 1f)]
        [SerializeField] private float _airControl = 0.4f;
        [Tooltip("넉백으로 날아가는 중 좌우로 움직일 수 있는 정도 (비율). 날아가는 방향으로는 가속도 감속도 되지 않고, 이 값만큼 좌우로만 조정할 수 있다. 올리면 착지 지점을 고르기 쉬워져 낙사가 줄어든다.")]
        [Range(0f, 1f)]
        [SerializeField] private float _knockbackLateralControl = 0.5f;
        [Tooltip("넉백 중으로 판정하는 수평 속도 하한 (m/s). 이 값보다 느려지면 일반 공중 조작으로 돌아간다. 높이면 넉백이 끝났다고 판단하는 시점이 빨라진다.")]
        [SerializeField] private float _knockbackControlThreshold = 1f;
        [Header("물리 · 감쇠")]
        [Tooltip("중력 가속도 (m/s²). 음수다. 절댓값을 키우면 낙하가 빨라져 제트팩으로 복귀할 여유가 줄어든다.")]
        [SerializeField] private float _gravity = -9.81f;
        [Tooltip("지면에 붙어 있을 때 넉백 속도가 줄어드는 비율 (m/s per s). 높이면 밀려나도 금방 멈춘다.")]
        [SerializeField] private float _groundDrag = 8f;
        [Tooltip("공중에서 넉백 속도가 줄어드는 비율 (m/s per s). 낮을수록 오래 날아가지만 그만큼 조작 불가 시간이 길어진다. 18 m/s 넉백은 이 값이 2.5면 0이 되기까지 약 7초가 걸린다.")]
        [SerializeField] private float _airDrag = 2.5f;
        [Header("캐릭터 캡슐")]
        [Tooltip("충돌 캡슐의 반지름 (m). 바꾸면 CharacterController 컴포넌트 설정과 함께 맞춰야 한다.")]
        [SerializeField] private float _capsuleRadius = 0.4f;
        [Tooltip("충돌 캡슐의 높이 (m). 바꾸면 CharacterController 컴포넌트 설정과 함께 맞춰야 한다.")]
        [SerializeField] private float _capsuleHeight = 1.8f;

        /// <summary>MoveSpeed 설정값 (m/s).</summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>JumpHeight 설정값 (m).</summary>
        public float JumpHeight => _jumpHeight;

        /// <summary>AirControl 설정값 (비율).</summary>
        public float AirControl => _airControl;

        /// <summary>넉백 중 좌우 조작 비율.</summary>
        public float KnockbackLateralControl => _knockbackLateralControl;

        /// <summary>넉백 판정 수평 속도 하한 (m/s).</summary>
        public float KnockbackControlThreshold => _knockbackControlThreshold;

        /// <summary>Gravity 설정값 (m/s²).</summary>
        public float Gravity => _gravity;

        /// <summary>GroundDrag 설정값 (m/s²).</summary>
        public float GroundDrag => _groundDrag;

        /// <summary>AirDrag 설정값 (m/s²).</summary>
        public float AirDrag => _airDrag;

        /// <summary>CapsuleRadius 설정값 (m).</summary>
        public float CapsuleRadius => _capsuleRadius;

        /// <summary>CapsuleHeight 설정값 (m).</summary>
        public float CapsuleHeight => _capsuleHeight;
    }
}
