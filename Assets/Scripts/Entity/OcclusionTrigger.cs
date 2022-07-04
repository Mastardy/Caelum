using UnityEngine;

public class OcclusionTrigger : MonoBehaviour
{
    public GameObject[] objects;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in objects)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in objects)
                obj.SetActive(true);
        }
    }
}
