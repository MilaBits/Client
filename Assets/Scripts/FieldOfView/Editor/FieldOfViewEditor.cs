using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private FieldOfView fov;

    private SerializedProperty detectionOffsetProp;
    private SerializedProperty viewConeWidthProp;
    private SerializedProperty viewRadiusProp;


    private void OnEnable()
    {
        fov = (FieldOfView) target;

        detectionOffsetProp = serializedObject.FindProperty("detectionOffset");
        viewConeWidthProp = serializedObject.FindProperty("viewConeWidth");
        viewRadiusProp = serializedObject.FindProperty("viewRadius");
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();

        GUIStyle labelStyle = new GUIStyle {fontSize = 14};
        var viewPoints = fov.CalculateViewPoints();

        Handles.color = Color.gray;
        labelStyle.normal.textColor = Color.yellow;
        Handles.Label(viewPoints[0] - detectionOffsetProp.vector3Value, "Visual Cone", labelStyle);
        Vector3 viewAngleA = fov.DirectionFromAngle(-viewConeWidthProp.floatValue / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(viewConeWidthProp.floatValue / 2, false);
        Handles.DrawLine(fov.transform.position,
            fov.transform.position + viewAngleA * viewRadiusProp.floatValue);
        Handles.DrawLine(fov.transform.position,
            fov.transform.position + viewAngleB * viewRadiusProp.floatValue);


        // Draw Visual FOV outline;
        Handles.color = Color.yellow;
        for (int i = 1; i < viewPoints.Count; i++)
        {
            Handles.DrawLine(viewPoints[i] - detectionOffsetProp.vector3Value,
                viewPoints[i - 1] - detectionOffsetProp.vector3Value);
        }


        Handles.color = Color.gray;
        labelStyle.normal.textColor = Color.green;
        Handles.Label(viewPoints[0], "Detection Cone", labelStyle);
        Handles.DrawWireArc(fov.transform.position + detectionOffsetProp.vector3Value, Vector3.up, Vector3.forward, 360,
            viewRadiusProp.floatValue);

        Handles.DrawLine(fov.transform.position + detectionOffsetProp.vector3Value,
            fov.transform.position + detectionOffsetProp.vector3Value + viewAngleA * viewRadiusProp.floatValue);
        Handles.DrawLine(fov.transform.position + detectionOffsetProp.vector3Value,
            fov.transform.position + detectionOffsetProp.vector3Value + viewAngleB * viewRadiusProp.floatValue);

        // Draw Visual FOV outline;
        Handles.color = Color.green;
        for (int i = 1; i < viewPoints.Count; i++)
        {
            Handles.DrawLine(viewPoints[i], viewPoints[i - 1]);
            Handles.DrawLine(viewPoints[i], fov.transform.position + detectionOffsetProp.vector3Value);
        }
    }
}