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

    private float inputX, inputZ;

    private void MovementInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
    }

    private void MovementUpdate()
    {
        inputX = 0.0f;
        inputZ = 0.0f;
        
        if(takeInput) MovementInput();
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        var playerTransform = transform;
        Vector3 move = playerTransform.right * inputX + playerTransform.forward * inputZ;

        characterController.Move(move * (speed * Time.deltaTime));
        
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
