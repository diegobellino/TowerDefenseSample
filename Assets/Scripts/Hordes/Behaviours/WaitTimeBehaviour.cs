using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    [CreateAssetMenu(menuName = "Tower Defense/Hordes/Wait Time Behaviour", fileName = "New Wait Time Behaviour")]
    public class WaitTimeBehaviour : BaseSpawnBehaviour
    {
        [SerializeField] private float amount;

        private float elapsedTime;

        public override void UpdateBehaviour(float deltaTime, IHordeController hordeController)
        {
            elapsedTime = deltaTime;
        }

        public override bool IsDone()
        {
            return elapsedTime >= amount;
        }
    }
}
