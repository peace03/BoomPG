using BoomPG.Core.Logging;
using BoomPG.Gameplay.Combat;
using BoomPG.Gameplay.Config;
using UnityEngine;

namespace BoomPG.Gameplay.Player
{
    /// <summary>입력, 외부 속도, 중력을 통합한다.</summary>
    [RequireComponent(typeof(CharacterController))]
    [DefaultExecutionOrder(-100)]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private MovementConfig _movementConfig;
        [SerializeField] private CombatConfig _combatConfig;

        private CharacterController _controller;
        private JetpackController _jetpack;
        private Vector3 _moveDirection;
        private Vector3 _inputVelocity;
        private Vector3 _externalVelocity;
        private Vector3 _horizontalAssist;
        private float _verticalVelocity;
        private bool _hasHorizontalAssist;
        private bool _hasVerticalOverride;
        private bool _jumpRequested;

        /// <summary>직전 이동의 접지 여부.</summary>
        public bool IsGrounded => _controller != null && _controller.isGrounded;
        /// <summary>넉백과 견인으로 누적된 월드 속도 (m/s).</summary>
        public Vector3 ExternalVelocity => _externalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _jetpack = GetComponent<JetpackController>();
            if (_movementConfig == null || _combatConfig == null)
            {
                GameLog.Error("Player", $"{name} 이동 또는 전투 설정 누락");
                enabled = false;
                return;
            }

            _controller.radius = _movementConfig.CapsuleRadius;
            _controller.height = _movementConfig.CapsuleHeight;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            bool grounded = IsGrounded;
            float drag = grounded ? _movementConfig.GroundDrag : _movementConfig.AirDrag;
            _externalVelocity = Vector3.MoveTowards(_externalVelocity, Vector3.zero, drag * deltaTime);
            _inputVelocity = _moveDirection * _movementConfig.MoveSpeed *
                (grounded ? 1f : _movementConfig.AirControl);
            if (_hasHorizontalAssist && !grounded)
            {
                _inputVelocity = _horizontalAssist;
            }

            if (grounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            if (_jumpRequested && grounded)
            {
                _verticalVelocity = Mathf.Sqrt(2f * -_movementConfig.Gravity * _movementConfig.JumpHeight);
            }

            if (!_hasVerticalOverride)
            {
                _verticalVelocity += _movementConfig.Gravity * deltaTime;
            }

            _controller.Move((_inputVelocity + _externalVelocity +
                Vector3.up * _verticalVelocity) * deltaTime);
            _jumpRequested = false;
            _hasHorizontalAssist = false;
            _hasVerticalOverride = false;
        }

        /// <summary>카메라 yaw로 변환한 월드 XZ 단위 방향을 받는다.</summary>
        public void SetMoveInput(Vector3 worldDirection)
        {
            worldDirection.y = 0f;
            _moveDirection = Vector3.ClampMagnitude(worldDirection, 1f);
        }

        /// <summary>접지한 경우에만 점프를 요청한다.</summary>
        public void RequestJump()
        {
            if (enabled && IsGrounded)
            {
                _jumpRequested = true;
            }
        }

        /// <summary>월드 속도(m/s)를 합성한다. 견인은 제트팩을 잠그지 않는다.</summary>
        public void ApplyKnockback(Vector3 velocity, bool isPull = false)
        {
            if (!enabled)
            {
                return;
            }

            _externalVelocity = KnockbackCalculator.Blend(_externalVelocity, velocity,
                _combatConfig.BlendMode, _combatConfig.MaxKnockbackSpeed);
            if (!isPull && _jetpack != null && _jetpack.enabled)
            {
                _jetpack.Lock(_jetpack.HitLockSeconds);
            }
        }

        /// <summary>이번 이동의 수직 속도를 덮어쓴다 (m/s).</summary>
        public void SetVerticalVelocity(float value)
        {
            _verticalVelocity = value;
            _hasVerticalOverride = true;
        }

        /// <summary>이번 공중 이동의 월드 수평 속도를 지정한다 (m/s).</summary>
        public void SetHorizontalAssist(Vector3 velocity)
        {
            velocity.y = 0f;
            _horizontalAssist = velocity;
            _hasHorizontalAssist = true;
        }

        /// <summary>리스폰 후 이전 이동이 이어지지 않도록 모든 누적 속도를 지운다.</summary>
        public void ResetVelocity()
        {
            _moveDirection = Vector3.zero;
            _inputVelocity = Vector3.zero;
            _externalVelocity = Vector3.zero;
            _horizontalAssist = Vector3.zero;
            _verticalVelocity = 0f;
            _hasHorizontalAssist = false;
            _hasVerticalOverride = false;
            _jumpRequested = false;
        }
    }
}
