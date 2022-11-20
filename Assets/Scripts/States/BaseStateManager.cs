using TowerDefense.GameActions;

namespace TowerDefense.States
{
    public enum StateId
    {
        Home,
        Level,
        GameOver
    }

    /// <summary>
    /// StateManagers act as bridges between 3D world and UI - for a specific state
    /// </summary>
    public abstract class BaseStateManager
    {
        #region VARIABLES

        public BaseStateController stateController;
        public BaseStateController uiController;

        #endregion

        public BaseStateManager()
        {
            
        }

        public void RegisterControllers(BaseStateController stateController, BaseStateController uiController)
        {
            this.stateController = stateController;
            this.uiController = uiController;
        }

        public void UnregisterControllers()
        {
            stateController = null;
            uiController = null;
        }

        public virtual void OnOpenState()
        {
            stateController?.RegisterManager(this);
            stateController?.OnOpenState();

            uiController?.RegisterManager(this);
            uiController?.OnOpenState();
        }

        public virtual void OnCloseState()
        {
            stateController?.UnregisterManager();
            stateController?.OnCloseState();

            uiController?.UnregisterManager();
            uiController?.OnCloseState();
        }

        public abstract void OnAction(GameAction action);

        public abstract void OnAction(GameAction action, BatonPassData data);
    }
}