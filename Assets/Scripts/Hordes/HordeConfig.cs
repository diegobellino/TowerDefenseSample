using UnityEngine;

namespace TowerDefense.Hordes
{
    [CreateAssetMenu(menuName = "Tower Defense/Hordes/Horde Config", fileName = "New Horde")]
    public class HordeConfig : ScriptableObject
    {
        [SerializeField] private BaseSpawnBehaviour[] spawnBehaviours;

        public BaseSpawnBehaviour[] SpawnBehaviourArray => spawnBehaviours;

        public int GetEnemyCount()
        {
            var count = 0;
            foreach (var spawnBehaviour in spawnBehaviours)
            {
                if (spawnBehaviour is SpawnEnemyBehaviour)
                {
                    count++;
                }
            }

            return count;
        }

    }
}
