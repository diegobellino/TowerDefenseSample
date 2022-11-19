namespace Utils.PluggableAI
{
    /// <summary>
    /// Transitions define the next state of the State Controller after an action has been performed, based on a Decision
    /// </summary>
    [System.Serializable]
    public class PluggableAITransition
    {
        public PluggableAIDecision decision;
        public PluggableAIState trueState, falseState;
    }
}