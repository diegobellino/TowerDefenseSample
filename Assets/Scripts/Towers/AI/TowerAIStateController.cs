using System.Collections.Generic;
using UnityEngine;
using Utils.PluggableAI;
using TowerDefense.Enemies;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.Towers.AI
{
    public enum AttackType
    {
        Damage,
        Slow
    }

    public class TowerAIStateController : PluggableAIStateController
    {
        #region VARIABLES

        public int Range { get; private set; }
        public int Damage { get; private set; }
        public float AttackRate { get; private set; }
        public HashSet<IEnemy> EnemiesInRange = new HashSet<IEnemy>();
        public IEnemy TargetEnemy;

        [SerializeField] private SphereCollider rangeTrigger;

        private bool canAttack;
        private float elapsedTime;

        #endregion

        #region LIFETIME

        public override void SmartUpdate(float deltaTime)
        {
            base.SmartUpdate(deltaTime);

            elapsedTime += deltaTime;

            if (elapsedTime >= AttackRate)
            {
                canAttack = true;
                elapsedTime = 0f;
            }
            else
            {
                canAttack = false;
            }
        }

        #endregion

        public void Setup(int range, int damage, float attackRate)
        {
            Range = range;
            Damage = damage;
            AttackRate = attackRate;

            rangeTrigger.radius = range;
            rangeTrigger.center = Vector3.zero;
            rangeTrigger.isTrigger = true;
        }

        public void Attack(AttackType attackType)
        {
            if (!canAttack)
            {
                return;
            }

            switch(attackType)
            {
                case AttackType.Damage:
                    TargetEnemy?.TakeDamage(Damage);
                    break;
                case AttackType.Slow:
                    foreach (var enemy in EnemiesInRange)
                    {
                        enemy.Slow();
                    }
                    break;
            }

            canAttack = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IEnemy>();

            if (enemy != null)
            {
                EnemiesInRange.Add(enemy);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IEnemy>();

            if (enemy != null)
            {
                EnemiesInRange.Remove(enemy);
            }

            if (enemy == TargetEnemy)
            {
                TargetEnemy = null;
            }
        }

        #region GIZMOS

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (currentState == null || !isActive)
            {
                return;
            }

            Handles.color = currentState.GizmoColor;
            Handles.Label(transform.position + Vector3.up, currentState.name);

            Gizmos.color = currentState.GizmoColor;
            Gizmos.DrawWireSphere(transform.position, Range);

            if (TargetEnemy != null)
            {
                Gizmos.DrawLine(transform.position, TargetEnemy.GetPosition());
            }
        }


#endif

        #endregion

    }
}