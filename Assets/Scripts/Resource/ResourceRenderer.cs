using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[ExecuteInEditMode]
public class ResourceRenderer : MonoBehaviour
{
    public bool drawGizmo = true;
    public Vector3 offset;
    [Range(-360, 360)] public float rotation = 0;
    public int TreeCount = 10;
    public int size = 1;
    public float sizeX = 0.5f;
    public float sizeZ = 0.5f;
    [Range(-10,10)]public int seed = 0;
    public int StartHeight = 1000;
    public int lineTraceLength = -1000;

    private List<GameObject> trees = new List<GameObject>();
    private List<Vector3> points = new List<Vector3>();
    [HideInInspector] public GameObject[] resourcePrefab;

    public void PlaceTrees()
    {
        Vector3 origin = transform.position;

        origin.y = StartHeight;

        for (int i = 0; i < points.Count; i++)
        {
            int rand = Random.Range(0, resourcePrefab.Length);
            GameObject tree = Instantiate(resourcePrefab[rand], transform);
            tree.transform.position = points[i];
            tree.transform.Rotate(0, Random.Range(0, 359), 0);
            tree.transform.localScale = Vector3.one * Random.Range(1.9f, 2.3f);
            tree.name = "Tree";
            trees.Add(tree);
        }
    }

    public void OnDrawGizmos()
    {
        Random.InitState(seed);
        
        points.Clear();

        Vector3 origin = transform.position;

        origin.y = StartHeight;

        for (int i = 0; i < TreeCount; i++)
        {
            Vector3 position = origin;

            Vector2 randomPoint = new(Random.Range(-sizeX, sizeX), Random.Range(-sizeZ, sizeZ));
            var angulo = Mathf.Atan2(randomPoint.normalized.y, randomPoint.normalized.x);

            position.x += Mathf.Cos(angulo - rotation * Mathf.Deg2Rad) * randomPoint.magnitude * size;
            position.z += Mathf.Sin(angulo - rotation * Mathf.Deg2Rad) * randomPoint.magnitude * size;

            position += offset;

            RaycastHit hit;
            if (Physics.Linecast(position, position + new Vector3(0, lineTraceLength, 0), out hit, -LayerMask.NameToLayer("Ground")))
            {
                points.Add(hit.point);
            }
        }
        if (!drawGizmo) return;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(points[i], 1);
        }

        #region BOUNDS

        Vector3[] bounds = new Vector3[4];

        for(int i = 0; i < 4; i++)
        {
            bounds[i] = transform.position;

            Vector2 rectangleSize = new(sizeX, sizeZ);
            rectangleSize.Normalize();
            var angulo = Mathf.Atan2(rectangleSize.y * (i is 2 or 3 ? -1 : 1), rectangleSize.x * (i is 1 or 2 ? -1 : 1));
            // Criação do quadrado
            rectangleSize = new(sizeX, sizeZ);
            bounds[i].x += Mathf.Cos(angulo - rotation * Mathf.Deg2Rad) * rectangleSize.magnitude * size;
            bounds[i].z += Mathf.Sin(angulo - rotation * Mathf.Deg2Rad) * rectangleSize.magnitude * size;
            // Translação do quadrado
            bounds[i] += offset;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bounds[0], bounds[1]);
        Gizmos.DrawLine(bounds[1], bounds[2]);
        Gizmos.DrawLine(bounds[2], bounds[3]);
        Gizmos.DrawLine(bounds[3], bounds[0]);
        #endregion
    }

    public void ClearTrees()
    {
        if (trees.Count == 0) foreach (var tree in GetComponentsInChildren<Transform>()) if(tree.gameObject != gameObject) trees.Add(tree.gameObject);
        
        foreach (GameObject tree in trees)
        {
            DestroyImmediate(tree.gameObject);
        }

        trees.Clear();
    }
}