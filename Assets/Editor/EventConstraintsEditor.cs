//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(EventConstraints))]
//public class EventConstraintEditor : PropertyDrawer
//{
//    SerializedProperty statesAllowed;
//    SerializedProperty statesNotAllowed;
//    SerializedProperty gameObjectConstraint;
//    int numberOfLines;
//    int lineSpacing = 1;
//    float propertyHeight;
//    SerializedProperty property;
//    Rect propertyRect;
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        numberOfLines = 0;
//        propertyHeight = 0;

//        this.property = property;
//        propertyRect = position;
//        property.serializedObject.Update();

//        statesAllowed = property.FindPropertyRelative("statesAllowed");
//        statesNotAllowed = property.FindPropertyRelative("statesNotAllowed");
//        gameObjectConstraint = property.FindPropertyRelative("gameObjectConstraint");

//        //var a = EditorGUI.LabelField(GetNextLineRect(), "GameState constraints");

//        EditorGUI.PropertyField(GetNextLineRect(), statesAllowed);
//        EditorGUI.PropertyField(GetNextLineRect(), statesNotAllowed);
//        EditorGUI.LabelField(GetNextLineRect(), "EventInfo constraints");
//        EditorGUI.PropertyField(GetNextLineRect(), gameObjectConstraint);

//        property.serializedObject.ApplyModifiedProperties();
//    }
//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return propertyHeight;
//    }

//    Rect GetNextLineRect()
//    {
//        Rect rect = new Rect(propertyRect.x, propertyRect.y + propertyHeight, propertyRect.width, EditorGUIUtility.singleLineHeight);
//        //propertyHeight += EditorGUIUtility.singleLineHeight + lineSpacing;
//        return rect;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventConstraints))]
public class EventConstraintsEditor : Editor
{
    SerializedProperty statesAllowed;
    SerializedProperty statesNotAllowed;
    SerializedProperty gameObjectConstraint;
    private void OnEnable()
    {
        statesAllowed = serializedObject.FindProperty("statesAllowed");
        statesNotAllowed = serializedObject.FindProperty("statesNotAllowed");
        gameObjectConstraint = serializedObject.FindProperty("gameObjectConstraint");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("GameState constraints");
        EditorGUILayout.PropertyField(statesAllowed);
        EditorGUILayout.PropertyField(statesNotAllowed);
        EditorGUILayout.LabelField("EventInfo constraints");
        EditorGUILayout.PropertyField(gameObjectConstraint);

        serializedObject.ApplyModifiedProperties();
    }
}
