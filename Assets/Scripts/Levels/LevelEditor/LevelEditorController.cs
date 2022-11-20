using System.Buffers;
using System.Collections.Generic;
using JetBrains.Annotations;
using TowerDefense.Hordes;
using UnityEngine;

namespace TowerDefense.Levels.LevelEditor
{
    /// <summary>
    /// Level editor main controller, spawns and manages Objects to represent the level ongoing edition
    /// </summary>
    public class LevelEditorController : MonoBehaviour
    {
         
        private readonly Dictionary<int, HordeController> spawnerObjects = new();
        private readonly Dictionary<int, Vector3> spawnerPositions = new();

        [SerializeField] private GameObject spawnerPrefab;
        [SerializeField] private GameObject castlePrefab;

        private GameObject castleObject;

        public void CreateCastle(Vector2Int position, int health)
        {
            if (castleObject != null)
            {
                return;
            }
            
            Debug.Log($"Create castle at {position} with health {health}");

            // This script is only for editor purposes, so some bad practices like this are allowed
            castleObject = Instantiate(castlePrefab, transform);
            var newPosition = new Vector3(position.x, 0, position.y);
            castleObject.transform.position = newPosition;
            
            foreach (var spawnerId in spawnerObjects.Keys)
            {
                spawnerObjects[spawnerId].UpdatePath(spawnerPositions[spawnerId], newPosition);
            }
        }

        public void ChangeCastleHealth(int health)
        {
            Debug.Log($"Changing castle health to {health}");
        }

        public void RepositionCastle(Vector2Int position)
        {
            Debug.Log($"Reposition castle to {position}");

            var newPosition = new Vector3(position.x, 0, position.y);
            castleObject.transform.position = newPosition;

            foreach (var spawnerId in spawnerObjects.Keys)
            {
                spawnerObjects[spawnerId].UpdatePath(spawnerPositions[spawnerId], newPosition);
            }
        }

        public void CreateSpawner(int spawnerId, Vector2Int position, [CanBeNull] HordeConfig config)
        {
            Debug.Log($"Create spawner {spawnerId} at {position} with config {config?.name}");
            var newPosition = new Vector3(position.x, 0, position.y);
            spawnerPositions[spawnerId] = newPosition;
            
            spawnerObjects[spawnerId] = Instantiate(spawnerPrefab, transform).GetComponent<HordeController>();
            spawnerObjects[spawnerId].transform.position = newPosition;

            spawnerObjects[spawnerId].UpdatePath(newPosition, castleObject.transform.position);

            if (config != null)
            {
                spawnerObjects[spawnerId].Initialize(config);
            }
        }

        public void DestroySpawner(int spawnerId)
        {
            Debug.Log($"Destroy spawner {spawnerId}");
            
            var spawner = spawnerObjects[spawnerId];
            spawnerObjects.Remove(spawnerId);
            spawnerPositions.Remove(spawnerId);
            
            DestroyImmediate(spawner.gameObject);
        }

        public void RepositionSpawner(int spawnerId, Vector2Int position)
        {
            Debug.Log($"Reposition spawner {spawnerId} at {position}");
            var newPosition = new Vector3(position.x, 0, position.y);
            spawnerPositions[spawnerId] = newPosition;

            spawnerObjects[spawnerId].transform.position = newPosition;
            spawnerObjects[spawnerId].UpdatePath(newPosition, castleObject.transform.position);
        }

        public void ChangeSpawnerConfig(int spawnerId, HordeConfig config)
        {
            Debug.Log($"Change spawner {spawnerId} config to {config.name}");
            spawnerObjects[spawnerId].Initialize(config);
        }
    }
}
