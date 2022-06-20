using UnityEngine;

public class Crop : MonoBehaviour
{
    public float timeToGrow;
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
    }
}