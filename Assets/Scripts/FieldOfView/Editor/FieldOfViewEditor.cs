using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private FieldOfView fov;


    private void OnEnable()
    {
        fov = (FieldOfView) target;
    }

    private void OnSceneGUI()
    {
//        if (EditorApplication.isPlaying) return;
        
        GUIStyle labelStyle = new GUIStyle {fontSize = 14};
        var viewPoints = fov.CalculateViewPoints();

        Handles.color = Color.gray;
        labelStyle.normal.textColor = Color.yellow;
        Handles.Label(viewPoints[0] - fov.DetectionOffset, "Visual Cone", labelStyle);
        Vector3 viewAngleA = fov.DirectionFromAngle(-fov.ViewConeWidth / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(fov.ViewConeWidth / 2, false);
        Handles.DrawLine(fov.transform.position,
            fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position,
            fov.transform.position + viewAngleB * fov.ViewRadius);


        // Draw Visual FOV outline;
        Handles.color = Color.yellow;
        for (int i = 1; i < viewPoints.Count; i++)
        {
            Handles.DrawLine(viewPoints[i] - fov.DetectionOffset, viewPoints[i - 1] - fov.DetectionOffset);
        }


        Handles.color = Color.gray;
        labelStyle.normal.textColor = Color.green;
        Handles.Label(viewPoints[0], "Detection Cone", labelStyle);
        Handles.DrawWireArc(fov.transform.position + fov.DetectionOffset, Vector3.up, Vector3.forward, 360,
            fov.ViewRadius);

        Handles.DrawLine(fov.transform.position + fov.DetectionOffset,
            fov.transform.position + fov.DetectionOffset + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position + fov.DetectionOffset,
            fov.transform.position + fov.DetectionOffset + viewAngleB * fov.ViewRadius);

        // Draw Visual FOV outline;
        Handles.color = Color.green;
        for (int i = 1; i < viewPoints.Count; i++)
        {
            Handles.DrawLine(viewPoints[i], viewPoints[i - 1]);
            Handles.DrawLine(viewPoints[i], fov.transform.position + fov.DetectionOffset);
        }
    }
}