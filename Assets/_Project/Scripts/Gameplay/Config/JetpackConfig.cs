using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>플레이 검증에 사용할 설정값을 보관한다.</summary>
    [CreateAssetMenu(menuName = "BoomPG/JetpackConfig")]
    public class JetpackConfig : ScriptableObject
    {
        [Header("연료")]
        [Tooltip("연료 최대치. 아래 소모 속도와 함께 총 비행 시간을 결정한다 (100 / 25 = 4초).")]
        [SerializeField] private float _maxFuel = 100f;
        [Tooltip("추진 중 초당 연료 소모량. 키우면 비행 시간이 짧아져 '지금 쓸까 아껴둘까'의 압박이 커진다.")]
        [SerializeField] private float _consumePerSecond = 25f;
        [Header("비행 성능")]
        [Tooltip("추진 중 상승 속도 (m/s). 낙하 속도를 이겨야 복귀가 가능하다.")]
        [SerializeField] private float _ascendSpeed = 6.5f;
        [Tooltip("추진 중 공중 수평 이동 속도 (m/s). 비행 시간과 곱해 도달 거리가 나온다 (4 m/s × 4초 = 16 m). 맵의 발판 간격이 이 거리 안에 들어와야 복귀할 수 있다.")]
        [SerializeField] private float _horizontalSpeed = 4f;
        [Header("회복")]
        [Tooltip("착지 후 연료 회복이 시작되기까지의 대기 시간 (초). 늘리면 계속 도망 다니는 플레이가 억제된다.")]
        [SerializeField] private float _refillDelay = 1.5f;
        [Tooltip("초당 연료 회복량. 완충까지 걸리는 시간을 결정한다.")]
        [SerializeField] private float _refillPerSecond = 20f;
        [Header("피격 잠금")]
        [Tooltip("피격 직후 제트팩을 쓸 수 없는 시간 (초). 넉백당한 직후 곧바로 복귀하지 못하게 만드는 '떨어지는 공포'의 길이다. 줄이면 낙사가 크게 줄어든다.")]
        [SerializeField] private float _hitLockSeconds = 1f;

        /// <summary>MaxFuel 설정값 (연료 단위).</summary>
        public float MaxFuel => _maxFuel;

        /// <summary>ConsumePerSecond 설정값 (연료/초).</summary>
        public float ConsumePerSecond => _consumePerSecond;

        /// <summary>AscendSpeed 설정값 (m/s).</summary>
        public float AscendSpeed => _ascendSpeed;

        /// <summary>HorizontalSpeed 설정값 (m/s).</summary>
        public float HorizontalSpeed => _horizontalSpeed;

        /// <summary>RefillDelay 설정값 (초).</summary>
        public float RefillDelay => _refillDelay;

        /// <summary>RefillPerSecond 설정값 (연료/초).</summary>
        public float RefillPerSecond => _refillPerSecond;

        /// <summary>HitLockSeconds 설정값 (초).</summary>
        public float HitLockSeconds => _hitLockSeconds;
    }
}
