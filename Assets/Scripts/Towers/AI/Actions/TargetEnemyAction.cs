using System.Collections.Generic;
using UnityEngine;
using Utils.PluggableAI;
using TowerDefense.Enemies;
using Consts = TowerDefense.Constants.Constants;

namespace TowerDefense.Towers.AI.Actions
{
    [CreateAssetMenu(menuName = "Tower Defense/Towers/AI/Actions/Target Enemy", fileName = "Target Enemy")]
    public class TargetEnemyAction : PluggableAIAction
    {
        public override void Act(PluggableAIStateController controller)
        {
            var towerController = controller as TowerAIStateController;
            var potentialEnemies = new Dictionary<EnemyType, IEnemy>();
            
            // Find potential targets
            foreach (var enemy in towerController.EnemiesInRange)
            {
                if (!enemy.IsTargeteable() || potentialEnemies.ContainsKey(enemy.Type))
                {
                    continue;
                }

                if (Vector3.Distance(enemy.GetPosition(), towerController.transform.position) > towerController.Range)
                {
                    continue;
                }

                potentialEnemies.Add(enemy.Type, enemy);
            }

            IEnemy highestPrioEnemy = null;
            var highestPrio = -1;

            // Choose target based on priority
            foreach (var enemy in potentialEnemies.Values)
            {
                if (Consts.ENEMY_PRIOS.TryGetValue(enemy.Type, out var currentPrio))
                {
                    if (currentPrio > highestPrio)
                    {
                        highestPrio = currentPrio;
                        highestPrioEnemy = enemy;
                    }
                }
            }

            towerController.TargetEnemy = highestPrioEnemy;
        }
    }
}