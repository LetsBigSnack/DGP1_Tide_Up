using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmoothCameraMove))]
public class SmoothCameraMoveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(8);
        
        SmoothCameraMove moveScript = (SmoothCameraMove)target;

        bool wasEnabled = GUI.enabled;
        if (!EditorApplication.isPlaying || moveScript.pointA == null || moveScript.pointB == null)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Start Move"))
        {
            moveScript.TriggerMove();
        }
        
        if (GUILayout.Button("Reset"))
        {
            moveScript.Reset();
        }

        GUI.enabled = wasEnabled;
    }
}