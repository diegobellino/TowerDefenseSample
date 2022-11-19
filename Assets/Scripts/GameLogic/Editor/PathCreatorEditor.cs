using System.Collections.Generic;
using TowerDefense.GameLogic.Runtime.Configs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.GameLogic.Editor
{
    [SerializeField]
    public class BezierNode
    {
        public Vector3 nodePosition = Vector3.zero;
        public Vector3 handlePosition = new Vector3(1, 0, 1);
    }

    public class PathCreatorEditor : EditorWindow
    {

        #region VARIABLES

        private const string EDITOR_SCENE_ASSET_PATH = "Assets/Scenes/Editor/PathCreator.unity";
        private const string BEZIER_NODE_ASSET_PATH = "Path/Bezier Node.prefab";
        
        [SerializeField] private PathConfig path;
        [SerializeField] private List<int> subdivisions;

        private SerializedObject serializedObject;
        private ReorderableList nodeSubdivisions;
        private Scene editorScene;
        private List<Transform> nodes;
        private List<Vector3> pathPoints;

        #endregion

        [MenuItem("TowerDefense/Path Creator")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            PathCreatorEditor window = (PathCreatorEditor)EditorWindow.GetWindow(typeof(PathCreatorEditor));
            window.titleContent = new GUIContent("Path Creator Tool");
            window.Show();
        }

        private void Awake()
        {
            nodes = new List<Transform>();
            subdivisions = new List<int>();
            pathPoints = new List<Vector3>();
            serializedObject = new SerializedObject(this);

            nodeSubdivisions = new ReorderableList(serializedObject, serializedObject.FindProperty("subdivisions"));
            nodeSubdivisions.onAddCallback += CreateBezierNode;
            nodeSubdivisions.drawHeaderCallback += DrawHeader;
            nodeSubdivisions.drawElementCallback += DrawElement;
            nodeSubdivisions.onRemoveCallback += RemoveBezierNode;
            nodeSubdivisions.onReorderCallback += ReorderSubdivisions;

            editorScene = EditorSceneManager.OpenScene(EDITOR_SCENE_ASSET_PATH, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(editorScene);

            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDestroy()
        {
            nodeSubdivisions.onAddCallback -= CreateBezierNode;
            nodeSubdivisions.drawHeaderCallback -= DrawHeader;
            nodeSubdivisions.drawElementCallback -= DrawElement;
            nodeSubdivisions.onRemoveCallback -= RemoveBezierNode;
            nodeSubdivisions.onReorderCallback -= ReorderSubdivisions;

            EditorSceneManager.CloseScene(editorScene, true);
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        #region GUI

        public void OnGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("path"));

            nodeSubdivisions.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Load Path"))
            {
                LoadPath();
            }

            if (GUILayout.Button("Save Path"))
            {
                SavePath();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (SceneManager.GetActiveScene() != editorScene)
            {
                return;
            }

            pathPoints.Clear();
            
            for (int i = 0; i < nodes.Count; i++)
            {
                var handle = nodes[i].GetChild(0);

                Handles.color = Color.cyan;

                Handles.Label(nodes[i].position + Vector3.up, $"Node {i}");
                Handles.DrawLine(nodes[i].position, handle.position);

                Handles.color = Color.white;

                if (nodes.Count >= 2 && i < nodes.Count - 1)
                {
                    var nodeSubdivisions = subdivisions[i];

                    if (nodeSubdivisions <= 0)
                    {
                        continue;
                    }

                    var pathPointsForNode = new List<Vector3>();


                    for (int j = 1; j <= nodeSubdivisions; j++)
                    {
                        var delta = (float)j / (float)nodeSubdivisions;

                        var position = Vector3.Lerp(
                                Vector3.Lerp(nodes[i].position, handle.position, delta),
                                Vector3.Lerp(handle.position, nodes[i + 1].position, delta),
                                delta);

                        pathPointsForNode.Add(position);
                    }

                    Handles.DrawLine(nodes[i].position, pathPointsForNode[0]);

                    for (int j = 0; j < nodeSubdivisions - 1; ++j)
                    {
                        Handles.DrawWireCube(pathPointsForNode[j], Vector3.one * .2f);
                        Handles.DrawLine(pathPointsForNode[j], pathPointsForNode[j + 1]);
                    }

                    Handles.DrawLine(pathPointsForNode[nodeSubdivisions - 1], nodes[i + 1].position);

                    pathPoints.AddRange(pathPointsForNode);
                }
            }
        }

        #endregion

        #region SAVE/LOAD

        private void SavePath()
        {
            path.Save(pathPoints.ToArray(), nodes.ToArray(), subdivisions.ToArray());
            
            EditorUtility.SetDirty(path);
            AssetDatabase.SaveAssets();
        }

        private void LoadPath()
        {
            
            if (path == null)
            {
                return;
            }

            foreach (var node in nodes)
            {
                DestroyImmediate(node.gameObject);
            }

            subdivisions.Clear();
            nodes.Clear();
            nodeSubdivisions.serializedProperty.ClearArray();
            
            for (int i = 0; i < path.NodePositions.Length; i++)
            {
                CreateBezierNode(nodeSubdivisions);
                
                nodes[i].position = path.NodePositions[i];
                nodes[i].GetChild(0).position = path.HandlePositions[i];

                subdivisions.Add(path.Subdivisions[i]);
            }
        }

        #endregion

        #region REORDERABLE LIST

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Bezier Nodes");

        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = nodeSubdivisions.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            var labelWidth = 160f;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), $"Node {index} precision: ");

            element.intValue = EditorGUI.IntField(new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight),
                element.intValue);
        }

        private void CreateBezierNode(ReorderableList list)
        {
            var node = EditorGUIUtility.Load(BEZIER_NODE_ASSET_PATH) as GameObject;
            var nodeTransform = Object.Instantiate(node).transform;
            nodeTransform.name = $"Bezier Node {nodeSubdivisions.count}";

            nodes.Add(nodeTransform);

            // Add element to reorderable list
            list.serializedProperty.arraySize++;
        }

        private void RemoveBezierNode(ReorderableList list)
        {
            var index = list.index;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.DeleteCommand();

            subdivisions.RemoveAt(index);

            var nodeToRemove = nodes[index];
            nodes.RemoveAt(index);
            DestroyImmediate(nodeToRemove.gameObject);
        }

        private void ReorderSubdivisions(ReorderableList list)
        {
            subdivisions.Clear();
            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(i).intValue;
                subdivisions.Add(element);
            }

            SceneView.RepaintAll();
        }

        #endregion
    }
}