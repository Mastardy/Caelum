using UnityEngine;

public partial class Player
{
    [Header("Movement")]
    private CharacterController characterController;

    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float speed = 6;
    [SerializeField] private float sprintSpeed = 9.5f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float deaccelerationEasing = 60f;
    [SerializeField] private float accelerationEasing = 30f;
    [SerializeField] private float airAccelerationEasing = 3f;
    [SerializeField] private float fallDamageMultiplier = 1.2f;
    [SerializeField] private float minFallDamageVelocity = -40f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float jumpHeight = 1.75f;
    [SerializeField] private float parachuteAcceleration = 50;
    [SerializeField] private float parachuteSpeed = 15f;

    [SerializeField] private float playerCameraEasing = 2;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private Vector3 parachuteCameraLocation = new(0,5,-3);
    [SerializeField] private GameObject parachuteObject;
    [SerializeField] private ParticleSystem speedLines;
    
    private Vector3 playerCameraPosition = new(0, 3, 0);
    
    private Vector2 horizontalVelocity;
    private float verticalVelocity;

    private bool dashing;
    private float dashed;
    private Vector2 dashVelocity;
    
    private bool wasGrounded;
    private int fallDamage;
    
    private bool isGrounded;
    private bool isCrouched;
    private bool isSprinting;
    
    private bool isTethered;
    private bool isTetheredPlus;

    private Vector2 input;
    
    private void MovementUpdate()
    {
        input.x = 0;
        input.y = 0;

        if (takeInput)
        {
            MovementInput();
        }

        CameraUpdate();

        if (IsTethered()) return;

        if (dashing)
        {
            ApplyDashPhysics();
            return;
        }
        
        PreFallDamage();
        
        IsGrounded();

        FallDamage();

        if (takeInput)
        {
            IsSprinting();

            IsCrouching();
        }

        Parachute();
        
        Move();
    }

    private void CameraUpdate()
    {
        if (inParachute)
        {
            if (playerCameraPosition.y > parachuteCameraLocation.y) playerCameraPosition.y = parachuteCameraLocation.y;
            else if (playerCameraPosition.y < parachuteCameraLocation.y) playerCameraPosition.y += Time.deltaTime * playerCameraEasing * 2f;

            if (playerCameraPosition.z < parachuteCameraLocation.z) playerCameraPosition.z = parachuteCameraLocation.z;
            else if (playerCameraPosition.z > parachuteCameraLocation.z) playerCameraPosition.z -= Time.deltaTime * playerCameraEasing * 3f;
        }
        else
        {
            if (isCrouched)
            {
                if (playerCameraPosition.y < 2) playerCameraPosition.y = 2;
                else if (playerCameraPosition.y > 2) playerCameraPosition.y -= Time.deltaTime * playerCameraEasing;
            }
            else
            {
                if (Mathf.Abs(playerCameraPosition.y - 3) < 0.1f) playerCameraPosition.y = 3;
                else if (playerCameraPosition.y > 3) playerCameraPosition.y -= Time.deltaTime * playerCameraEasing;
                else if (playerCameraPosition.y < 3) playerCameraPosition.y += Time.deltaTime * playerCameraEasing;
            }
            
            if (playerCameraPosition.z > 0) playerCameraPosition.z = 0;
            else if (playerCameraPosition.z < 0) playerCameraPosition.z += Time.deltaTime * playerCameraEasing * 2f;
        }

        playerCamera.localPosition = playerCameraPosition;
    }

    private void MovementInput()
    {
        if (Input.GetKeyDown(gameOptions.jumpKey) && isGrounded) verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * -gravity);

        input.x = InputHelper.GetKey(gameOptions.leftKey) ? -1 : InputHelper.GetKey(gameOptions.rightKey) ? 1 : 0;
        input.y = InputHelper.GetKey(gameOptions.backwardKey) ? -1 : InputHelper.GetKey(gameOptions.forwardKey) ? 1 : 0;

        if (input.x == 0 && input.y == 0) isSprinting = gameOptions.toggleSprint ? false : isSprinting;

