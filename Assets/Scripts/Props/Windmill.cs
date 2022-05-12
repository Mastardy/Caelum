using UnityEngine;

public class Windmill : MonoBehaviour
{
    public GameObject blades;
    private float xRotation;
    [Range(-1,1)] public float speed = 0.2f;
    public float perlinScale = 2;
    
    void Start()
    {
        xRotation = Random.Range(0, 359);
        blades.transform.localRotation = Quaternion.Euler(xRotation,-90,0);
        speed = Random.Range(-1.0f, -0.2f);
    }
    void FixedUpdate()
    {
        xRotation += speed * Mathf.PerlinNoise(0, Time.time / perlinScale) * 2.0f;
        blades.transform.localRotation = Quaternion.Euler(xRotation,-90,0);
        if (xRotation >= 360) xRotation = 0;
    }
}
