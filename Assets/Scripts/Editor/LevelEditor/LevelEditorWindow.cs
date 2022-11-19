using TowerDefense.Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TowerDefense.Editor.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        private TemplateContainer mainElement;
        
        [MenuItem("TowerDefense/Open Level Editor")]
        public static void ShowExample()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent("Level Editor");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var visualTree = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelEditor/LevelEditorWindow.uxml");
            mainElement = visualTree.Instantiate();
            
            var loadLevelPanel = mainElement.Q("load-level-asset-field");

            var assetField = new ObjectField("Level Asset")
            {
                objectType = typeof(LevelConfig)
            };

            loadLevelPanel.Add(assetField);

            root.Add(mainElement);
        }

        public void OnGUI()
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
    }
}