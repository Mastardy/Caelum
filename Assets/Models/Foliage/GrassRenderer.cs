using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[ExecuteInEditMode]
public class GrassRenderer : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public int seed;
    public int size;

    public int grassNum;
    public float startHeight = 1000;
    private void Update()
    {
        Random.InitState(seed);
        List<Matrix4x4> matrices = new List<Matrix4x4>();
        var results = new NativeArray<RaycastHit>(grassNum, Allocator.TempJob);
        var command = new NativeArray<RaycastCommand>(grassNum, Allocator.TempJob);
        for (int i = 0; i < grassNum; i++)
        {
            Vector3 origin = transform.position;
            origin.y = startHeight;
            origin.x += size * Random.Range(-0.5f, 0.5f);
            origin.z += size * Random.Range(-0.5f, 0.5f);
            command[i] = new RaycastCommand(origin, Vector3.down);
        }
        JobHandle handle = RaycastCommand.ScheduleBatch(command, results, 1, default(JobHandle));
        handle.Complete();
        for (int i = 0; i < results.Length; i++)
        {
            RaycastHit batchedHit = results[i];
            if (results[i].collider != null)
            {
                matrices.Add(Matrix4x4.TRS(batchedHit.point, Quaternion.identity, Vector3.one));
            }
        }
        while (matrices.Count > 1023)
        {
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, matrices.GetRange(0, 1023));
            matrices.RemoveRange(0, 1023);
        }
        Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, matrices);
        results.Dispose();
        command.Dispose();
    }
}