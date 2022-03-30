using UnityEngine;

public partial class Player
{
    [Header("Movement")]
    private CharacterController characterController;

    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 5f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float movementEasing;
    
    private Vector3 velocity;
    private bool isGrounded;
    
    private bool isCrouched;
    private bool isSprinting;

    private Vector2 input, lastInput;
    private float startInputXTimer, endInputXTimer, startInputYTimer, endInputYTimer;

    private void MovementInput()
    {
        if (Input.GetKeyDown(gameOptions.jumpKey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isCrouched = false;
            isSprinting = false;
        }

        input.x = Input.GetKey(gameOptions.leftKey) ? -1 : Input.GetKey(gameOptions.rightKey) ? 1 : 0;
        input.y = Input.GetKey(gameOptions.backwardKey) ? -1 : Input.GetKey(gameOptions.forwardKey) ? 1 : 0;
        
        if (input.x != 0) lastInput.x = input.x;
        if (input.y != 0) lastInput.y = input.y;
        
        if (input.x != 0) endInputXTimer = Time.time;
        if (input.y != 0) endInputYTimer = Time.time;
        if (input.x == 0) startInputXTimer = Time.time;
        if (input.y == 0) startInputYTimer = Time.time;
    }

    private void MovementUpdate()
    {
        input.x = 0;
        input.y = 0;

        if (takeInput)
        {
            MovementInput();
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            if (gameOptions.toggleSprint)
            {
                if (Input.GetKeyDown(gameOptions.sprintKey))
                {
                    isSprinting = !isSprinting;
                }
            }
            else
            {
                isSprinting = Input.GetKey(gameOptions.sprintKey);
            }

            if (gameOptions.toggleDuck)
            {
                if (Input.GetKeyDown(gameOptions.duckKey))
                {
                    isCrouched = !isCrouched;
                }
            }
            else
            {
                isCrouched = Input.GetKey(gameOptions.duckKey);
            }

            playerCamera.localPosition = new Vector3(0, isCrouched ? 0 : 0.5f, 0);
            
            velocity.y = -2f;
            
            input.x = input.x == 0
                ? Mathf.Lerp(lastInput.x, 0, (Time.time - endInputXTimer) / 0.1f)
                : Mathf.Lerp(0, input.x, (Time.time - startInputXTimer) / 0.1f);
        
            input.y = input.y == 0
                ? Mathf.Lerp(lastInput.y, 0, (Time.time - endInputYTimer) / 0.1f)
                : Mathf.Lerp(0, input.y, (Time.time - startInputYTimer) / 0.1f); 
        }
        
        if (!isGrounded)
        {
            input.x = input.x == 0
                ? Mathf.Lerp(lastInput.x, 0, (Time.time - endInputXTimer) / movementEasing)
                : Mathf.Lerp(0, input.x, (Time.time - startInputXTimer) / movementEasing);
        
            input.y = input.y == 0
                ? Mathf.Lerp(lastInput.y, 0, (Time.time - endInputYTimer) / movementEasing)
                : Mathf.Lerp(0, input.y, (Time.time - startInputYTimer) / movementEasing);   
        }

        var playerTransform = transform;
        Vector3 move = playerTransform.right * input.x + playerTransform.forward * input.y;

        if(move.magnitude > 1) move.Normalize();

        characterController.Move(move * (isCrouched ? crouchSpeed 
                                     : isSprinting ? sprintSpeed 
                                     : speed) * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
