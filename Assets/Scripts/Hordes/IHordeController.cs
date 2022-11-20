using System.Collections;
using System.Collections.Generic;
using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    public interface IHordeController
    {
        void Initialize(HordeConfig config);
        void UpdatePath(Vector3 startPosition, Vector3 endPosition);
        void SpawnEnemy(EnemyType type);
    }
}
