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

        public int HordeCount => hordeConfig.GetEnemyCount();
        [SerializeField] private Transform spawnPoint;
        [SerializeField] HordeConfig hordeConfig;
        [SerializeField] private TowerDefense.ObjectPool.ObjectPool pool;

        private Queue<ISpawnBehaviour> spawnBehaviours;
        private ISpawnBehaviour currentBehaviour;
        private Dictionary<HordeConfig, List<IEnemy>> enemiesByHorde = new();
        private float timeSinceLastSpawned;

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

        public void SpawnEnemy(EnemyType type)
        {
            var enemyObject = pool.RetrieveObject(type.ToString());
            enemyObject.transform.position = spawnPoint.transform.position;
            enemyObject.SetActive(true);
        }
    }
}
