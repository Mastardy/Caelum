using UnityEngine.AI;
using UnityEngine;

public class AnimalsOcclusion : MonoBehaviour
{
    public GameObject[] animals;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var animal in animals)
                animal.GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var animal in animals)
            {
                if(!animal.GetComponent<Animal>().dead)
                    animal.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
    }
}
