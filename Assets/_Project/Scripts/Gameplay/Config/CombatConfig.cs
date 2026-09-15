using System;
using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>거리 구간별 스플래시 수치.</summary>
    [Serializable]
    public struct SplashBand
    {
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _damage;
        [SerializeField] private float _knockbackSpeed;

        /// <summary>구간의 포함 상한 거리 (m).</summary>
        public float MaxDistance => _maxDistance;
        /// <summary>구간 피해량 (HP).</summary>
        public float Damage => _damage;
        /// <summary>구간 넉백 속도 (m/s).</summary>
        public float KnockbackSpeed => _knockbackSpeed;

        /// <summary>기본 구간을 생성한다. 거리 m, 피해 HP, 속도 m/s.</summary>
        public SplashBand(float maxDistance, float damage, float knockbackSpeed)
        {
            _maxDistance = maxDistance;
            _damage = damage;
            _knockbackSpeed = knockbackSpeed;
        }
    }

    /// <summary>연속 폭발의 외부 속도 합성 규칙.</summary>
    public enum KnockbackBlendMode { Additive, KeepStronger, AdditiveDamped }

    /// <summary>로켓과 폭발의 공통 수치를 보관한다.</summary>
    [CreateAssetMenu(menuName = "BoomPG/CombatConfig")]
    public class CombatConfig : ScriptableObject
    {
        [SerializeField] private float _rocketSpeed = 28f;
        [SerializeField] private float _rocketGravityScale = 0.35f;
        [SerializeField] private float _reloadSeconds = 2.2f;
        [SerializeField] private float _explosionRadius = 4.5f;
        [SerializeField] private float _directHitDamage = 35f;
        [SerializeField] private float _directHitKnockback = 14f;
        [SerializeField] private SplashBand[] _splashBands =
        {
            new SplashBand(1f, 20f, 18f),
            new SplashBand(2.5f, 12f, 13f),
            new SplashBand(4f, 6f, 8f),
            new SplashBand(4.5f, 3f, 4f)
        };
        [SerializeField] private float _knockbackUpMin = 0.35f;
        [SerializeField] private float _selfDamageRatio = 0.5f;
        [SerializeField] private float _selfKnockbackRatio = 1f;
        [SerializeField] private KnockbackBlendMode _blendMode = KnockbackBlendMode.Additive;
        [SerializeField] private float _maxKnockbackSpeed = 25f;

        /// <summary>로켓 초기 속도 (m/s).</summary>
        public float RocketSpeed => _rocketSpeed;
        /// <summary>로켓 중력 배율.</summary>
        public float RocketGravityScale => _rocketGravityScale;
        /// <summary>재장전 시간 (초).</summary>
        public float ReloadSeconds => _reloadSeconds;
        /// <summary>폭발 반경 (m).</summary>
        public float ExplosionRadius => _explosionRadius;
        /// <summary>직격 피해량 (HP).</summary>
        public float DirectHitDamage => _directHitDamage;
        /// <summary>직격 넉백 속도 (m/s).</summary>
        public float DirectHitKnockback => _directHitKnockback;
        /// <summary>거리 오름차순 스플래시 구간.</summary>
        public SplashBand[] SplashBands => _splashBands;
        /// <summary>재정규화 전 방향 y 하한 (비율).</summary>
        public float KnockbackUpMin => _knockbackUpMin;
        /// <summary>자가 피해 배율.</summary>
        public float SelfDamageRatio => _selfDamageRatio;
        /// <summary>자가 넉백 배율.</summary>
        public float SelfKnockbackRatio => _selfKnockbackRatio;
        /// <summary>외부 속도 합성 규칙.</summary>
        public KnockbackBlendMode BlendMode => _blendMode;
        /// <summary>가산 합성 속도 상한 (m/s).</summary>
        public float MaxKnockbackSpeed => _maxKnockbackSpeed;
    }
}
