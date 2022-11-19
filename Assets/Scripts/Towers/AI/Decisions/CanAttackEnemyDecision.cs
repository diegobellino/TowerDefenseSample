using UnityEngine;
using Utils.PluggableAI;

namespace TowerDefense.Towers.AI.Decisions
{
    [CreateAssetMenu(menuName = "Tower Defense/Towers/AI/Decisions/Can Attack", fileName = "Can Attack Enemy")]
    public class CanAttackEnemyDecision : PluggableAIDecision
    {
        public override bool Decide(PluggableAIStateController controller)
        {
            var towerController = controller as TowerAIStateController;

            return towerController.TargetEnemy != null && towerController.TargetEnemy.IsTargeteable();
        }
    }
}
