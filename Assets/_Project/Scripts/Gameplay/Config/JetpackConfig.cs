using UnityEngine;

namespace BoomPG.Gameplay.Config
{
    /// <summary>플레이 검증에 사용할 설정값을 보관한다.</summary>
    [CreateAssetMenu(menuName = "BoomPG/JetpackConfig")]
    public class JetpackConfig : ScriptableObject
    {
        [SerializeField] private float _maxFuel = 100f;
        [SerializeField] private float _consumePerSecond = 25f;
        [SerializeField] private float _ascendSpeed = 6.5f;
        [SerializeField] private float _horizontalSpeed = 4f;
        [SerializeField] private float _refillDelay = 1.5f;
        [SerializeField] private float _refillPerSecond = 20f;
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
