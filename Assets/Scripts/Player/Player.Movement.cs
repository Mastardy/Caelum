using UnityEngine;
using UnityEngine.UI;

public partial class Player
{
    [Header("Movement")]
    private CharacterController characterController;

    [SerializeField] private float crouchSpeed = 1;
    [SerializeField] private float speed = 3;
    [SerializeField] private float sprintSpeed = 5;
    [SerializeField] private float accelerationEasing = 5f;
    [SerializeField] private float gravity = -30;
    [SerializeField] private float jumpHeight = 1.75f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;
    
    private Vector2 horizontalVelocity;
    private float verticalVelocity;
    
    private bool isGrounded;
    private bool isCrouched;
    private bool isSprinting;

    private Vector2 input;
    
    private void MovementUpdate()
    {
        input.x = 0;
        input.y = 0;

        if (takeInput)
        {
            MovementInput();
        }

        playerCamera.localPosition = new Vector3(0, isCrouched ? 2 : 3, 0);
        
        IsGrounded();

        IsSprinting();

        IsCrouching();

        Move();
    }
    
    private void MovementInput()
    {
        if (Input.GetKeyDown(gameOptions.jumpKey) && isGrounded) verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * -gravity);

        input.x = InputHelper.GetKey(gameOptions.leftKey) ? -1 : InputHelper.GetKey(gameOptions.rightKey) ? 1 : 0;
        input.y = InputHelper.GetKey(gameOptions.backwardKey) ? -1 : InputHelper.GetKey(gameOptions.forwardKey) ? 1 : 0;
    }
    
    private void IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0) verticalVelocity = -1f;
    }

    private void IsSprinting()
    {
        if (gameOptions.toggleSprint)
        {
            if (Input.GetKeyDown(gameOptions.sprintKey)) isSprinting = !isSprinting;
            return;
        }
        
        isSprinting = Input.GetKey(gameOptions.sprintKey);
    }

    private void IsCrouching()
    {
        if (gameOptions.toggleDuck)
        {
            if (Input.GetKeyDown(gameOptions.duckKey)) isCrouched = !isCrouched;
            return;
        }
        
        isCrouched = Input.GetKey(gameOptions.duckKey);
    }

    private void Move()
    {
        var maxSpeed = isCrouched ? crouchSpeed : isSprinting ? sprintSpeed : speed;

        if (input.x == 0)
        {
            if (horizontalVelocity.x > -0.05f && horizontalVelocity.x < 0.05f) horizontalVelocity.x = 0.0f;
            else if (input.x < 0) horizontalVelocity.x += 1 * Time.deltaTime * accelerationEasing;
            else horizontalVelocity.x -= 1 * Time.deltaTime * accelerationEasing;
        }
        else if (horizontalVelocity.x < maxSpeed) horizontalVelocity.x += input.x * Time.deltaTime * accelerationEasing;

        if (input.y == 0)
        {
            if (horizontalVelocity.y > -0.05f && horizontalVelocity.y < 0.05f) horizontalVelocity.y = 0.0f;
            else if (input.y < 0) horizontalVelocity.y += 1 * Time.deltaTime * accelerationEasing;
            else horizontalVelocity.y -= 1 * Time.deltaTime * accelerationEasing;
        }
        else if (horizontalVelocity.y < maxSpeed) horizontalVelocity.y += input.y * Time.deltaTime * accelerationEasing;

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= maxSpeed;
        }
        
        verticalVelocity += gravity * Time.deltaTime;
        
        characterController.Move((transform.up * verticalVelocity + transform.right * horizontalVelocity.x + transform.forward * horizontalVelocity.y) * Time.deltaTime);
    }
}
