using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RebuildPlane))]
public class rebuild_sphereEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset Plane"))
        {
            RebuildPlane.ResetPlane(Selection.activeGameObject.GetComponent<RebuildPlane>().MF.sharedMesh);
        }
    }
}