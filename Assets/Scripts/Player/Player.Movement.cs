using UnityEngine;

public partial class Player
{
    [Header("Movement")]
    private CharacterController characterController;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 5f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    private Vector3 velocity;
    private bool isGrounded;

    private void MovementUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        var playerTransform = transform;
        Vector3 move = playerTransform.right * x + playerTransform.forward * z;

        characterController.Move(move * (speed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
