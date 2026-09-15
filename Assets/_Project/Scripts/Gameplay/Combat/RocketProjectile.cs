using BoomPG.Core.Logging;
using BoomPG.Gameplay.Config;
using UnityEngine;

namespace BoomPG.Gameplay.Combat
{
    /// <summary>수동 속도와 선분 충돌로 로켓을 이동시킨다.</summary>
    public class RocketProjectile : MonoBehaviour
    {
        private static int _collisionMask = -1;
        private static int _playerLayer = -1;

        private CombatConfig _config;
        private GameObject _owner;
        private Vector3 _velocity;
        private float _launchedAt;
        private bool _launched;

        private void Awake()
        {
            if (_collisionMask < 0)
            {
                _collisionMask = LayerMask.GetMask("Player", "Platform");
                _playerLayer = LayerMask.NameToLayer("Player");
            }
        }

        private void FixedUpdate()
        {
            if (!_launched)
            {
                return;
            }

            if (Time.time - _launchedAt >= 5f)
            {
                _launched = false;
                Destroy(gameObject);
                return;
            }

            _velocity += Vector3.up *
                (Physics.gravity.y * _config.RocketGravityScale * Time.fixedDeltaTime);
            Vector3 previous = transform.position;
            Vector3 next = previous + _velocity * Time.fixedDeltaTime;
            if (Physics.Linecast(previous, next, out RaycastHit hit,
                _collisionMask, QueryTriggerInteraction.Ignore))
            {
                GameObject target = hit.collider.gameObject;
                if (target != gameObject && target != _owner &&
                    (_owner == null || !target.transform.IsChildOf(_owner.transform)))
                {
                    transform.position = hit.point;
                    _launched = false;
                    ExplosionResolver.Resolve(hit.point, _owner,
                        target.layer == _playerLayer ? target : null, _config);
                    Destroy(gameObject);
                    return;
                }
            }

            transform.position = next;
        }

        /// <summary>월드 위치(m), 단위 방향, 발사자와 설정으로 발사한다.</summary>
        public void Launch(Vector3 origin, Vector3 direction, GameObject owner, CombatConfig config)
        {
            if (config == null || direction.sqrMagnitude <= 0f)
            {
                GameLog.Error("Combat", "로켓 발사 설정 또는 방향 누락");
                enabled = false;
                Destroy(gameObject);
                return;
            }

            transform.position = origin;
            _owner = owner;
            _config = config;
            _velocity = direction.normalized * config.RocketSpeed;
            _launchedAt = Time.time;
            _launched = true;
        }
    }
}
