using System.Collections.Generic;
using UnityEngine;
using TowerDefense.Enemies;

namespace TowerDefense.GameLogic.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Tower Defense/Horde Config", fileName = "New Horde")]
    public class HordeConfig : ScriptableObject
    {
        #region VARIABLES

        public GameObject[] Enemies => enemies;
        public float TimeBetweenSpawns => timeBetweenSpawns;

        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private HordeConfig spawnAfterDefeated;
        [SerializeField] private GameObject[] enemies;

        #endregion

        public bool ShouldSpawn(HashSet<HordeConfig> defeatedHordes)
        {
            foreach (var horde in defeatedHordes)
            {
                if (horde == spawnAfterDefeated)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
