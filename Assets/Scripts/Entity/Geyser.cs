using UnityEngine;

public class Geyser : MonoBehaviour
{
    [SerializeField] private float verticalImpulse = 30f;
    private float lastImpulse;
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastImpulse > 1.0f)
        {
            if (other.GetComponent<Player>().Geyser(verticalImpulse))
            {
                lastImpulse = Time.time;                
            }
        }
    }
}
