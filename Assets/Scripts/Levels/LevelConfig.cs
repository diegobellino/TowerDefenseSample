using TowerDefense.GameLogic.Runtime.Configs;
using UnityEngine;

namespace TowerDefense.Levels
{
    
    [CreateAssetMenu(fileName = "New level config", menuName = "Tower Defense/Levels/New Level")]
    public class LevelConfig : ScriptableObject
    {
        public Vector2 mapSize;
        
        public Vector2 castlePosition;
        public int castleHealth;
        
        public Vector2[] hordeSpawnerLocations;
        public HordeConfig[] hordeConfigs;
    }
}