using UnityEngine;

namespace TowerDefense.States
{
    public class BaseStateController : MonoBehaviour
    {
        #region VARIABLES

        protected BaseStateManager stateManager;

        #endregion

        public virtual void OnOpenState()
        {

        }

        public virtual void OnCloseState()
        {

        }

        public virtual void RegisterManager(BaseStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public virtual void UnregisterManager()
        {
            this.stateManager = null;
        }
    }
}
