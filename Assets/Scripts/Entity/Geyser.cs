using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField] private float verticalImpulse = 30f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<Player>().Geyser(verticalImpulse);
    }
}
