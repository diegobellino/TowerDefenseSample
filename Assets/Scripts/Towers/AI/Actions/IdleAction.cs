using UnityEngine;
using Utils.PluggableAI;

namespace TowerDefense.Towers.AI.Actions
{
    [CreateAssetMenu(menuName = "Tower Defense/Towers/AI/Actions/Idle", fileName = "Idle")]
    public class IdleAction : PluggableAIAction
    {
        public override void Act(PluggableAIStateController controller)
        {
            return;
        }
    }
}
