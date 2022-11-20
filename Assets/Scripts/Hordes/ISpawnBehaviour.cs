using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Hordes
{
    public interface ISpawnBehaviour
    {
        void UpdateBehaviour(float deltaTime, IHordeController hordeController);
        bool IsDone();
    }
}
