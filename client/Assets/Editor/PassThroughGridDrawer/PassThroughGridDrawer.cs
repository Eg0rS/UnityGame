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

            SerializedProperty grid = property.FindPropertyRelative("_cellDatas");
            for (int i = 0; i < grid.arraySize; i++) {
                SerializedProperty row = grid.GetArrayElementAtIndex(i).FindPropertyRelative("_isFilled");
                if (i % 3 == 0) {
                    contentPosition.y = position.y + 5 + CELL_SIZE * i / 3;
                    contentPosition.x = CELL_SIZE;
                }
                EditorGUI.PropertyField(contentPosition, row, GUIContent.none);
                contentPosition.x += CELL_SIZE;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CELL_SIZE * MATRIX_SIZE;
        }
    }
}