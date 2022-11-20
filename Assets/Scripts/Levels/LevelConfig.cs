using TowerDefense.GameLogic.Runtime.Configs;
using UnityEngine;

namespace TowerDefense.Levels
{
    
    [CreateAssetMenu(fileName = "New level config", menuName = "Tower Defense/Levels/New Level")]
    public class LevelConfig : ScriptableObject
    {
        public Vector2Int mapSize = new Vector2Int(10, 10);
        
        public Vector2Int castlePosition;
        public int castleHealth;
        
        public Vector2Int[] hordeSpawnerLocations;
        public HordeConfig[] hordeConfigs;
    }
}