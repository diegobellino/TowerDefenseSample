using UnityEngine;

namespace Utils.PluggableAI
{
    /// <summary>
    /// Decisions return a logical value, which is used by the State Controller to decide the next state
    /// </summary>
    public abstract class PluggableAIDecision : ScriptableObject
    {
        public abstract bool Decide(PluggableAIStateController controller);
    }
}