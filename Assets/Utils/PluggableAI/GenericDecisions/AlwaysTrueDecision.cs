using UnityEngine;

namespace Utils.PluggableAI.GenericDecisions
{
    [CreateAssetMenu(menuName = "PluggableAI/Generic Decisions/Always True", fileName = "AlwaysTrue")]
    public class AlwaysTrueDecision : PluggableAIDecision
    {
        public override bool Decide(PluggableAIStateController controller)
        {
            return true;
        }
    }
}
