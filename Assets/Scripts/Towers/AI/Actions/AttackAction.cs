using UnityEngine;
using Utils.PluggableAI;

namespace TowerDefense.Towers.AI
{
    [CreateAssetMenu(menuName = "Tower Defense/Towers/AI/Actions/Attack", fileName = "Attack")]
    public class AttackAction : PluggableAIAction
    {
        [SerializeField] private AttackType attackType;

        public override void Act(PluggableAIStateController controller)
        {
            var towerController = controller as TowerAIStateController;

            towerController.Attack(attackType);
        }
    }
}
