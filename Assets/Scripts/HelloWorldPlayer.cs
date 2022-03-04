using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        private void Update()
        {
            var speed = 3 * Time.deltaTime;

            transform.position += Input.GetKey(KeyCode.W) ? transform.forward * speed : Vector3.zero;
            transform.position += Input.GetKey(KeyCode.D) ? transform.right * speed : Vector3.zero;
            transform.position += Input.GetKey(KeyCode.S) ? -transform.forward * speed : Vector3.zero;
            transform.position += Input.GetKey(KeyCode.A) ? -transform.right * speed : Vector3.zero;
        }
    }
}