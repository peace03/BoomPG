using BoomPG.Gameplay.Player;
using UnityEngine;

namespace BoomPG.Gameplay.World
{
    /// <summary>킬존 진입을 낙사로 전달한다.</summary>
    public class KillZoneTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.KillByRingOut(null);
            }
        }
    }
}
