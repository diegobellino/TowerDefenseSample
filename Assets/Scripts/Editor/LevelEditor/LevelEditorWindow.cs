using System.Collections.Generic;
using System.Linq;
using TowerDefense.GameLogic.Runtime.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LevelConfig = TowerDefense.Levels.LevelConfig;

namespace TowerDefense.Editor.LevelEditor
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
        
        [MenuItem("TowerDefense/Open Level Editor")]
        public static void ShowExample()
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
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelEditor/MainWindow.uxml");
            mainElement = visualTree.Instantiate();

            var levelSizePanel = mainElement.Q("level-size-field");
            levelSizePanel.Add(new Vector2Field("Size (Units)"));
            
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

            var mapSize = mainElement.Q("level-size-field").Children().First() as Vector2Field;
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
            
            var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
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
            LoadLevel();
        }

        private void OpenLevelEditor()
        {
            levelEditorScene = EditorSceneManager.OpenScene(LEVEL_EDITOR_SCENE_PATH, OpenSceneMode.Additive);

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
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelEditor/LevelEditor.uxml");
            
            mainElement = visualTree.Instantiate();

            mainElement.Q<Label>("level-editor-title").text = $"<color=green>{selectedLevelConfig.name}</color> Editor";
            mainElement.Q("castle-health-field").Add(new IntegerField("Health"));
            mainElement.Q("castle-position-field").Add(new Vector2Field("Position"));
            mainElement.Q<Button>("add-spawner-button").clicked += AddSpawnerContainer;
            mainElement.Q<Button>("save-button").clicked += SaveLevel;

            mainElement.Q<Button>("exit-button").clicked += OnExitLevelEditor;
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
            
            var castleHealthField = mainElement.Q("castle-health-field").Children().First() as IntegerField;
            var castlePositionField = mainElement.Q("castle-position-field").Children().First() as Vector2Field;

            if (selectedLevelConfig == null ||
                selectedLevelConfig.hordeSpawnerLocations.Length != selectedLevelConfig.hordeConfigs.Length)
            {
                return;
            }
            
            castleHealthField.value = selectedLevelConfig.castleHealth;
            castlePositionField.value = selectedLevelConfig.castlePosition;

            for (int i = 0; i < selectedLevelConfig.hordeConfigs.Length; i++)
            {
                AddSpawnerContainer(selectedLevelConfig.hordeConfigs[i], selectedLevelConfig.hordeSpawnerLocations[i]);
            }
        }

        private void SaveLevel()
        {
            var health = mainElement.Q("castle-health-field").Children().First() as IntegerField;
            var castlePosition = mainElement.Q("castle-position-field").Children().First() as Vector2Field;
            var hordeConfigs = new List<HordeConfig>();
            var hordeSpawnerLocations = new List<Vector2>();
            
            foreach (var spawner in mainElement.Q("horde-spawners").Children())
            {
                var locationField = spawner.Q("horde-spawner-position-field").Children().First() as Vector2Field;
                var configField = spawner.Q("horde-spawner-config-field").Children().First() as ObjectField;
                
                hordeConfigs.Add(configField.value as HordeConfig);
                hordeSpawnerLocations.Add(locationField.value);
            }
            
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
        
        private void AddSpawnerContainer(HordeConfig config, Vector2? spawnerPosition)
        {
            spawnersCount++;
            
            hordeSpawnerContainer ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelEditor/HordeSpawnerContainer.uxml");;
            var newSpawnerContainer = hordeSpawnerContainer.Instantiate();

            var id = $"Spawner {spawnersCount}";
            newSpawnerContainer.Q<Label>("horde-spawner-id").text = id;

            var positionField = new Vector2Field("Position");
            if (spawnerPosition != null)
            {
                positionField.value = spawnerPosition.Value;
            }
            newSpawnerContainer.Q("horde-spawner-position-field").Add(positionField);

            var configField = new ObjectField("Config"){
                objectType = typeof(HordeConfig)
            };
            if (config != null)
            {
                configField.value = config;
            }
            newSpawnerContainer.Q("horde-spawner-config-field").Add(configField);
            
            newSpawnerContainer.Q<Button>("remove-spawner-button").clicked += () =>
            {
                RemoveSpawnerContainer(id);
            };
            
            mainElement.Q("horde-spawners").Add(newSpawnerContainer);
        }

        private void RemoveSpawnerContainer(string id)
        {
            var hordeSpawners = mainElement.Q("horde-spawners");
            var toRemove = hordeSpawners.Children().First(
                child => child.Q<Label>("horde-spawner-id").text.Equals(id));
            hordeSpawners.Remove(toRemove);
        }
        
        #endregion
        
    }
}