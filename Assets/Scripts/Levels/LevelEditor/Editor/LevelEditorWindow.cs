using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TowerDefense.GameLogic.Runtime.Configs;
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

        private LevelEditorController controller;

        [MenuItem("TowerDefense/Open Level Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent("Level Editor");
        }

        public void CreateGUI()
        {
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
        
        #region MAIN_VIEW

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

            var mapSize = mainElement.Q("level-size-field").Children().First() as Vector2IntField;
            if (mapSize.value.magnitude <= 0)
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
            selectedLevelConfig.mapSize = mapSize.value;
            
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

        #region LEVEL_EDITOR_VIEW

        private void CreateLevelEditorView()
        {
            spawnersCount = 0;

            mainElement?.Clear();

            var visualTree = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Levels/LevelEditor/Editor/UI/LevelEditor.uxml");
            
            mainElement = visualTree.Instantiate();

            mainElement.Q<Label>("level-editor-title").text = $"<color=green>{selectedLevelConfig.name}</color> Editor";
            
            var levelSizePanel = mainElement.Q("level-size-field");
            var levelSizeField = new Vector2IntField("Size (Units)");
            levelSizeField.RegisterValueChangedCallback(e =>
            {
                controller.ResizeMap(e.newValue);
            });
            levelSizePanel.Add(levelSizeField);
            
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
            mainElement.Q<Button>("exit-button").clicked += OnExitLevelEditor;
            
            LoadLevel();
            controller.CreateCastle(castlePositionField.value, castleHealthField.value);
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

        private void LoadLevel()
        {
            spawnersCount = 0;
            
            var levelSizeField = mainElement.Q("level-size-field").Children().First() as Vector2IntField;
            var castleHealthField = mainElement.Q("castle-health-field").Children().First() as IntegerField;
            var castlePositionField = mainElement.Q("castle-position-field").Children().First() as Vector2IntField;

            if (selectedLevelConfig == null ||
                selectedLevelConfig.hordeSpawnerLocations.Length != selectedLevelConfig.hordeConfigs.Length)
            {
                return;
            }

            levelSizeField.value = selectedLevelConfig.mapSize;
            castleHealthField.value = selectedLevelConfig.castleHealth;
            castlePositionField.value = selectedLevelConfig.castlePosition;

            for (int i = 0; i < selectedLevelConfig.hordeConfigs.Length; i++)
            {
                AddSpawnerContainer(selectedLevelConfig.hordeConfigs[i], selectedLevelConfig.hordeSpawnerLocations[i]);
            }
        }

        private void SaveLevel()
        {
            var size = mainElement.Q("level-size-field").Children().First() as Vector2IntField;
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

            selectedLevelConfig.mapSize = size.value;
            selectedLevelConfig.castleHealth = health.value;
            selectedLevelConfig.castlePosition = castlePosition.value;
            selectedLevelConfig.hordeConfigs = hordeConfigs.ToArray();
            selectedLevelConfig.hordeSpawnerLocations = hordeSpawnerLocations.ToArray();
            
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