using UnityEngine;
using System.Collections.Generic;
using TowerDefense.GameLogic.Runtime.Configs;
using TowerDefense.Enemies;
namespace TowerDefense.GameLogic.Runtime
{
    /// <summary>
    /// Spawns linked hordes when their respective conditions are met and updates them
    /// </summary>
    public class HordeController : MonoBehaviour
    {
        #region VARIABLES

        public int HordeCount => hordes.Length;
        public int DefeatedHordes => defeatedHordes.Count;

        [SerializeField] private Transform SpawnPoint;
        [SerializeField] private HordeConfig[] hordes;
        [SerializeField] private ObjectPool pool;
        
        private HashSet<HordeConfig> defeatedHordes = new HashSet<HordeConfig>();
        private HashSet<HordeConfig> spawnedHordes = new HashSet<HordeConfig>();
        private Dictionary<HordeConfig, List<IEnemy>> enemiesByHorde = new Dictionary<HordeConfig, List<IEnemy>>();
        private Dictionary<HordeConfig, float> timeSinceLastSpawnedByHorde = new Dictionary<HordeConfig, float>();

        #endregion

        #region LIFETIME

        public void UpdateHordes(float deltaTime)
        {
            foreach (var horde in hordes)
            {
                if (ShouldSpawnHorde(horde))
                {
                    if (!timeSinceLastSpawnedByHorde.TryGetValue(horde, out var timeSinceLastSpawned))
                    {
                        timeSinceLastSpawned = 0f;
                        timeSinceLastSpawnedByHorde.Add(horde, timeSinceLastSpawned);
                    }

                    timeSinceLastSpawned += deltaTime;

                    var hordeFinishedSpawning = false;
                    if (timeSinceLastSpawned >= horde.TimeBetweenSpawns)
                    {
                        if (!enemiesByHorde.ContainsKey(horde))
                        {
                            enemiesByHorde.Add(horde, new List<IEnemy>());
                        }
                        var index = enemiesByHorde[horde].Count;
                        var enemyObject = pool.RetrieveObject(horde.Enemies[index].GetComponent<IEnemy>().PoolId);
                        enemyObject.transform.position = SpawnPoint.position;
                        enemyObject.transform.rotation = SpawnPoint.rotation;

                        var enemy = enemyObject.GetComponent<IEnemy>();
                        enemy.ResetValues();
                        enemy.shouldMove = true;

                        enemiesByHorde[horde].Add(enemy);
                        enemyObject.SetActive(true);

                        timeSinceLastSpawned = 0f;

                        hordeFinishedSpawning = enemiesByHorde[horde].Count == horde.Enemies.Length;
                    }

                    if (hordeFinishedSpawning)
                    {
                        spawnedHordes.Add(horde);
                        timeSinceLastSpawnedByHorde.Remove(horde);
                    } 
                    else
                    {
                        timeSinceLastSpawnedByHorde[horde] = timeSinceLastSpawned;
                    }
                }

                if (ShouldMarkHordeDefeated(horde))
                {
                    defeatedHordes.Add(horde);
                    enemiesByHorde.Remove(horde);
                }
            }
        }

        #endregion

        private bool ShouldSpawnHorde(HordeConfig horde)
        {
            if (spawnedHordes.Contains(horde))
            {
                return false;
            }

            return horde.ShouldSpawn(defeatedHordes);
        }

        private bool ShouldMarkHordeDefeated(HordeConfig horde)
        {
            if (defeatedHordes.Contains(horde) || !spawnedHordes.Contains(horde))
            {
                return false;
            }

            var hordeDefeated = true;
            var enemies = enemiesByHorde[horde];

            foreach (var enemy in enemies)
            {
                hordeDefeated &= enemy.IsDead();

                if (!hordeDefeated)
                {
                    return false;
                }
            }

            return true;
        }

        public bool NoHordesLeft()
        {
            return spawnedHordes.Count == hordes.Length && 
                defeatedHordes.Count == hordes.Length;
        }
    }
}
