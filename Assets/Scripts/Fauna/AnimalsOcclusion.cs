using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsOcclusion : MonoBehaviour
{
    public GameObject[] animals;

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
            {
                if(!animal.GetComponent<Animal>().dead)
                    animal.SetActive(true);
            }
        }
    }
}
