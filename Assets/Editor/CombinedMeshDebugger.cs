#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CombinedMeshDebugger : EditorWindow
{
    [MenuItem("Window/Debug/Combined Mesh Debugger")]
    public static void ShowWindow()
    {
        GetWindow<CombinedMeshDebugger>("Combined Mesh Debugger");
    }

    private int combinedMeshCount = 0;
    private List<string> combinedMeshNames = new List<string>();
    private Dictionary<string, GameObject> sampleCombinedMeshObjects = new Dictionary<string, GameObject>();
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Combined Mesh Debugger", EditorStyles.boldLabel);

        if (GUILayout.Button("Count Combined Meshes"))
        {
            CountCombinedMeshes();
        }

        GUILayout.Label("Number of combined meshes: " + combinedMeshCount);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(600));

        foreach (var meshName in combinedMeshNames)
        {
            GUILayout.Label("Combined Mesh Name: " + meshName);
            if (sampleCombinedMeshObjects.TryGetValue(meshName, out var sampleObject))
            {
                GUILayout.Label("Sample Object: " + sampleObject.name);
                if (GUILayout.Button("Select " + sampleObject.name))
                {
                    Selection.activeGameObject = sampleObject;
                }
            }
        }

        GUILayout.EndScrollView();
    }

    private void CountCombinedMeshes()
    {
        combinedMeshCount = 0;
        combinedMeshNames.Clear();
        sampleCombinedMeshObjects.Clear();

        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.StartsWith("Combined Mesh (root: scene)"))
            {
               
                string meshName = meshFilter.sharedMesh.name;

                if (!combinedMeshNames.Contains(meshName))
                {
                    combinedMeshCount++;
                    combinedMeshNames.Add(meshName);
                    sampleCombinedMeshObjects[meshName] = meshFilter.gameObject;
                }
            }
        }

        Debug.Log("Number of combined meshes in the scene: " + combinedMeshCount);
    }
}

#endif
