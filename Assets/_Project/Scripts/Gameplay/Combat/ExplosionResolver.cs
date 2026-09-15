using System.Collections.Generic;
using BoomPG.Core.Logging;
using BoomPG.Gameplay.Config;
using BoomPG.Gameplay.Player;
using UnityEngine;

namespace BoomPG.Gameplay.Combat
{
    /// <summary>차폐와 직격, 자가 배율을 적용해 폭발을 처리한다.</summary>
    public static class ExplosionResolver
    {
        private static readonly int TargetMask = LayerMask.GetMask("Player");
        private static readonly int BlockMask = LayerMask.GetMask("Platform");

        /// <summary>월드 폭심(m), 발사자, 직격 대상 및 전투 설정으로 폭발을 처리한다.</summary>
        public static void Resolve(Vector3 center, GameObject owner, GameObject directHitTarget,
            CombatConfig config)
        {
            if (config == null)
            {
                GameLog.Error("Combat", "폭발 전투 설정 누락");
                return;
            }

            Collider[] targets = Physics.OverlapSphere(center, config.ExplosionRadius, TargetMask);
            var processed = new HashSet<GameObject>();
            foreach (Collider collider in targets)
            {
                GameObject target = collider.gameObject;
                if (!processed.Add(target) ||
                    Physics.Linecast(center, collider.bounds.center, BlockMask))
                {
                    continue;
                }

                PlayerMotor motor = target.GetComponent<PlayerMotor>();
                HealthComponent health = target.GetComponent<HealthComponent>();
                if (motor == null || health == null || health.IsDead)
                {
                    continue;
                }

                float damage;
                float knockbackSpeed;
                if (target == directHitTarget)
                {
                    damage = config.DirectHitDamage;
                    knockbackSpeed = config.DirectHitKnockback;
                }
                else if (!KnockbackCalculator.TryGetSplash(config,
                    Vector3.Distance(center, target.transform.position), out damage, out knockbackSpeed))
                {
                    continue;
                }

                if (target == owner)
                {
                    damage *= config.SelfDamageRatio;
                    knockbackSpeed *= config.SelfKnockbackRatio;
                }

                Vector3 direction = KnockbackCalculator.GetDirection(center,
                    target.transform.position, config.KnockbackUpMin);
                health.ApplyDamage(damage, owner);
                motor.ApplyKnockback(direction * knockbackSpeed);
                GameLog.Combat($"{target.name} 데미지 {damage} 넉백 {knockbackSpeed:F1} m/s");
            }
        }
    }
}
