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
    public Vector2 randomSize = new (1.9f, 2.3f);

    public void RandomizePoints()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.RandomizePoints();
        }
    }

    public void ShowPoints()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.drawGizmo = !r.drawGizmo;
        }
    }

    public void Place()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.resourcePrefab = resourcePrefab;
            r.PlaceTrees(randomSize);
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

    public void Rename()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.RenameTrees();
        }
    }

    public void Resize()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.ResizeTrees(randomSize);
        }
    }

    public void Rotate()
    {
        resources = GetComponents<ResourceRenderer>();
        foreach (var r in resources)
        {
            r.RotateTrees();
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
        GUILayout.Label("Visualization");
        if (GUILayout.Button(new GUIContent("Randomize Points")))
        {
            resourcesMaster.RandomizePoints();
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Toggle Show Points")))
        {
            resourcesMaster.ShowPoints();
        }

        GUILayout.Space(5);
        GUILayout.Label("Placement");
        if (GUILayout.Button(new GUIContent("Place Trees")))
        {
            resourcesMaster.Place();
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Clear Trees")))
        {
            resourcesMaster.Clear();
        }
        GUILayout.Space(5);
        GUILayout.Label("Adjustment");
        if (GUILayout.Button(new GUIContent("Rename Trees")))
        {
            resourcesMaster.Rename();
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Resize Trees")))
        {
            resourcesMaster.Resize();
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Rotate Trees")))
        {
            resourcesMaster.Rotate();
        }
    }
}
#endif