using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Connectable))]
[CanEditMultipleObjects]
public class ConnectableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Connectable connectable = (Connectable) target;
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Connect to surroundings"))
        {
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                gameObject.GetComponent<Connectable>().Connect();
            }
        }
    }
}