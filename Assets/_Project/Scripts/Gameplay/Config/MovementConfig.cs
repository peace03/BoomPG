using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>플레이 검증에 사용할 설정값을 보관한다.</summary>
    [CreateAssetMenu(menuName = "BoomPG/MovementConfig")]
    public class MovementConfig : ScriptableObject
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _jumpHeight = 1.8f;
        [SerializeField] private float _airControl = 0.4f;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _groundDrag = 8f;
        [SerializeField] private float _airDrag = 2.5f;
        [SerializeField] private float _capsuleRadius = 0.4f;
        [SerializeField] private float _capsuleHeight = 1.8f;

        /// <summary>MoveSpeed 설정값 (m/s).</summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>JumpHeight 설정값 (m).</summary>
        public float JumpHeight => _jumpHeight;

        /// <summary>AirControl 설정값 (비율).</summary>
        public float AirControl => _airControl;

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
