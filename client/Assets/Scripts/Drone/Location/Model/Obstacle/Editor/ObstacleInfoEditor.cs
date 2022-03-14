using UnityEditor;

namespace Drone.Location.Model.Obstacle.Editor
{
    [CustomEditor(typeof(ObstacleInfo))]
    [CanEditMultipleObjects]
    public class ObstacleInfoEditor : UnityEditor.Editor
    {
        private SerializedProperty _passThroughGrids;

        private void OnEnable()
        {
            _passThroughGrids = serializedObject.FindProperty("PassThroughGrid");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_passThroughGrids);
            serializedObject.ApplyModifiedProperties();
        }
    }
}