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

        public void ClearAll()
        {
            DestroyImmediate(castleObject);
            foreach (var spawner in spawnerObjects.Values)
            {
                DestroyImmediate(spawner.gameObject);
            }
            
            spawnerObjects.Clear();
            spawnerPositions.Clear();
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
            var newPosition = new Vector3(position.x, 0, position.y);
            castleObject.transform.position = newPosition;
            
            foreach (var spawnerId in spawnerObjects.Keys)
            {
                var path = GetPath(spawnerPositions[spawnerId], newPosition);
                spawnerObjects[spawnerId].UpdatePath(path);
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
                var path = GetPath(spawnerPositions[spawnerId], newPosition);
                spawnerObjects[spawnerId].UpdatePath(path);
            }
        }

        public void CreateSpawner(int spawnerId, Vector2Int position, [CanBeNull] HordeConfig config)
        {
            Debug.Log($"Create spawner {spawnerId} at {position} with config {config?.name}");
            var newPosition = new Vector3(position.x, 0, position.y);
            spawnerPositions[spawnerId] = newPosition;
            
            spawnerObjects[spawnerId] = Instantiate(spawnerPrefab, transform).GetComponent<HordeController>();
            spawnerObjects[spawnerId].transform.position = newPosition;

            var path = GetPath(newPosition, castleObject.transform.position);
            spawnerObjects[spawnerId].UpdatePath(path);

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
            var path = GetPath(newPosition, castleObject.transform.position);
            spawnerObjects[spawnerId].UpdatePath(path);
        }

        public void ChangeSpawnerConfig(int spawnerId, HordeConfig config)
        {
            Debug.Log($"Change spawner {spawnerId} config to {config.name}");
            spawnerObjects[spawnerId].Initialize(config);
        }

        private List<Vector3> GetPath(Vector3 startPosition, Vector3 endPosition)
        {
            var path = new List<Vector3>();
            path.Add(startPosition);
            var pathFound = false;
            var isHorizontalSearch = true;
            var curPathPoint = startPosition;

            while (!pathFound)
            {
                var objectivePoint = isHorizontalSearch ? 
                    curPathPoint + Vector3.right * (endPosition.x - curPathPoint.x): 
                    curPathPoint + Vector3.forward * (endPosition.z - curPathPoint.z);

                curPathPoint = objectivePoint;
                path.Add(curPathPoint);

                isHorizontalSearch = !isHorizontalSearch;
                pathFound = curPathPoint == endPosition;
            }
            return path;
        }
    }
}
