using UnityEngine;
using Utils.PluggableAI;

namespace TowerDefense.Towers.AI.Decisions
{
    [CreateAssetMenu(menuName = "Tower Defense/Towers/AI/Decisions/Enemy In Range", fileName = "Any Enemy In Range")]
    public class EnemyInRangeDecision : PluggableAIDecision
    {
        public override bool Decide(PluggableAIStateController controller)
        {
            var towerController = controller as TowerAIStateController;

            return towerController.EnemiesInRange.Count > 0;
        }
    }
}
