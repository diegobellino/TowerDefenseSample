using UnityEngine;

namespace TowerDefense.Hordes
{
    public abstract class BaseSpawnBehaviour : ScriptableObject, ISpawnBehaviour
    {
        public abstract void UpdateBehaviour(float deltaTime, IHordeController hordeController);

        public abstract bool IsDone();
    }
}
