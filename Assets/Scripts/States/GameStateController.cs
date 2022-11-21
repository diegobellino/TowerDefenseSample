using UnityEngine;
using System.Collections.Generic;
using TowerDefense.GameActions;
using TowerDefense.Levels;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.States
{
    /// <summary>
    /// Lives throughout all lifetime of the app. Controls Game States
    /// </summary>
    public class GameStateController : MonoBehaviour, IActionReceiver
    {
        #region VARIABLES

        public static GameStateController Instance;

        [SerializeField] private StateConfig[] stateConfigs;
        [SerializeField] private GameObject mainCanvas;

        private Dictionary<StateId, BaseStateManager> states = new();
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

            GameActionManager.RegisterActionReceiver(this);

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
            #if UNITY_EDITOR
            
            if (EditorPrefs.HasKey("EnterPlayModeOnLevel"))
            {
                var levelPath = EditorPrefs.GetString("EnterPlayModeOnLevel");
                var levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(levelPath);
                ChangeState(StateId.Level, levelConfig);

                EditorPrefs.DeleteKey("EnterPlayModeOnLevel");
                return;
            }
            
            #endif
            ChangeState(StateId.Home);
            
        }

        public void ChangeState(StateId stateId, object stateData = null)
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

            newState.OnOpenState(stateData);

            currentState = newState;
        }

        public void OnAction(GameAction action)
        {
            currentState?.OnAction(action);
        }

        public void OnAction(GameAction action, BatonPassData data)
        {
            currentState?.OnAction(action, data);
        }
    }
}