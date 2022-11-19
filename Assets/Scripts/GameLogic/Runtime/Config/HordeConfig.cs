using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.GameLogic.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Tower Defense/Horde Config", fileName = "New Horde")]
    public class HordeConfig : ScriptableObject
    {
        #region VARIABLES

        private enum SpawnCondition
        {
            AfterTime,
            AfterHordeDefeated
        }

        public GameObject[] Enemies => enemies;
        public float TimeBetweenSpawns => timeBetweenSpawns;

        [SerializeField] private SpawnCondition spawnCondition;
        [SerializeField] private float timeToSpawn;
        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private HordeConfig spawnAfterDefeated;
        [SerializeField] private GameObject[] enemies;

        #endregion

        public bool ShouldSpawn(float elapsedTime, HashSet<HordeConfig> defeatedHordes)
        {
            switch (spawnCondition)
            {
                case SpawnCondition.AfterTime:
                    return elapsedTime >= timeToSpawn;

                case SpawnCondition.AfterHordeDefeated:
                    foreach (var horde in defeatedHordes)
                    {
                        if (horde == spawnAfterDefeated)
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

    }
}
