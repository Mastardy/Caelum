using System;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        
        [SerializeField] private float sensitivity;
        [SerializeField] private float speed;

        private Vector3 lastMousePosition;
        
        private void Start()
        {
            lastMousePosition = Input.mousePosition;
            
            if(Camera.main) if(Camera.main != playerCamera) Camera.main.enabled = false;
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;

            transform.position += CalculateMovement() * Time.deltaTime;

            Vector2 aim = CalculateAim();
            
            playerCamera.transform.Rotate(Vector3.right, aim.x);
            transform.Rotate(Vector3.up, aim.y);
        }

        private Vector3 CalculateMovement()
        {
            var movement = new Vector3();
            
            movement += Input.GetKey(KeyCode.W) ? transform.forward : Vector3.zero;
            movement += Input.GetKey(KeyCode.D) ? transform.right : Vector3.zero;
            movement += Input.GetKey(KeyCode.S) ? -transform.forward : Vector3.zero;
            movement += Input.GetKey(KeyCode.A) ? -transform.right : Vector3.zero;
            
            movement.Normalize();

            return movement * speed;
        }

        private Vector2 CalculateAim()
        {
            var aim = new Vector2();

            aim.y = Input.mousePosition.x - lastMousePosition.x;
            aim.x = lastMousePosition.y - Input.mousePosition.y;

            lastMousePosition = Input.mousePosition;
            
            return aim * sensitivity;
        }
    }
}