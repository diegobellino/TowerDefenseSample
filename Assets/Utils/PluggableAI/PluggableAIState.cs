using UnityEngine;

namespace Utils.PluggableAI
{
    /// <summary>
    /// States represent the current behaviour of the AI. They include a set of actions and transitions to other states
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/State")]
    public class PluggableAIState : ScriptableObject
    {
        public Color GizmoColor = Color.gray;

        [SerializeField] private PluggableAIAction[] actions;
        [SerializeField] private PluggableAITransition[] transitions;

        public void UpdateState(PluggableAIStateController controller)
        {
            DoActions(controller);
            CheckTransitions(controller);
        }

        void DoActions(PluggableAIStateController controller)
        {
            foreach (PluggableAIAction action in actions)
            {
                action.Act(controller);
            }
        }

        void CheckTransitions(PluggableAIStateController controller)
        {
            foreach (PluggableAITransition transition in transitions)
            {
                if (transition.decision.Decide(controller))
                {
                    if (controller.TriggerState(transition.trueState))
                    {
                        return;
                    }
                }
                else
                {
                    if (controller.TriggerState(transition.falseState))
                    {
                        return;
                    }
                }
            }
        }
    }
}