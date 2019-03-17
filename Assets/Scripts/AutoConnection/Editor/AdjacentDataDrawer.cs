using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AdjacentData))]
public class AdjacentDataDrawer : PropertyDrawer
{
    private SerializedProperty directionsProp;

    private SerializedProperty northProp;
    private SerializedProperty eastProp;
    private SerializedProperty southProp;
    private SerializedProperty westProp;

    private SerializedProperty northEastProp;
    private SerializedProperty southEastProp;
    private SerializedProperty southWestProp;
    private SerializedProperty northWestProp;

    private string name;
    private bool cache = false;

    private bool foldout;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            name = property.displayName;

            property.Next(true);
            directionsProp = property.Copy();
            property.Next(true);
            northProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            eastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            southProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            westProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            northEastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            southEastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            southWestProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            northWestProp = property.Copy();

            cache = true;
        }

        Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

        position.height = 16f;
        foldout = EditorGUI.Foldout(position, foldout, label);
        EditorGUI.indentLevel += 1;
        contentPosition = EditorGUI.IndentedRect(position);
        contentPosition.y += 18f;

        EditorGUIUtility.labelWidth = 75f;
        EditorGUI.indentLevel = 0;
        
        if (foldout)
        {
            EditorGUI.BeginProperty(contentPosition, label, northProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, northProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, eastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, eastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, southProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, southProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, westProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, westProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, northEastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, northEastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, southEastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, southEastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, southWestProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, southWestProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, northWestProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, northWestProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
        }
        
        EditorGUI.BeginProperty(contentPosition, label, directionsProp);
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(contentPosition, directionsProp, new GUIContent(""));
        }
        EditorGUI.EndProperty();
        contentPosition.y += 32f;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!foldout) return 16f;
        return 16f + 18f * 8;
    }
}