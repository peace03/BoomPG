using BoomPG.Core.Logging;
using BoomPG.Gameplay.Config;
using UnityEngine;

namespace BoomPG.Gameplay.Combat
{
    /// <summary>단발 발사와 자동 재장전을 관리한다.</summary>
    public class RpgLauncher : MonoBehaviour
    {
        [SerializeField] private GameObject _rocketPrefab;
        [SerializeField] private CombatConfig _combatConfig;

        private float _reloadRemaining;
        private bool _hasRound = true;

        /// <summary>재장전 진행 여부.</summary>
        public bool IsReloading => _reloadRemaining > 0f;
        /// <summary>재장전 완료 비율 (0~1). 장전 상태는 1.</summary>
        public float ReloadProgress => !IsReloading ? 1f :
            Mathf.Clamp01(1f - _reloadRemaining / _combatConfig.ReloadSeconds);

        private void Awake()
        {
            if (_rocketPrefab == null || _combatConfig == null ||
                _rocketPrefab.GetComponent<RocketProjectile>() == null)
            {
                GameLog.Error("Combat", $"{name} 로켓 프리팹 또는 전투 설정 누락");
                enabled = false;
            }
        }

        private void Update()
        {
            if (!IsReloading)
            {
                return;
            }

            _reloadRemaining = Mathf.Max(0f, _reloadRemaining - Time.deltaTime);
            if (!IsReloading)
            {
                _hasRound = true;
            }
        }

        /// <summary>월드 위치(m)에서 단위 방향으로 장전된 로켓을 발사한다.</summary>
        public void TryFire(Vector3 origin, Vector3 direction)
        {
            if (!enabled || IsReloading || !_hasRound || direction.sqrMagnitude <= 0f)
            {
                return;
            }

            GameObject rocket = Instantiate(_rocketPrefab, origin, Quaternion.LookRotation(direction));
            rocket.GetComponent<RocketProjectile>().Launch(origin, direction, gameObject, _combatConfig);
            _hasRound = false;
            RequestReload();
        }

        /// <summary>빈 탄창의 재장전을 시작한다. 진행 중인 재장전은 유지한다.</summary>
        public void RequestReload()
        {
            if (!enabled || IsReloading || _hasRound)
            {
                return;
            }

            _reloadRemaining = Mathf.Max(0f, _combatConfig.ReloadSeconds);
            if (!IsReloading)
            {
                _hasRound = true;
            }
        }
    }
}
