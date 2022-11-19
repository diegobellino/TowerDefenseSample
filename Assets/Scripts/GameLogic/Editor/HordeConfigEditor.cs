using UnityEditor;
using TowerDefense.GameLogic.Runtime.Configs;

namespace TowerDefense.GameLogic.Editor
{
    [CustomEditor(typeof(HordeConfig))]
    public class HordeConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var spawnConditionProperty = serializedObject.FindProperty("spawnCondition");
            EditorGUILayout.PropertyField(spawnConditionProperty);

            switch (spawnConditionProperty.enumValueIndex)
            {
                case 0:
                    var timeToSpawnProperty = serializedObject.FindProperty("timeToSpawn");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Time To Start Spawning");
                    timeToSpawnProperty.floatValue = EditorGUILayout.FloatField(timeToSpawnProperty.floatValue);
                    EditorGUILayout.EndHorizontal();
                    break;
                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnAfterDefeated"));
                    break;
            }

            var timeBetweenSpawnsProperty = serializedObject.FindProperty("timeBetweenSpawns");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time Between Spawns");
            timeBetweenSpawnsProperty.floatValue = EditorGUILayout.FloatField(timeBetweenSpawnsProperty.floatValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemies"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}