        if (InputHelper.GetKeyDown(KeyCode.B, 0.5f))
        {
            BeginDash();
        }
    }

    private void BeginDash()
    {
        dashing = true;
        dashVelocity = new Vector2(input.x, input.y);
        dashVelocity = dashVelocity == Vector2.zero ? Vector2.up : dashVelocity;
        dashed = Time.time;
    }

    private void ApplyDashPhysics()
    {
        if (Time.time - dashed > dashTime)
        {
            EndDash();
            return;
        }

        var playerTransform = transform;
        
        if(!inParachute) verticalVelocity += gravity * Time.deltaTime;
        
        characterController.Move((playerTransform.forward * dashVelocity.y + playerTransform.right * dashVelocity.x).normalized * (dashSpeed * Time.deltaTime) + playerTransform.up * verticalVelocity * Time.deltaTime);
    }
    
    private void EndDash()
    {
        dashing = false;
    }
    
    private void PreFallDamage()
    {
        wasGrounded = isGrounded;
        if (inParachute && verticalVelocity > minFallDamageVelocity) fallDamage = 0;
        if (verticalVelocity > minFallDamageVelocity) return;
        fallDamage = (int)(fallDamageMultiplier * verticalVelocity);
    }
    
    private void IsGrounded()
    {
        var playerTransform = transform;
        var sideSize = characterController.radius * playerTransform.localScale.x;
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(sideSize, groundDistance, sideSize), playerTransform.rotation, groundMask);

        if (isGrounded && verticalVelocity < 0) verticalVelocity = -1f;
    }

    private void FallDamage()
    {
        if (wasGrounded || !isGrounded || fallDamage == 0) return;
        TakeDamageServerRpc(-fallDamage);
        fallDamage = 0;
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

    private bool wasInParachute;
    private bool inParachute;
    private float lastParachuteOpen;
    
    private void Parachute()
    {
        wasInParachute = inParachute;

        if (isGrounded)
        {
            inParachute = false;
            if (wasInParachute && !inParachute)
            {
                lastParachuteOpen = Time.time;
                EnableFirstPerson();
                parachuteObject.SetActive(false);
                if (currentWeapon)
                    currentWeapon.SetActive(true);
                speedLines.Stop();
            }
            return;
        }

        if (verticalVelocity > -20f && !wasInParachute)
        {
            inParachute = false;
            if (wasInParachute && !inParachute)
            {
                lastParachuteOpen = Time.time;
                EnableFirstPerson();
                parachuteObject.SetActive(false);
                if (currentWeapon)
                    currentWeapon.SetActive(true);
                speedLines.Stop();
            }
            return;
        }

        if (Time.time - lastParachuteOpen < 1f)
        {
            inParachute = false;
            if (wasInParachute && !inParachute)
            {
                lastParachuteOpen = Time.time;
                EnableFirstPerson();
                parachuteObject.SetActive(false);
                if (currentWeapon)
                    currentWeapon.SetActive(true);
                speedLines.Stop();
            }
            return;
        }

        if (gameOptions.toggleParachute)
        {
            inParachute = Input.GetKeyDown(gameOptions.jumpKey) ? !inParachute : inParachute;
        }
        else
        {
            inParachute = Input.GetKey(gameOptions.jumpKey);
        }

        if (wasInParachute && !inParachute)
        {
            lastParachuteOpen = Time.time;
            EnableFirstPerson();
            parachuteObject.SetActive(false);
            if (currentWeapon)
                currentWeapon.SetActive(true);
            speedLines.Stop();
        }

        if (inParachute)
        {
            verticalVelocity += parachuteAcceleration * Time.deltaTime;
            if (verticalVelocity > -12.6f) verticalVelocity = -12.5f;
            if(!wasInParachute)
            {
                DisableFirstPerson();
                parachuteObject.SetActive(true);
                if (currentWeapon)
                    currentWeapon.SetActive(false);
                speedLines.Play();
            }
        }
    }

    private void Move()
    {
        var maxSpeed = inParachute ? parachuteSpeed : isCrouched ? crouchSpeed : isSprinting ? sprintSpeed : speed;

        if (input.x == 0)
        {
            if (horizontalVelocity.x is > -0.1f and < 0.1f) horizontalVelocity.x = 0.0f;
            else if (horizontalVelocity.x < 0) horizontalVelocity.x += Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
            else horizontalVelocity.x -= Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
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
            else if (horizontalVelocity.y < 0) horizontalVelocity.y += Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
            else horizontalVelocity.y -= 1 * Time.deltaTime * (isGrounded ? accelerationEasing : airAccelerationEasing);
        }
        else
        {
            horizontalVelocity.y += input.y * Time.deltaTime * (isGrounded 
                ? (input.y < 0 && horizontalVelocity.y > 0) || (input.x > 0 && horizontalVelocity.y < 0) ? deaccelerationEasing : accelerationEasing 
                : airAccelerationEasing);
            if(isGrounded) Mathf.Clamp(horizontalVelocity.y, -maxSpeed, maxSpeed);
        }

        if (horizontalVelocity.magnitude > maxSpeed && (isGrounded || inParachute))
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= maxSpeed;
        }

        if(!inParachute) verticalVelocity += gravity * Time.deltaTime;

        var playerTransform = transform;
        
        characterController.Move((playerTransform.up * verticalVelocity + playerTransform.right * horizontalVelocity.x + playerTransform.forward * horizontalVelocity.y) * Time.deltaTime);
    }

    public bool Geyser(float velocity)
    {
        if (!Input.GetKey(KeyCode.Space)) return false;

        verticalVelocity = velocity;
        inParachute = false;
        
        return true;
    }
}
