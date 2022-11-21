using System.Collections;
using System.Collections.Generic;
using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    public interface IHordeController
    {
        void Initialize(HordeConfig config);
        void UpdatePath(List<Vector3> newPath);
        void SpawnEnemy(EnemyType type);
    }
}
