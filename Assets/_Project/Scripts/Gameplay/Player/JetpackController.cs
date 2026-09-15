using BoomPG.Core.Logging;
using BoomPG.Gameplay.Config;
using UnityEngine;

namespace BoomPG.Gameplay.Player
{
    /// <summary>공중 추진과 연료 회복, 피격 잠금을 관리한다.</summary>
    [DefaultExecutionOrder(-200)]
    public class JetpackController : MonoBehaviour
    {
        [SerializeField] private JetpackConfig _jetpackConfig;

        private PlayerMotor _motor;
        private float _fuel;
        private float _lockUntil;
        private float _groundedSeconds;
        private bool _held;
        private Vector3 _worldDirection;

        /// <summary>남은 연료 비율 (0~1).</summary>
        public float FuelRatio => _jetpackConfig != null && _jetpackConfig.MaxFuel > 0f
            ? Mathf.Clamp01(_fuel / _jetpackConfig.MaxFuel) : 0f;
        /// <summary>피격 잠금 여부.</summary>
        public bool IsLocked => Time.time < _lockUntil;
        /// <summary>설정된 피격 잠금 시간 (초).</summary>
        public float HitLockSeconds => _jetpackConfig != null ? _jetpackConfig.HitLockSeconds : 0f;

        private void Awake()
        {
            _motor = GetComponent<PlayerMotor>();
            if (_jetpackConfig == null || _motor == null)
            {
                GameLog.Error("Player", $"{name} 제트팩 설정 또는 모터 누락");
                enabled = false;
                return;
            }

            _fuel = _jetpackConfig.MaxFuel;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (_motor.IsGrounded)
            {
                // 대기 시간이 끝난 프레임에서도 실제 회복 가능한 시간만 계산한다.
                float previous = _groundedSeconds;
                _groundedSeconds += deltaTime;
                float refillTime = Mathf.Max(0f, _groundedSeconds - _jetpackConfig.RefillDelay) -
                    Mathf.Max(0f, previous - _jetpackConfig.RefillDelay);
                _fuel = Mathf.Min(_jetpackConfig.MaxFuel,
                    _fuel + _jetpackConfig.RefillPerSecond * refillTime);
                return;
            }

            _groundedSeconds = 0f;
            if (!_held || IsLocked || _fuel <= 0f || !_motor.enabled)
            {
                return;
            }

            _fuel = Mathf.Max(0f, _fuel - _jetpackConfig.ConsumePerSecond * deltaTime);
            _motor.SetVerticalVelocity(_jetpackConfig.AscendSpeed);
            _motor.SetHorizontalAssist(_worldDirection * _jetpackConfig.HorizontalSpeed);
        }

        /// <summary>홀드와 월드 XZ 단위 방향을 입력받는다.</summary>
        public void SetThrustInput(bool held, Vector3 worldDirection)
        {
            _held = held;
            worldDirection.y = 0f;
            _worldDirection = Vector3.ClampMagnitude(worldDirection, 1f);
        }

        /// <summary>기존 잠금이 짧아지지 않도록 갱신한다 (초).</summary>
        public void Lock(float seconds)
        {
            _lockUntil = Mathf.Max(_lockUntil, Time.time + seconds);
        }
    }
}
