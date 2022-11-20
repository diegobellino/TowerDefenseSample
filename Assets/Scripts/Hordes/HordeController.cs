using System;
using UnityEngine;
using System.Collections.Generic;
using TowerDefense.Enemies;

namespace TowerDefense.Hordes
{
    /// <summary>
    /// Spawns linked hordes when their respective conditions are met and updates them
    /// </summary>
    public class HordeController : MonoBehaviour, IHordeController
    {
        #region VARIABLES
        
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LineRenderer pathRenderer;

        private HordeConfig hordeConfig;
        private TowerDefense.ObjectPool.ObjectPool pool;
        private Queue<ISpawnBehaviour> spawnBehaviours;
        private ISpawnBehaviour currentBehaviour;
        private Dictionary<HordeConfig, List<IEnemy>> enemiesByHorde = new();
        private float timeSinceLastSpawned;
        
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

        public void UpdatePath(Vector3 startPosition, Vector3 endPosition)
        {
            pathRenderer.positionCount = 3;
            
            pathRenderer.SetPosition(0, startPosition);
            pathRenderer.SetPosition(1, new Vector3(endPosition.x, startPosition.y, startPosition.z));
            pathRenderer.SetPosition(2, endPosition);
        }

        public void SpawnEnemy(EnemyType type)
        {
            var enemyObject = pool.RetrieveObject(type.ToString());
            enemyObject.transform.position = spawnPoint.transform.position;
            enemyObject.SetActive(true);
        }
    }
}
