using UnityEditor;

namespace Drone.Location.Model.Obstacle.Editor
{
    [CustomEditor(typeof(ObstacleInfo))]
    [CanEditMultipleObjects]
    public class ObstacleInfoEditor : UnityEditor.Editor
    {
        private ObstacleInfo _subject;
        private SerializedProperty _depth;
        private SerializedProperty _passThroughGrids;

        private void OnEnable()
        {
            _subject = target as ObstacleInfo;
            _depth = serializedObject.FindProperty("Depth");
            _passThroughGrids = serializedObject.FindProperty("PassThroughGrids");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_passThroughGrids);
            EditorGUILayout.PropertyField(_depth);
            if (_subject.PassThroughGrids.Count < _subject.Depth) {
                while (_subject.PassThroughGrids.Count != _subject.Depth) {
                    _subject.PassThroughGrids.Add(new PassThroughGrid());
                }
            }
            if (_subject.PassThroughGrids.Count > _subject.Depth) {
                while (_subject.PassThroughGrids.Count != _subject.Depth) {
                    _subject.PassThroughGrids.RemoveAt(_subject.PassThroughGrids.Count - 1);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}