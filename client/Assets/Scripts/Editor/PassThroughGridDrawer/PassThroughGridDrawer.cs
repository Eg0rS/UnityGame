using Drone.Location.Model.Obstacle;
using UnityEditor;
using UnityEngine;

namespace Editor.PassThroughGridDrawer
{
    [CustomPropertyDrawer((typeof(PassThroughGrid)))]
    public class PassThroughGridDrawer : PropertyDrawer
    {
        private const float CELL_SIZE = 50f;
        private const int MATRIX_SIZE = 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            contentPosition.width = CELL_SIZE;
            contentPosition.height = CELL_SIZE;

            SerializedProperty grid = property.FindPropertyRelative("rows");
            grid.arraySize = MATRIX_SIZE;
            for (int i = 0; i < grid.arraySize; i++) {
                SerializedProperty row = grid.GetArrayElementAtIndex(i).FindPropertyRelative("columns");
                row.arraySize = MATRIX_SIZE;
                contentPosition.y = position.y + 5 + CELL_SIZE * i;
                contentPosition.x = CELL_SIZE;
                for (int j = 0; j < row.arraySize; j++) {
                    EditorGUI.PropertyField(contentPosition, row.GetArrayElementAtIndex(j), GUIContent.none);
                    contentPosition.x += CELL_SIZE;
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CELL_SIZE * MATRIX_SIZE;
        }
    }
}