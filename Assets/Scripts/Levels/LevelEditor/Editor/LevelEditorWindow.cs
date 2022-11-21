using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TowerDefense.GameActions;
using TowerDefense.Hordes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TowerDefense.Levels.LevelEditor.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region VARIABLES
        
        enum ViewType
        {
            Main,
            LevelEditor
        }

        private const string LEVEL_EDITOR_SCENE_PATH = "Assets/Scenes/Editor/LevelEditor.unity";

        private ViewType selectedViewType = ViewType.Main;
        
        private TemplateContainer mainElement;
        private VisualTreeAsset hordeSpawnerContainer;

        private Scene levelEditorScene;
        private LevelConfig selectedLevelConfig;
        private string levelPath;
        
        private int spawnersCount;
        private bool isTesting;

        private LevelEditorController controller;
        
        #endregion

        [MenuItem("TowerDefense/Open Level Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent("Level Editor");
        }

        public void CreateGUI()
        {
            if (isTesting)
            {
                EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;

                return;
            }
            
            rootVisualElement.Clear();

            switch (selectedViewType)
            {
                case ViewType.Main:
                    CreateMainView();
                    break;
                case ViewType.LevelEditor:
                    CreateLevelEditorView();
                    break;
            }
            
            rootVisualElement.Add(mainElement);
        }
        
        public void OnGUI()
        {
            if (mainElement == null)
            {
                return;
            }
            
            switch (selectedViewType)
            {
                case ViewType.Main:
                    OnMainViewGUI();
                    break;
                case ViewType.LevelEditor:
                    OnLevelEditorGUI();
                    break;
            }
        }
        
        private void OnEditorPlayModeStateChanged(PlayModeStateChange change)
        {
            if (isTesting && change == PlayModeStateChange.EnteredEditMode)
            {
                isTesting = false;
                OpenLevelEditor();
            }
            
        }
        
        #region MAIN VIEW

        private void CreateMainView()
        {
            mainElement?.Clear();
            
            var visualTree = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Levels/LevelEditor/Editor/UI/MainWindow.uxml");
            mainElement = visualTree.Instantiate();

            var loadLevelPanel = mainElement.Q("load-level-asset-field");
            loadLevelPanel.Add(new ObjectField("Level Asset")
            {
                objectType = typeof(LevelConfig)
            });

            mainElement.Q<Button>("create-level-button").clicked += OnCreateLevelButton;
            mainElement.Q<Button>("load-level-button").clicked += OnLoadLevelButton;
        }

        private void OnMainViewGUI()
        {
            if (mainElement.Q<RadioButton>("new-level-button").value)
            {
                mainElement.Q("new-level-panel").RemoveFromClassList("hide");
                mainElement.Q("load-level-panel").AddToClassList("hide");
            }
            else if (mainElement.Q<RadioButton>("load-level-button").value)
            {
                mainElement.Q("new-level-panel").AddToClassList("hide");
                mainElement.Q("load-level-panel").RemoveFromClassList("hide");
            }
        }
        
        private void OnCreateLevelButton()
        {
            var levelName = mainElement.Q<TextField>("level-name-field").value;
            if (levelName.Equals("filler text"))
            {
                return;
            }

            levelPath = EditorUtility.SaveFolderPanel("Save level asset to path", "", "");
            if (string.IsNullOrEmpty(levelPath))
            {
                return;
            }
            
            levelPath = "Assets" + levelPath.Split("/Assets").Last();
            levelPath += $"/{levelName}.asset";
            
            var levelConfig = CreateInstance<LevelConfig>();
            levelConfig.name = levelName;
            
            AssetDatabase.CreateAsset(levelConfig, levelPath);
            
            selectedLevelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(levelPath);
            
            OpenLevelEditor();
        }
        
        private void OnLoadLevelButton()
        {
            var objectField = mainElement.Q("load-level-asset-field").Children().First() as ObjectField;
            selectedLevelConfig = objectField?.value as LevelConfig;
            
            OpenLevelEditor();
        }

        private void OpenLevelEditor()
        {
            levelEditorScene = EditorSceneManager.OpenScene(LEVEL_EDITOR_SCENE_PATH, OpenSceneMode.Additive);

            controller = FindObjectOfType<LevelEditorController>();
            Assert.NotNull(controller, $"Could not find object of type {typeof(LevelEditorController)}");
            
            selectedViewType = ViewType.LevelEditor;
            CreateGUI();
        }
        
        #endregion

        #region LEVEL EDITOR VIEW

        private void CreateLevelEditorView()
        {
            spawnersCount = 0;
            
            EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
            
            mainElement?.Clear();

            var visualTree = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Levels/LevelEditor/Editor/UI/LevelEditor.uxml");
            
            mainElement = visualTree.Instantiate();

            mainElement.Q<Label>("level-editor-title").text = $"<color=green>{selectedLevelConfig.name}</color> Editor";

            var castleHealthField = new IntegerField("Health");
            castleHealthField.RegisterValueChangedCallback(e =>
            {
                controller.ChangeCastleHealth(e.newValue);
            });
            mainElement.Q("castle-health-field").Add(castleHealthField);
            
            var castlePositionField = new Vector2IntField("Position");
            castlePositionField.RegisterValueChangedCallback(e =>
            {
                controller.RepositionCastle(e.newValue);
            });
            mainElement.Q("castle-position-field").Add(castlePositionField);
            
            mainElement.Q<Button>("add-spawner-button").clicked += AddSpawnerContainer;
            mainElement.Q<Button>("save-button").clicked += SaveLevel;
            mainElement.Q<Button>("test-level-button").clicked += TestLevel;
            mainElement.Q<Button>("exit-button").clicked += OnExitLevelEditor;
            
            LoadLevel();
        }

        private void OnLevelEditorGUI()
        {
            
        }

        private void OnExitLevelEditor()
        {
            EditorSceneManager.CloseScene(levelEditorScene, true);
            
            selectedViewType = ViewType.Main;
            CreateGUI();
        }

        private void TestLevel()
        {
            var path = AssetDatabase.GetAssetPath(selectedLevelConfig);
            EditorPrefs.SetString("EnterPlayModeOnLevel", path);
            controller.ClearAll();
            isTesting = true;
            EditorApplication.isPlaying = true;
        }

        private void LoadLevel()
        {
            spawnersCount = 0;
            
            var castleHealthField = mainElement.Q("castle-health-field").Children().First() as IntegerField;
            var castlePositionField = mainElement.Q("castle-position-field").Children().First() as Vector2IntField;

            if (selectedLevelConfig == null)
            {
                return;
            }

            castleHealthField.value = selectedLevelConfig.castleHealth;
            castlePositionField.value = selectedLevelConfig.castlePosition;
            
            controller.CreateCastle(castlePositionField.value, castleHealthField.value);

            if (selectedLevelConfig.hordeConfigs == null)
            {
                return;
            }

            for (int i = 0; i < selectedLevelConfig.hordeConfigs.Length; i++)
            {
                AddSpawnerContainer(selectedLevelConfig.hordeConfigs[i], selectedLevelConfig.hordeSpawnerLocations[i]);
            }
        }

        private void SaveLevel()
        {
            var health = mainElement.Q("castle-health-field").Children().First() as IntegerField;
            var castlePosition = mainElement.Q("castle-position-field").Children().First() as Vector2IntField;
            var hordeConfigs = new List<HordeConfig>();
            var hordeSpawnerLocations = new List<Vector2Int>();
            
            foreach (var spawner in mainElement.Q("horde-spawners").Children())
            {
                var locationField = spawner.Q("horde-spawner-position-field").Children().First() as Vector2IntField;
                var configField = spawner.Q("horde-spawner-config-field").Children().First() as ObjectField;
                
                hordeConfigs.Add(configField.value as HordeConfig);
                hordeSpawnerLocations.Add(locationField.value);
            }

            selectedLevelConfig.castleHealth = health.value;
            selectedLevelConfig.castlePosition = castlePosition.value;
            selectedLevelConfig.hordeConfigs = hordeConfigs.ToArray();
            selectedLevelConfig.hordeSpawnerLocations = hordeSpawnerLocations.ToArray();
            
            EditorUtility.SetDirty(selectedLevelConfig);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Level {selectedLevelConfig.name} saved!");
        }

        private void AddSpawnerContainer()
        {
            AddSpawnerContainer(null, null);
        }
        
        private void AddSpawnerContainer(HordeConfig config, Vector2Int? spawnerPosition)
        {
            spawnersCount++;
            var currentCount = spawnersCount;
            
            hordeSpawnerContainer ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Levels/LevelEditor/Editor/UI/HordeSpawnerContainer.uxml");;
            var newSpawnerContainer = hordeSpawnerContainer.Instantiate();

            var id = $"Spawner {spawnersCount}";
            newSpawnerContainer.Q<Label>("horde-spawner-id").text = id;

            var positionField = new Vector2IntField("Position");
            if (spawnerPosition != null)
            {
                positionField.value = spawnerPosition.Value;
            }
            positionField.RegisterValueChangedCallback(e =>
            {
                controller.RepositionSpawner(currentCount, e.newValue);
            });
            newSpawnerContainer.Q("horde-spawner-position-field").Add(positionField);

            var configField = new ObjectField("Config"){
                objectType = typeof(HordeConfig)
            };
            if (config != null)
            {
                configField.value = config;
            }
            configField.RegisterValueChangedCallback(e =>
            {
                controller.ChangeSpawnerConfig(currentCount, e.newValue as HordeConfig);
            });
            newSpawnerContainer.Q("horde-spawner-config-field").Add(configField);
            
            newSpawnerContainer.Q<Button>("remove-spawner-button").clicked += () =>
            {
                RemoveSpawnerContainer(currentCount);
            };
            
            mainElement.Q("horde-spawners").Add(newSpawnerContainer);
            
            controller.CreateSpawner(currentCount, positionField.value, configField.value as HordeConfig);
        }

        private void RemoveSpawnerContainer(int id)
        {
            var stringId = $"Spawner {id}";
            var hordeSpawners = mainElement.Q("horde-spawners");
            var toRemove = hordeSpawners.Children().First(
                child => child.Q<Label>("horde-spawner-id").text.Equals(stringId));
            hordeSpawners.Remove(toRemove);
            
            controller.DestroySpawner(id);
        }
        
        #endregion
    }
}