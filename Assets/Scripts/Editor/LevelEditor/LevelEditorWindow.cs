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

            mainElement.Q<Button>("create-level-button").clicked += OnCreateLevel;
            mainElement.Q<Button>("load-level-button").clicked += OnLoadLevel;
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
        
        private void OnCreateLevel()
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
            
            var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
            levelConfig.name = levelName;
            
            AssetDatabase.CreateAsset(levelConfig, levelPath);
            selectedLevelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(levelPath);
            
            OpenLevelEditor();
        }
        
        private void OnLoadLevel()
        {
            var objectField = mainElement.Q("load-level-asset-field").Children().First() as ObjectField;
            selectedLevelConfig = objectField?.value as LevelConfig;
            OpenLevelEditor();
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

        private void SaveLevel()
        {
            
        }

        private void AddSpawnerContainer()
        {
            spawnersCount++;
            
            hordeSpawnerContainer ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelEditor/HordeSpawnerContainer.uxml");;
            var newSpawnerContainer = hordeSpawnerContainer.Instantiate();

            var id = $"Spawner {spawnersCount}";
            newSpawnerContainer.Q<Label>("horde-spawner-id").text = id;
            newSpawnerContainer.Q("horde-spawner-position-field").Add(new Vector2Field("Position"));
            newSpawnerContainer.Q("horde-spawner-config-field").Add(new ObjectField("Config")
            {
                objectType = typeof(HordeConfig)
            });
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