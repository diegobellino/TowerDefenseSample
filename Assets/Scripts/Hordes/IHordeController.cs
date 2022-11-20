using System.Collections;
using System.Collections.Generic;
using TowerDefense.Enemies;
using UnityEngine;

namespace TowerDefense.Hordes
{
    public interface IHordeController
    {
        void SpawnEnemy(EnemyType type);
    }
}
