using TowerDefense.Hordes;
using UnityEngine;

namespace TowerDefense.Levels
{
    
    [CreateAssetMenu(fileName = "New level config", menuName = "Tower Defense/Levels/New Level")]
    public class LevelConfig : ScriptableObject
    {
        public Vector2Int castlePosition;
        public int castleHealth;
        
        public Vector2Int[] hordeSpawnerLocations;
        public HordeConfig[] hordeConfigs;
    }
}