using UnityEditor;
using TowerDefense.GameLogic.Runtime.Configs;

namespace TowerDefense.GameLogic.Editor
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var spawnConditionProperty = serializedObject.FindProperty("winCondition");
            EditorGUILayout.PropertyField(spawnConditionProperty);

            switch (spawnConditionProperty.enumValueIndex)
            {
                case 0:
                    var timeToSurviveProperty = serializedObject.FindProperty("timeToSurvive");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Time To Survive");
                    timeToSurviveProperty.floatValue = EditorGUILayout.FloatField(timeToSurviveProperty.floatValue);
                    EditorGUILayout.EndHorizontal();
                    break;
            }

            var healthProperty = serializedObject.FindProperty("health");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health");
            healthProperty.floatValue = EditorGUILayout.FloatField(healthProperty.floatValue);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
