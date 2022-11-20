using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    [CreateAssetMenu(menuName = "Tower Defense/Hordes/Spawn Enemy Behaviour", fileName = "New Spawn Enemy Behaviour")]
    public class SpawnEnemyBehaviour : BaseSpawnBehaviour
    {
        [SerializeField] private EnemyType enemyType;

        private bool didSpawn;
        private bool isDone;

        private float elapsedTime;
        
        public override void UpdateBehaviour(float deltaTime, IHordeController hordeController)
        {
            elapsedTime += deltaTime;

            if (didSpawn)
            {
                return;
            }
            
            hordeController.SpawnEnemy(enemyType);
        }

        public override bool IsDone()
        {
            return elapsedTime >= 2f;
        }
    }
}
