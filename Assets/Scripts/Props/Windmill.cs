using UnityEngine;

public class Windmill : MonoBehaviour
{
    public GameObject blades;
    private float xRotation;
    [Range(-1,1)] public float speed = 0.2f;
    public float perlinScale = 2;
    public Vector3 defaultRotation = new Vector3(0, -90, 0);
    public Vector3 rotationMultiplier = new Vector3(1, 0, 0);
    
    void Start()
    {
        xRotation = Random.Range(0, 359);
        blades.transform.localRotation = Quaternion.Euler(xRotation * rotationMultiplier.x, defaultRotation.y + (xRotation * rotationMultiplier.y), defaultRotation.z + (xRotation * rotationMultiplier.z));
    }
    void FixedUpdate()
    {
        xRotation += speed * Mathf.PerlinNoise(0, Time.time / perlinScale) * 2.0f;
        blades.transform.localRotation = Quaternion.Euler(xRotation * rotationMultiplier.x, defaultRotation.y + (xRotation * rotationMultiplier.y), defaultRotation.z + (xRotation * rotationMultiplier.z));
        if (xRotation >= 360) xRotation = 0;
    }
}
