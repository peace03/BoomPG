using System;
using BoomPG.Core.Logging;
using UnityEngine;

namespace BoomPG.Gameplay.Player
{
    /// <summary>체력 소진과 낙사를 구분한다.</summary>
    public enum DeathCause { Damage, RingOut }

    /// <summary>체력과 반복 검증용 스폰 복귀를 관리한다.</summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;

        private CharacterController _controller;
        private PlayerMotor _motor;
        private Vector3 _spawnPoint;

        /// <summary>현재 체력 (HP).</summary>
        public float Current { get; private set; }
        /// <summary>사망 이후 스폰 복귀 전 상태.</summary>
        public bool IsDead { get; private set; }
        /// <summary>사망당 한 번 발생한다. 마지막 인자는 가해자이며 없으면 null.</summary>
        public event Action<HealthComponent, DeathCause, GameObject> Died;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _motor = GetComponent<PlayerMotor>();
            _spawnPoint = transform.position;
            Current = _maxHealth;
        }

        private void LateUpdate()
        {
            if (!IsDead)
            {
                return;
            }

            // 같은 폭발의 넉백 적용과 이동 콜백이 끝난 뒤 초기화해야 재발사를 방지한다.
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            transform.position = _spawnPoint;
            if (_controller != null)
            {
                _controller.enabled = true;
            }

            if (_motor != null)
            {
                _motor.ResetVelocity();
            }

            Current = _maxHealth;
            IsDead = false;
        }

        /// <summary>양수 피해량(HP)을 적용하며 가해자를 사망 이벤트에 전달한다.</summary>
        public void ApplyDamage(float amount, GameObject source)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            Current = Mathf.Max(0f, Current - amount);
            if (Current <= 0f)
            {
                Die(DeathCause.Damage, source);
            }
        }

        /// <summary>낙사 처리한다. 가해자가 없으면 null을 전달한다.</summary>
        public void KillByRingOut(GameObject credit)
        {
            Die(DeathCause.RingOut, credit);
        }

        private void Die(DeathCause cause, GameObject source)
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            Current = 0f;
            GameLog.Combat($"{name} 사망 ({cause})");
            Died?.Invoke(this, cause, source);
        }
    }
}
