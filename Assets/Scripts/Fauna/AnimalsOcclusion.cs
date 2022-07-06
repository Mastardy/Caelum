using UnityEngine.AI;
using UnityEngine;

public class AnimalsOcclusion : MonoBehaviour
{
    public GameObject[] animals;
    public bool hideAtStart;
    public StartPoint startPoint;

    private void Start()
    {
        startPoint.OnStart.AddListener(DelayedHideAtStart);
    }

    public void DelayedHideAtStart()
    {
        Invoke("HideAtStart", 5f);
    }

    private void HideAtStart()
    {
        foreach (var animal in animals)
        {
            animal.SetActive(false);
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
