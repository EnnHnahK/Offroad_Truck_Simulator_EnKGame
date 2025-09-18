using UnityEditor;
using UnityEngine;

public class DebugTimeScale : EditorWindow
{
    [MenuItem("Window/Debug/Debug Time Scale")]
    public static void ShowWindow()
    {
        GetWindow<DebugTimeScale>("Debug Time Scale");
    }

    void OnGUI()
    {
        GUILayout.Label("Time Scale Debugger", EditorStyles.boldLabel);
        
        Debug.Log(Time.timeScale);
       
    }
}
