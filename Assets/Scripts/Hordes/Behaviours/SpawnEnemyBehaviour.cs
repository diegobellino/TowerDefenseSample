using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    [CreateAssetMenu(menuName = "Tower Defense/Hordes/Spawn Enemy Behaviour", fileName = "New Spawn Enemy Behaviour")]
    public class SpawnEnemyBehaviour : BaseSpawnBehaviour
    {
        [SerializeField] private EnemyType enemyType;

        public override void UpdateBehaviour(float deltaTime, IHordeController hordeController)
        {
            hordeController.SpawnEnemy(enemyType);
            Debug.Log($"{enemyType} spawned!");
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}
