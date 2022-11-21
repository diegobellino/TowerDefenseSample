using UnityEngine;
using Utils.Interfaces;

namespace TowerDefense.Enemies
{
    [System.Serializable]
    public enum EnemyType
    {
        Normal,
        Fast,
        Boss
    }

    public interface IEnemy : IDamageable, IPoolable
    {
        EnemyType Type { get; }
        
        bool shouldMove { set; }

        bool IsTargeteable();

        bool IsDead();

        Vector3 GetPosition();

        void ResetValues();

        void Slow();
    }
}
