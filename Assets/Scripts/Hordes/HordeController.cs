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
        public int HordeCount => hordeConfig.GetEnemyCount();

        
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LineRenderer pathRenderer;

        private HordeConfig hordeConfig;
        private Queue<ISpawnBehaviour> spawnBehaviours;
        private ISpawnBehaviour currentBehaviour;

        private List<Vector3> path;

        private bool active;

        #endregion

        #region LIFETIME

        public void Initialize(HordeConfig config)
        {
            hordeConfig = config;
            
            spawnBehaviours = hordeConfig.SpawnBehaviourQueue;
            currentBehaviour = spawnBehaviours.Dequeue();

            active = true;
        }
        
        public void UpdateController(float deltaTime)
        {
            if (!active)
            {
                return;
            }
            
            if (currentBehaviour == null)
            {
                if (spawnBehaviours.Count == 0)
                {
                    active = false;
                    return;
                }
                
                currentBehaviour = spawnBehaviours.Dequeue();
            }
            
            currentBehaviour.UpdateBehaviour(deltaTime, this);

            if (currentBehaviour.IsDone())
            {
                currentBehaviour = null;
            }
        }

        #endregion

        public void UpdatePath(List<Vector3> newPath)
        {
            path = newPath;
            pathRenderer.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                var position = path[i];
                position.y = .1f;
                pathRenderer.SetPosition(i, position);
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
