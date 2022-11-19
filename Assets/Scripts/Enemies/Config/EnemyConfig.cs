using UnityEngine;

namespace TowerDefense.Enemies.Config
{

    [CreateAssetMenu(menuName = "Tower Defense/Enemies/Enemy Config", fileName = "New Enemy")]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private EnemyType type;
        [SerializeField] float maxHealth = 100f;
        [SerializeField, Range(0.5f, 2.5f)] float speed = 1f;
        [SerializeField] int currencyDrop = 20;
        [SerializeField] float damage = 20f;
        public EnemyType Type => type;
        public float MaxHealth => maxHealth;
        public float Speed => speed;
        public float Damage => damage;
    }
}