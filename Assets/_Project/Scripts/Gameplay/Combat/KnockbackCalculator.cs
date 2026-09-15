using BoomPG.Gameplay.Config;
using UnityEngine;

namespace BoomPG.Gameplay.Combat
{
    /// <summary>씬 상태에 의존하지 않는 넉백 계산.</summary>
    public static class KnockbackCalculator
    {
        /// <summary>거리(m)에 해당하는 첫 구간의 피해(HP)와 속도(m/s)를 반환한다.</summary>
        public static bool TryGetSplash(CombatConfig config, float distance,
            out float damage, out float knockbackSpeed)
        {
            damage = 0f;
            knockbackSpeed = 0f;
            if (config == null || config.SplashBands == null)
            {
                return false;
            }

            foreach (SplashBand band in config.SplashBands)
            {
                if (distance <= band.MaxDistance)
                {
                    damage = band.Damage;
                    knockbackSpeed = band.KnockbackSpeed;
                    return true;
                }
            }

            return false;
        }

        /// <summary>월드 위치(m) 두 개와 y 하한 비율로 단위 넉백 방향을 구한다.</summary>
        public static Vector3 GetDirection(Vector3 explosionPos, Vector3 targetPos, float minUpward)
        {
            Vector3 dir = (targetPos - explosionPos).normalized;
            if (dir == Vector3.zero)
            {
                return Vector3.up;
            }

            dir.y = Mathf.Max(dir.y, minUpward);
            return dir.normalized;
        }

        /// <summary>기존/신규 속도를 합성한다. 속도와 상한의 단위는 m/s.</summary>
        public static Vector3 Blend(Vector3 current, Vector3 incoming,
            KnockbackBlendMode mode, float maxSpeed)
        {
            switch (mode)
            {
                case KnockbackBlendMode.KeepStronger:
                    return incoming.sqrMagnitude > current.sqrMagnitude ? incoming : current;
                case KnockbackBlendMode.AdditiveDamped:
                    return Vector3.ClampMagnitude(current * 0.5f + incoming, maxSpeed);
                default:
                    return Vector3.ClampMagnitude(current + incoming, maxSpeed);
            }
        }
    }
}
