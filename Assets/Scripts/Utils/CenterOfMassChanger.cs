using UnityEngine;

public class CenterOfMassChanger : MonoBehaviour
{
    [SerializeField] private Vector3 com = new(0, 3.5f, 0);
    
    private void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = com;
    }
}
