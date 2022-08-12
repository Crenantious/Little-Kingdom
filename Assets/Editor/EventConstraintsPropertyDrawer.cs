using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EventConstraints))]
public class EventConstraintPropertyDrawer : PropertyDrawer
{
    SerializedProperty statesAllowed;
    SerializedProperty statesNotAllowed;
    SerializedProperty gameObjectConstraint;
    int lineSpacing = 1;
    float propertyHeight;
    Rect propertyRect;
    bool isSetup = false;

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        propertyHeight = 0;
        propertyRect = rect;

        property.serializedObject.Update();

        if (!isSetup)
        {
            statesAllowed = property.FindPropertyRelative("statesAllowed");
            statesNotAllowed = property.FindPropertyRelative("statesNotAllowed");
            gameObjectConstraint = property.FindPropertyRelative("gameObjectConstraints");
        }

        EditorGUI.LabelField(GetNextLineRect(), "GameState constraints");

        EditorGUI.PropertyField(GetNextLineRect(statesAllowed), statesAllowed);
        EditorGUI.PropertyField(GetNextLineRect(statesNotAllowed), statesNotAllowed);
        EditorGUI.LabelField(GetNextLineRect(), "EventInfo constraints");
        EditorGUI.PropertyField(GetNextLineRect(gameObjectConstraint), gameObjectConstraint);

        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }

    Rect GetNextLineRect(SerializedProperty property = null)
    {
        Rect rect = new(propertyRect.x, propertyRect.y + propertyHeight, propertyRect.width, EditorGUIUtility.singleLineHeight);

        propertyHeight += property == null ? EditorGUIUtility.singleLineHeight + lineSpacing : EditorGUI.GetPropertyHeight(property);

        return rect;
    }
}