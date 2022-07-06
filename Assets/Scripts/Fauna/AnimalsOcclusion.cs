using UnityEngine.AI;
using UnityEngine;

public class AnimalsOcclusion : MonoBehaviour
{
    public GameObject[] animals;
    public bool hideAtStart;

    private void Start()
    {
        if (hideAtStart) Invoke("HideAtStart", 1f);
    }

    private void HideAtStart()
    {
        Debug.Log("hiding");
        foreach (var animal in animals)
        {
            animal.SetActive(false);
            Debug.Log("hidden");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var animal in animals)
                animal.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var animal in animals)
                if(!animal.GetComponent<Animal>().dead)
                    animal.SetActive(true);
        }
    }
}
