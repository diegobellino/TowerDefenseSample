using UnityEngine;
using Utils.SmartUpdate;


namespace Utils.PluggableAI
{
    /// <summary>
    /// Base class for AI Controllers. Implements the most basic state controller for a State Machine AI
    /// Includes functions to transition from state, set up and Gizmo debugging
    /// </summary>
    public abstract class PluggableAIStateController : MonoBehaviour, ISmartUpdate
    {
        #region VARIABLES

        [SerializeField] private UpdateGroup updateGroup = UpdateGroup.Timed;
        public UpdateGroup Group => updateGroup;

        [SerializeField] protected PluggableAIState currentState;

        protected bool isActive;

        #endregion

        #region LIFETIME

        private void Start()
        {
            SmartUpdateController.Instance?.Register(this);
        }

        private void OnDestroy()
        {
            SmartUpdateController.Instance?.Unregister(this);
        }

        public virtual void SmartUpdate(float deltaTime)
        {
            if (!isActive)
            {
                return;
            }

            if (currentState == null)
            {
                Debug.LogWarning("PluggableAI: Trying to update null current state, AI will be disabled");
                EnableAI(false);

                return;
            }

            currentState.UpdateState(this);
        }

        #endregion

        public virtual void EnableAI(bool isActive)
        {
            this.isActive = isActive;

            Debug.Log($"PluggableAI: {(isActive ? "Enabled" : "Disabled")} AI update on {name}");
        }

        public virtual bool TriggerState(PluggableAIState newState)
        {
            if (newState != currentState && newState != null)
            {
                Debug.Log($"PluggableAI: {currentState} -> {newState}");

                currentState = newState;

                return true;
            }

            return false;
        }
    }
}