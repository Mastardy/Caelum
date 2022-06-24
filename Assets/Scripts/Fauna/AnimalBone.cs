using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBone : MonoBehaviour
{
    public Animal animalOwner;

    private void Start()
    {
        animalOwner = GetComponentInParent<Animal>();
    }
}
