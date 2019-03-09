using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AdjacentData))]
public class AdjacentDataDrawer : PropertyDrawer
{
    private SerializedProperty NorthProp;
    private SerializedProperty EastProp;
    private SerializedProperty SouthProp;
    private SerializedProperty WestProp;

    private SerializedProperty NorthEastProp;
    private SerializedProperty SouthEastProp;
    private SerializedProperty SouthWestProp;
    private SerializedProperty NorthWestProp;

    private string name;
    private bool cache = false;

    private bool foldout;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!cache)
        {
            name = property.displayName;

            property.Next(true);
            property.Next(true);
            NorthProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            EastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            SouthProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            WestProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            NorthEastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            SouthEastProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            SouthWestProp = property.Copy();
            property.Next(true);
            property.Next(true);
            property.Next(true);
            NorthWestProp = property.Copy();

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
            EditorGUI.BeginProperty(contentPosition, label, NorthProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, NorthProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, EastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, EastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, SouthProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, SouthProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, WestProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, WestProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();

            contentPosition.y += 36f;
            EditorGUI.BeginProperty(contentPosition, label, NorthEastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, NorthEastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, SouthEastProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, SouthEastProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, SouthWestProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, SouthWestProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
            contentPosition.y += 18f;
            EditorGUI.BeginProperty(contentPosition, label, NorthWestProp);
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(contentPosition, NorthWestProp, typeof(Connectable));
            }
            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!foldout) return 16f;
        return 16f + 18f * 9;
    }
}