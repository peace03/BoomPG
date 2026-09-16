using System;
using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>거리 구간별 스플래시 수치.</summary>
    [Serializable]
    public struct SplashBand
    {
        [Tooltip("이 구간이 적용되는 폭심으로부터의 상한 거리 (m). 거리가 이 값 이하면 아래 피해·넉백이 적용된다. 배열은 반드시 오름차순이어야 한다.")]
        [SerializeField] private float _maxDistance;
        [Tooltip("이 구간의 피해량 (HP). 플레이어 체력은 100이다.")]
        [SerializeField] private float _damage;
        [Tooltip("이 구간의 넉백 속도 (m/s). 폭심에 가까울수록 커야 '발밑 조준'이 의미를 갖는다.")]
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
        [Header("로켓")]
        [Tooltip("로켓의 초기 비행 속도 (m/s). 낮을수록 예측 조준이 필요해져 실력 차이가 드러난다. 높이면 맞히기 쉬워지는 대신 회피가 어려워진다.")]
        [SerializeField] private float _rocketSpeed = 28f;
        [Tooltip("로켓에 적용되는 중력 배율. 0이면 직선으로 날아가고, 높일수록 포물선이 가팔라져 원거리 조준이 어려워진다.")]
        [Range(0f, 2f)]
        [SerializeField] private float _rocketGravityScale = 0.35f;
        [Tooltip("재장전 시간 (초). 줄이면 교전 템포가 빨라지지만 한 발의 무게감과 빗나갔을 때의 처벌이 약해진다.")]
        [SerializeField] private float _reloadSeconds = 2.2f;
        [Header("폭발 · 직격")]
        [Tooltip("폭발 판정 반경 (m). 이 안에 있는 대상이 스플래시를 받는다. 넓히면 빗나가도 효과가 닿아 쉬워지고, 좁히면 정확도 요구가 올라간다.")]
        [SerializeField] private float _explosionRadius = 4.5f;
        [Tooltip("로켓이 몸에 직접 맞았을 때의 피해량 (HP). 체력 100 기준 35면 3방 처치다. 40으로 올리면 스플래시 한 번을 섞어도 처치가 되어 템포가 빨라진다.")]
        [SerializeField] private float _directHitDamage = 35f;
        [Tooltip("직격 시 넉백 속도 (m/s). 근접 스플래시(18)보다 낮게 두어야 '죽일까(직격) vs 떨어뜨릴까(발밑)'의 선택이 성립한다.")]
        [SerializeField] private float _directHitKnockback = 14f;
        [Header("스플래시 구간 (거리 오름차순)")]
        [Tooltip("폭심 거리에 따른 피해·넉백 표. 위에서부터 순서대로 검사해 거리가 상한 이하인 첫 구간을 적용한다. 반드시 거리 오름차순으로 유지할 것.")]
        [SerializeField] private SplashBand[] _splashBands =
        {
            new SplashBand(1f, 20f, 18f),
            new SplashBand(2.5f, 12f, 13f),
            new SplashBand(4f, 6f, 8f),
            new SplashBand(4.5f, 3f, 4f)
        };
        [Header("넉백 방향")]
        [Tooltip("넉백 방향의 y 성분 하한 (비율). 항상 살짝 위로 띄워 미끄러지듯 밀리게 한다. 높이면 위로 크게 떠서 공중 체류가 길어지고, 0이면 지면을 따라 밀린다.")]
        [Range(0f, 1f)]
        [SerializeField] private float _knockbackUpMin = 0.35f;
        [Header("로켓 점프 (자가 피해)")]
        [Tooltip("자기 폭발에 받는 피해 비율. 0.5면 스플래시 피해의 절반을 받는다. 로켓 점프를 남발하면 올려서 억제한다.")]
        [Range(0f, 1f)]
        [SerializeField] private float _selfDamageRatio = 0.5f;
        [Tooltip("자기 폭발에 받는 넉백 비율. 1.0이면 남에게 주는 것과 같은 힘으로 자신이 밀린다. 올리면 로켓 점프 도달 거리가 늘어난다.")]
        [Range(0f, 2f)]
        [SerializeField] private float _selfKnockbackRatio = 1f;
        [Header("넉백 합성 (D-022)")]
        [Tooltip("폭발이 겹쳤을 때 기존 넉백과 새 넉백을 합치는 규칙. Additive=합산 후 상한 제한(반대 방향은 상쇄), KeepStronger=더 강한 쪽만 유지, AdditiveDamped=기존을 절반으로 줄인 뒤 합산. 대입 방식이 아니어야 약한 폭발이 강한 넉백을 지우지 않는다.")]
        [SerializeField] private KnockbackBlendMode _blendMode = KnockbackBlendMode.Additive;
        [Tooltip("합성된 넉백 속도의 상한 (m/s). 낮추면 연쇄 폭발로 과도하게 날아가는 것을 막지만 극적인 장면도 줄어든다. 단일 최대 넉백은 18 m/s다.")]
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
