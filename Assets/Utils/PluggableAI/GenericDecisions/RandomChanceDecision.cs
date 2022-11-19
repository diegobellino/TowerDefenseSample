using UnityEngine;

namespace Utils.PluggableAI.GenericDecisions
{
    [CreateAssetMenu(menuName = "PluggableAI/Generic Decisions/Random Chance", fileName = "RandomChance")]
    public class RandomChanceDecision : PluggableAIDecision
    {
        [SerializeField, Range(0,100)] private int chance;

        public override bool Decide(PluggableAIStateController controller)
        {
            return Random.Range(0f, 1f) <= chance / 100f;
        }
    }
}
