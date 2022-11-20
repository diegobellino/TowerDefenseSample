using System.Collections.Generic;
using JetBrains.Annotations;
using TowerDefense.GameLogic.Runtime;
using TowerDefense.GameLogic.Runtime.Configs;
using UnityEngine;

namespace TowerDefense.Levels.LevelEditor
{
    public class LevelEditorController : MonoBehaviour
    {
         
        private Dictionary<int, HordeController> spawners;

        public void ResizeMap(Vector2Int size)
        {
            Debug.Log($"Resize map to {size}");
        }

        public void CreateCastle(Vector2Int position, int health)
        {
            Debug.Log($"Create castle at {position} with health {health}");

        }

        public void ChangeCastleHealth(int health)
        {
            Debug.Log($"Changing castle health to {health}");
        }

        public void RepositionCastle(Vector2Int position)
        {
            Debug.Log($"Reposition castle to {position}");
        }

        public void CreateSpawner(int spawnerId, Vector2Int position, [CanBeNull] HordeConfig config)
        {
            Debug.Log($"Create spawner {spawnerId} at {position} with config {config?.name}");

        }

        public void DestroySpawner(int spawnerId)
        {
            Debug.Log($"Destroy spawner {spawnerId}");

        }

        public void RepositionSpawner(int spawnerId, Vector2Int position)
        {
            Debug.Log($"Reposition spawner {spawnerId} at {position}");
        }

        public void ChangeSpawnerConfig(int spawnerId, HordeConfig config)
        {
            Debug.Log($"Change spawner {spawnerId} config to {config.name}");
        }
    }
}
