using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        private void Update()
        {
            var speed = 3 * Time.deltaTime;
            
            transform.position += Input.GetKey(KeyCode.W) ? transform.forward * speed : 
                Input.GetKey(KeyCode.D) ? transform.right * speed : 
                Input.GetKey(KeyCode.S) ? -transform.forward * speed :
                Input.GetKey(KeyCode.A) ? -transform.right * speed : Vector3.zero;
        }
    }
}