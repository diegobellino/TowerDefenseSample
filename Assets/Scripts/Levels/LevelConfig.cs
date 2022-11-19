using UnityEngine;

namespace TowerDefense.Levels
{
    
    [CreateAssetMenu(fileName = "New level config", menuName = "Tower Defense/Levels/New Level")]
    public class LevelConfig : ScriptableObject
    {
        private Vector2 mapSize;
        private Vector2[] hordeSpawnerLocations;
        private Vector2 castleLocation;

        public Vector2 MapSize => mapSize;

    }
}