using UnityEngine;

namespace Utils.PluggableAI
{
    /// <summary>
    /// Actions define the behaviour of the state that they are attached to
    /// </summary>
    public abstract class PluggableAIAction : ScriptableObject
    {
        public abstract void Act(PluggableAIStateController controller);
    }
}