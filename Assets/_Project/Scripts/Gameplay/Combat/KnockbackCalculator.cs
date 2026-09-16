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

        /// <summary>
        /// 넉백 중 이동 입력을 날아가는 방향의 좌우 성분으로만 제한한다.
        /// 날아가는 축으로는 가속·감속이 불가능해지고, 착지 지점만 조정할 수 있다.
        /// </summary>
        /// <param name="moveDirection">카메라 기준으로 변환된 월드 XZ 이동 입력 (단위 벡터).</param>
        /// <param name="knockbackVelocity">현재 외부 속도 (m/s). y 성분은 무시한다.</param>
        /// <returns>좌우 축 성분만 남긴 이동 방향. 넉백의 수평 성분이 없으면 입력을 그대로 돌려준다.</returns>
        public static Vector3 ProjectLateralInput(Vector3 moveDirection, Vector3 knockbackVelocity)
        {
            Vector3 horizontalKnockback = new Vector3(knockbackVelocity.x, 0f, knockbackVelocity.z);
            if (horizontalKnockback.sqrMagnitude < 1e-6f)
            {
                return moveDirection;
            }

            Vector3 lateralAxis = Vector3.Cross(Vector3.up, horizontalKnockback.normalized);
            return lateralAxis * Vector3.Dot(moveDirection, lateralAxis);
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
