using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnvironmentControlBehaviour))]
public class EnvironmentControlDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 9;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty useFogProp = property.FindPropertyRelative("useFog");
        SerializedProperty fogModeProp = property.FindPropertyRelative("fogMode");
        SerializedProperty fogColorProp = property.FindPropertyRelative("fogColor");
        SerializedProperty densityProp = property.FindPropertyRelative("density");
        SerializedProperty startDistanceProp = property.FindPropertyRelative("startDistance");
        SerializedProperty endDistanceProp = property.FindPropertyRelative("endDistance");
        SerializedProperty targetCameraProp = property.FindPropertyRelative("targetCamera");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Use Fog
        EditorGUI.PropertyField(singleFieldRect, useFogProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // Fog Mode
        EditorGUI.PropertyField(singleFieldRect, fogModeProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // Fog Color
        EditorGUI.PropertyField(singleFieldRect, fogColorProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // Density
        EditorGUI.PropertyField(singleFieldRect, densityProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // Start Distance
        EditorGUI.PropertyField(singleFieldRect, startDistanceProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // End Distance
        EditorGUI.PropertyField(singleFieldRect, endDistanceProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;

        // Target Cameras
        EditorGUI.PropertyField(singleFieldRect, targetCameraProp);
    }
}
