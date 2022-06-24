using UnityEngine;

public class AnimalAnimationEvents : MonoBehaviour
{
    public Animal animal;

    private void Start()
    {
        animal = GetComponentInParent<Animal>();
    }

    public void TryAttack()
    {
        animal.TryAttack();
    }
}
