using UnityEngine;

public partial class Player
{
    [Header("Movement")]
    private CharacterController characterController;

    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float speed = 6;
    [SerializeField] private float sprintSpeed = 9.5f;
    [SerializeField] private float deaccelerationEasing = 60f;
    [SerializeField] private float accelerationEasing = 30f;
    [SerializeField] private float airAccelerationEasing = 3f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float jumpHeight = 1.75f;

    [SerializeField] private float playerCameraHeight = 3;
    [SerializeField] private float playerCameraEasing = 2;

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

        if (isCrouched)
        {
            if (playerCameraHeight < 2) playerCameraHeight = 2;
            else if (playerCameraHeight > 2) playerCameraHeight -= Time.deltaTime * playerCameraEasing;
        }
        else
        {
            if (playerCameraHeight > 3) playerCameraHeight = 3;
            else if (playerCameraHeight < 3) playerCameraHeight += Time.deltaTime * playerCameraEasing;
        }
        
        playerCamera.localPosition = new Vector3(0, playerCameraHeight, 0);
        
        IsGrounded();

        IsSprinting();

        IsCrouching();

        Parachute();
        
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

    private void Parachute()
    {
        if (isGrounded) return;

        if (Input.GetKey(KeyCode.O)) verticalVelocity = -2f;
    }
    
    private void Move()
    {
        var maxSpeed = isCrouched ? crouchSpeed : isSprinting ? sprintSpeed : speed;

        if (input.x == 0)
        {
            if (horizontalVelocity.x is > -0.1f and < 0.1f) horizontalVelocity.x = 0.0f;
            else if (horizontalVelocity.x < 0) horizontalVelocity.x += 1 * Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
            else horizontalVelocity.x -= 1 * Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
        }
        else
        {
            horizontalVelocity.x += input.x * Time.deltaTime * (isGrounded
                ? (input.x < 0 && horizontalVelocity.x > 0) || (input.x > 0 && horizontalVelocity.x < 0) ? deaccelerationEasing : accelerationEasing
                : airAccelerationEasing);
            if(isGrounded) Mathf.Clamp(horizontalVelocity.x, -maxSpeed, maxSpeed);
        }

        if (input.y == 0)
        {
            if (horizontalVelocity.y is > -0.1f and < 0.1f) horizontalVelocity.y = 0.0f;
            else if (horizontalVelocity.y < 0) horizontalVelocity.y += 1 * Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
            else horizontalVelocity.y -= 1 * Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
        }
        else
        {
            horizontalVelocity.y += input.y * Time.deltaTime * (isGrounded 
                ? (input.y < 0 && horizontalVelocity.y > 0) || (input.x > 0 && horizontalVelocity.y < 0) ? deaccelerationEasing : accelerationEasing 
                : airAccelerationEasing);
            if(isGrounded) Mathf.Clamp(horizontalVelocity.y, -maxSpeed, maxSpeed);
        }

        if (horizontalVelocity.magnitude > maxSpeed && isGrounded)
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= maxSpeed;
        }
        
        verticalVelocity += gravity * Time.deltaTime;

        var playerTransform = transform;
        characterController.Move((playerTransform.up * verticalVelocity + playerTransform.right * horizontalVelocity.x + playerTransform.forward * horizontalVelocity.y) * Time.deltaTime);
    }
}
