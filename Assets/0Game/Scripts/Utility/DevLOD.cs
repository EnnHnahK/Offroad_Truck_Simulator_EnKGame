using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DevLOD : MonoBehaviour
{
    public float percent = 6;
    public bool child = true;
    public bool findAllChild;
    public bool add = false;
    public List<Transform> listT;
    
    [Button]
    void SetLOD()
    {
        if (findAllChild)
        {
            CheckChild(transform);
            return;
        }
        if (child)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<LODGroup>())
                {
                    LOD[] lods = new LOD[1];
                    
                    Renderer[] renderers = new Renderer[1];
                    renderers[0] = child.GetComponent<Renderer>();

                    lods[0] = new LOD(percent/100f, renderers);
                
                    child.GetComponent<LODGroup>().SetLODs(lods);
                    Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
                    continue;
                }
                if (add)
                {
                    LOD[] lods = new LOD[1];
                    
                    Renderer[] renderers = new Renderer[1];
                    renderers[0] = child.GetComponent<Renderer>();

                    lods[0] = new LOD(percent/100f, renderers);
                    var group = child.AddComponent<LODGroup>();
                    group.SetLODs(lods);
                    Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
                    continue;
                }
            }
        }
        else
        {
            foreach (Transform child in listT)
            {
                if (child.GetComponent<LODGroup>())
                {
                    LOD[] lods = new LOD[1];
                    
                    Renderer[] renderers = new Renderer[1];
                    renderers[0] = child.GetComponent<Renderer>();

                    lods[0] = new LOD(percent/100f, renderers);
                
                    child.GetComponent<LODGroup>().SetLODs(lods);
                    Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
                    continue;
                }
                if (add)
                {
                    LOD[] lods = new LOD[1];
                    
                    Renderer[] renderers = new Renderer[1];
                    renderers[0] = child.GetComponent<Renderer>();

                    lods[0] = new LOD(percent/100f, renderers);
                
                    var group = child.AddComponent<LODGroup>();
                    group.SetLODs(lods);
                    Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
                    continue;
                }
            }
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    void CheckChild(Transform t)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                CheckLOD(child);
                CheckChild(child);
            }
        }
    }

    void CheckLOD(Transform child)
    {
        if (child.GetComponent<LODGroup>())
        {
            LOD[] lods = new LOD[1];
                    
            Renderer[] renderers = new Renderer[1];
            renderers[0] = child.GetComponent<Renderer>();

            lods[0] = new LOD(percent/100f, renderers);
                
            child.GetComponent<LODGroup>().SetLODs(lods);
            Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
        }else if (add && child.GetComponent<MeshRenderer>())
        {
            LOD[] lods = new LOD[1];
                    
            Renderer[] renderers = new Renderer[1];
            renderers[0] = child.GetComponent<Renderer>();

            lods[0] = new LOD(percent/100f, renderers);
            var group = child.AddComponent<LODGroup>();
            group.SetLODs(lods);
            Debug.LogError(child.GetComponent<LODGroup>().GetLODs()[0].renderers.Length);
        }
    }
    
    [Button]
    public void ClearComponent(){
        CheckChildClear(transform);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    
    void CheckChildClear(Transform t)
    {
        if (t.childCount > 0)
        {
            foreach (Transform child in t)
            {
                if (!child.gameObject.activeSelf)
                {
                    DestroyImmediate(child.GetComponent<MeshRenderer>());
                    DestroyImmediate(child.GetComponent<MeshFilter>());
                    DestroyImmediate(child.GetComponent<LODGroup>());
                }
                CheckChildClear(child);
            }
        }
    }
}
