using UnityEngine;
using System.Collections.Generic;

namespace TowerDefense.States
{
    public enum GameAction
    {
        Gameplay_SpawnTower,
        Gameplay_Select,
        Gameplay_Unselect,
        Gameplay_LevelUpTower,
        Gameplay_TakeDamage
    }

    public struct BatonPassData
    {
        public int IntData;
        public float FloatData;
        public string StringData;
        public bool BoolData;
        public object ExtraData;
    }

    /// <summary>
    /// Lives trhoughout all lifetime of the app. Controls Game States
    /// </summary>
    public class GameStateController : MonoBehaviour
    {
        #region VARIABLES

        public static GameStateController Instance;

        [SerializeField] private StateConfig[] stateConfigs;
        [SerializeField] private GameObject mainCanvas;

        private Dictionary<StateId, BaseStateManager> states = new Dictionary<StateId, BaseStateManager>();

        private BaseStateManager currentState;

        #endregion

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(this);

            CreateStates();
        }

        private void CreateStates()
        {
            // Add all states here
            states.Add(StateId.Home, new HomeStateManager());
            states.Add(StateId.Level, new LevelStateManager());
            states.Add(StateId.GameOver, new GameOverStateManager());
        }

        private void Start()
        {
            ChangeState(StateId.Home);
        }

        public void ChangeState(StateId stateId)
        {
            if (!states.ContainsKey(stateId))
            {
                return;
            }

            var newState = states[stateId];

            if (newState == currentState)
            {
                return;
            }

            if (currentState != null)
            {
                currentState.OnCloseState();

                if (currentState.stateController != null)
                {
                    Destroy(currentState.stateController.gameObject);
                }

                if (currentState.uiController != null)
                {
                    Destroy(currentState.uiController.gameObject);
                }

                currentState.UnregisterControllers();
            }

            foreach(var config in stateConfigs)
            {
                if (config.StateId != stateId)
                {
                    continue;
                }

                BaseStateController stateController = null;
                BaseStateController uiController = null;
                
                if (config.StateController != null)
                {
                   stateController  = Instantiate(config.StateController);
                }

                if (config.UIController != null)
                {
                    uiController = Instantiate(config.UIController, mainCanvas.transform);
                }

                newState.RegisterControllers(stateController, uiController);
            }

            newState.OnOpenState();

            currentState = newState;
        }

        public void FireAction(GameAction action)
        {
            currentState?.OnAction(action);
        }

        public void FireAction(GameAction action, BatonPassData data)
        {
            currentState?.OnAction(action, data);
        }
    }
}