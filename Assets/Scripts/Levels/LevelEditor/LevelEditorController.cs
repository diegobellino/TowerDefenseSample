using System.Collections.Generic;
using JetBrains.Annotations;
using TowerDefense.GameLogic.Runtime;
using TowerDefense.GameLogic.Runtime.Configs;
using UnityEngine;

namespace TowerDefense.Levels.LevelEditor
{
    /// <summary>
    /// Level editor main controller, spawns and manages Objects to represent the level ongoing edition
    /// </summary>
    public class LevelEditorController : MonoBehaviour
    {
         
        private readonly Dictionary<int, GameObject> spawnerObjects = new();

        [SerializeField] private GameObject spawnerPrefab;
        [SerializeField] private GameObject castlePrefab;

        private GameObject castleObject;

        public void ResizeMap(Vector2Int size)
        {
            Debug.Log($"Resize map to {size}");
        }

        public void CreateCastle(Vector2Int position, int health)
        {
            if (castleObject != null)
            {
                return;
            }
            
            Debug.Log($"Create castle at {position} with health {health}");

            // This script is only for editor purposes, so some bad practices like this are allowed
            castleObject = Instantiate(castlePrefab, transform);
            castleObject.transform.position = new Vector3(position.x, 0, position.y);
        }

        public void ChangeCastleHealth(int health)
        {
            Debug.Log($"Changing castle health to {health}");
        }

        public void RepositionCastle(Vector2Int position)
        {
            Debug.Log($"Reposition castle to {position}");
            
            castleObject.transform.position = new Vector3(position.x, 0, position.y);
        }

        public void CreateSpawner(int spawnerId, Vector2Int position, [CanBeNull] HordeConfig config)
        {
            Debug.Log($"Create spawner {spawnerId} at {position} with config {config?.name}");

            spawnerObjects[spawnerId] = Instantiate(spawnerPrefab, transform);
            spawnerObjects[spawnerId].transform.position = new Vector3(position.x, 0, position.y);
        }

        public void DestroySpawner(int spawnerId)
        {
            Debug.Log($"Destroy spawner {spawnerId}");
            
            var spawner = spawnerObjects[spawnerId];
            spawnerObjects.Remove(spawnerId);
            
            DestroyImmediate(spawner);
        }

        public void RepositionSpawner(int spawnerId, Vector2Int position)
        {
            Debug.Log($"Reposition spawner {spawnerId} at {position}");

            spawnerObjects[spawnerId].transform.position = new Vector3(position.x, 0, position.y);
        }

        public void ChangeSpawnerConfig(int spawnerId, HordeConfig config)
        {
            Debug.Log($"Change spawner {spawnerId} config to {config.name}");
        }
    }
}
