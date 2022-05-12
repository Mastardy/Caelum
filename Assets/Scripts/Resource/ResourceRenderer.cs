using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[ExecuteInEditMode]
public class ResourceRenderer : MonoBehaviour
{
    public bool Place = false;
    public bool Clear = false;
    public GameObject[] ResourcePrefab;
    public int TreeCount = 10;
    public int StartHeight = 1000;
    public int size = 1;
    [Range(-10,10)]public int seed = 0;
    public float distancex = 0.5f;
    public float distancez = 0.5f;

    private List<GameObject> trees = new List<GameObject>();
    private List<Vector3> points = new List<Vector3>();

    private void Update()
    {
        if (Place)
        {
            Place = false;
            for(int i = 0; i < 5; i++)
                PlaceTrees();
        }
        if (Clear)
        {
            Clear = false;
            ClearTrees();
        }
    }

    private void PlaceTrees()
    {
        ClearTrees();

        Vector3 origin = transform.position;

        origin.y = StartHeight;

        for (int i = 0; i < points.Count; i++)
        {
            int rand = Random.Range(0, ResourcePrefab.Length);
            GameObject tree = Instantiate(ResourcePrefab[rand], transform);
            tree.transform.position = points[i];
            tree.transform.Rotate(0, Random.Range(0, 359), 0);
            tree.transform.localScale = Vector3.one * Random.Range(1.3f, 1.5f);
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
            position.x += size * Random.Range(-distancex, distancex);
            position.z += size * Random.Range(-distancez, distancez);

            Vector2 rotatedPoint = RotatePoint(new Vector2(position.x, position.z), new Vector2(transform.position.x, transform.position.z), -transform.rotation.eulerAngles.y);

            position.x = rotatedPoint.x;
            position.z = rotatedPoint.y;

            RaycastHit hit;
            if (Physics.Linecast(position, position + new Vector3(0,-1000,0), out hit, -LayerMask.NameToLayer("Ground")))
            {
                points.Add(hit.point);
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(points[i], 1);
        }

        #region BOUNDS

        Vector3 bound1 = transform.position;
        bound1.x += distancex * size;
        bound1.z += distancez * size;
        Vector2 rotatedBound = (RotatePoint(new Vector2(bound1.x, bound1.z), new Vector2(transform.position.x, transform.position.z), -transform.rotation.eulerAngles.y));
        bound1.x = rotatedBound.x;
        bound1.z = rotatedBound.y;

        Vector3 bound2 = transform.position;
        bound2.x += -distancex * size;
        bound2.z += distancez * size;
        rotatedBound = (RotatePoint(new Vector2(bound2.x, bound2.z), new Vector2(transform.position.x, transform.position.z), -transform.rotation.eulerAngles.y));
        bound2.x = rotatedBound.x;
        bound2.z = rotatedBound.y;

        Vector3 bound3 = transform.position;
        bound3.x += distancex * size;
        bound3.z += -distancez * size;
        rotatedBound = (RotatePoint(new Vector2(bound3.x, bound3.z), new Vector2(transform.position.x, transform.position.z), -transform.rotation.eulerAngles.y));
        bound3.x = rotatedBound.x;
        bound3.z = rotatedBound.y;

        Vector3 bound4 = transform.position;
        bound4.x += -distancex * size;
        bound4.z += -distancez * size;
        rotatedBound = (RotatePoint(new Vector2(bound4.x, bound4.z), new Vector2(transform.position.x, transform.position.z), -transform.rotation.eulerAngles.y));
        bound4.x = rotatedBound.x;
        bound4.z = rotatedBound.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bound3, bound1);
        Gizmos.DrawLine(bound2, bound4);
        Gizmos.DrawLine(bound1, bound2);
        Gizmos.DrawLine(bound3, bound4);
        #endregion
    }

    static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * (Mathf.PI / 180);
        float cosTheta = Mathf.Cos(angleInRadians);
        float sinTheta = Mathf.Sin(angleInRadians);
        return new Vector2((cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                (sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y));
    }

    private void ClearTrees()
    {
        if (trees.Count > 0)
            foreach (GameObject tree in trees)
            {
                DestroyImmediate(tree.gameObject);
            }
        trees.Clear();
    }
}