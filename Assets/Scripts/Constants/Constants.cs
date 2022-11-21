using System.Collections.Generic;
using TowerDefense.Enemies;

namespace TowerDefense.Constants
{
    public static class Constants
    {
        
        public const float GRID_SIZE = 1f;

        public static Dictionary<EnemyType, int> ENEMY_PRIOS = new Dictionary<EnemyType, int>()
        {
            { EnemyType.Boss, 10 },
            { EnemyType.Fast, 5 },
            { EnemyType.Normal, 1 }
        };
        public const float TOWER_BOUNDARY_OFFSET = 0.1f;
        public const float TOWER_POSITIONING_HEIGHT = 2.5f;
        public const float TOWER_GAMEPLAY_HEIGHT = 1f;
        public const float TOWER_RAYCAST_LENGTH = 10f;

        public const float INPUT_RAYCAST_LENGTH = 50f;
        public const string INPUT_GAMEPLAY_MAP = "Gameplay";
        public const string INPUT_POSITIONING_MAP = "Tower Placement";

        public const string POOL_ID_NORMAL_ENEMY = "Enemy";
        public const string POOL_ID_NORMAL_TOWER = "Tower";
    }
}