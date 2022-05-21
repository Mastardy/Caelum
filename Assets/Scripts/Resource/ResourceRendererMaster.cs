#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[ExecuteInEditMode]
public class ResourceRendererMaster : MonoBehaviour
{
    private ResourceRenderer[] resources;
    public GameObject[] resourcePrefab;

    public void RandomizePoints()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.RandomizePoints();
        }
    }
    public void Place()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.resourcePrefab = resourcePrefab;
            r.PlaceTrees();
        }
    }

    public void Clear()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.ClearTrees();
        }
    }
}

[CustomEditor(typeof(ResourceRendererMaster), true)]
[CanEditMultipleObjects]
public class ResourceRendererMasterEditor : Editor
{
    private ResourceRendererMaster resourcesMaster;
    
    public override void OnInspectorGUI()
    {
        resourcesMaster = (ResourceRendererMaster)target;

        base.OnInspectorGUI();

        if (GUILayout.Button(new GUIContent("Randomize Points")))
        {
            resourcesMaster.RandomizePoints();
        }

        if (GUILayout.Button(new GUIContent("Place Trees")))
        {
            resourcesMaster.Place();
        }
        if (GUILayout.Button(new GUIContent("Clear Trees")))
        {
            resourcesMaster.Clear();
        }        
    }
}
#endif