using TowerDefense.States;
using UnityEngine;

namespace TowerDefense.Towers.Config
{ 

    [CreateAssetMenu(menuName = "Tower Defense/Towers/Tower Config", fileName = "New Tower")]
    public class TowerConfig: ScriptableObject
    {
        [Header("Persistent config")]
        [SerializeField] private TowerType towerType;
        [SerializeField] private int baseCost;
        [SerializeField] private int maxLevel;
        [SerializeField] private Vector2 size;

        [Header("Level dependent config")]
        [SerializeField] private int[] upgradeCostPerLevel;
        [SerializeField] private int[] rangePerLevel;
        [SerializeField] private int[] damagePerLevel;
        [SerializeField] private float[] attackRatePerLevel;
        [SerializeField] private Mesh[] meshesPerLevel;
        [SerializeField] private Material[] materialsPerLevel;

        public TowerType TowerType => towerType;
        public int BaseCost => baseCost;
        public int MaxLevel => maxLevel;
        public Vector2 Size => size;
        
        public int GetUpgradeCost(int level)
        {
            return upgradeCostPerLevel[Mathf.Clamp(level, 0, upgradeCostPerLevel.Length)];
        }

        public int GetRange(int level)
        {
            return rangePerLevel[Mathf.Clamp(level, 0, rangePerLevel.Length)];
        }

        public int GetDamage(int level)
        {
            return damagePerLevel[Mathf.Clamp(level, 0, damagePerLevel.Length)];
        }

        public float GetAttackRate(int level)
        {
            return attackRatePerLevel[Mathf.Clamp(level, 0, attackRatePerLevel.Length)];
        }

        public Mesh GetMesh(int level)
        {
            return meshesPerLevel[Mathf.Clamp(level, 0, meshesPerLevel.Length)];
        }

        public Material GetMaterial(int level)
        {
            return materialsPerLevel[Mathf.Clamp(level, 0, materialsPerLevel.Length)];
        }
    }
}