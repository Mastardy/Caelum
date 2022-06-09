using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimationEvents : MonoBehaviour
{
    public Animal animal;

    public void TryAttack()
    {
        animal.TryAttack();
    }
}
