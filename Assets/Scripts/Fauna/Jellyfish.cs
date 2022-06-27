using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    public Animal animal;
    public GameObject deathParticle;
    public Transform anchor;

    private void Start()
    {
        animal = GetComponent<Animal>();
    }

    private void Update()
    {
        if (animal.dead)
        {
            GameObject.Instantiate(deathParticle, anchor);
            Destroy(gameObject);
        }
    }
}
