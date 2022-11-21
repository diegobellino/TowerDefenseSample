using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Enemies;
using Utils.Interfaces;

namespace TowerDefense.Hordes
{
    /// <summary>
    /// Spawns linked hordes when their respective conditions are met and updates them
    /// </summary>
    public class HordeController : MonoBehaviour, IHordeController, IPoolable
    {
        #region VARIABLES
        
        public IPool Pool { private get; set; }
        public string PoolId => nameof(HordeController);
        
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LineRenderer pathRenderer;

        private HordeConfig hordeConfig;
        private Queue<ISpawnBehaviour> spawnBehaviours;
        private ISpawnBehaviour currentBehaviour;

        private List<Vector3> path;

        public int HordeCount => hordeConfig.GetEnemyCount();

        #endregion

        #region LIFETIME

        private void Awake()
        {
            spawnBehaviours = hordeConfig.SpawnBehaviourQueue;
            currentBehaviour = spawnBehaviours.Dequeue();
        }

        public void UpdateController(float deltaTime)
        {
            if (currentBehaviour == null)
            {
                currentBehaviour = spawnBehaviours.Dequeue();
            }
            
            currentBehaviour.UpdateBehaviour(deltaTime, this);

            if (currentBehaviour.IsDone())
            {
                currentBehaviour = null;
            }
        }

        #endregion

        public void Initialize(HordeConfig config)
        {
            hordeConfig = config;
        }

        public void UpdatePath(List<Vector3> newPath)
        {
            path = newPath;
            pathRenderer.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                pathRenderer.SetPosition(i, path[i]);
            }
        }

        public void SpawnEnemy(EnemyType type)
        {
            var enemyObject = Pool.RetrieveObject(type.ToString());
            enemyObject.transform.position = spawnPoint.transform.position;
            enemyObject.SetActive(true);
        }
    }
}
