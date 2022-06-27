using UnityEngine;

public class Crop : MonoBehaviour
{
    [HideInInspector] public float timeToGrow;
    private float plantedTime;
    public float percentage;
    private Vector3 baseScale;

    public void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void Update()
    {
        if(baseScale.magnitude < transform.localScale.magnitude) return;
        transform.localScale += baseScale * Time.deltaTime / timeToGrow;
        plantedTime += Time.deltaTime;
        percentage = plantedTime/timeToGrow;
    }
}