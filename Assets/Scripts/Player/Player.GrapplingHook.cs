using UnityEngine;

public partial class Player
{
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float grappleMaxLength = 30;
    [SerializeField] private float grappleEasing;
    [SerializeField] private float grapplePlusEasing;
    [SerializeField] private float hookSpeed = 1f;
    private Vector3 grapplePoint;
    private float grappleTimer;
    private float grappleLength;
    
    [SerializeField] private float grappleVelocity;
    [SerializeField] private float grapplePlusVelocity = 21f;
    private float currentGrappleVelocity;
    
    private bool IsTethered()
    {
        if (isTethered) ApplyGrapplePhysics();
        else if (isTetheredPlus) ApplyGrapplePlusPhysics();
        else return false;

        return true;
    }

    private void BeginGrapple()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grappleMaxLength, grappleLayer))
        {
            isTethered = true;
            grapplePoint = hit.point;
            grappleTimer = Time.time;
            currentGrappleVelocity = 0;

            PlayToolSwing(ItemTag.Grappling.ToString());
            AnimatorPullGrappling(true);
        }
    }

    private void BeginGrapplePlus()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grappleMaxLength))
        {
            isTetheredPlus = true;
            grapplePoint = hit.point;
            grappleLength = hit.distance;
            grappleTimer = Time.time;
            
            horizontalVelocity = Vector2.zero;
            verticalVelocity = 0;

            PlayToolSwing(ItemTag.Grappling.ToString());
            AnimatorPullGrappling(true);
        }
    }

    private void EndGrapple()
    {
        isTethered = false;

        horizontalVelocity = Vector2.zero;
        verticalVelocity = 0;

        currentWeapon.GetComponent<GrapplingHook>().HookAnchor.transform.localPosition = new Vector3(-0.675f, 0, 0);
        AnimatorPullGrappling(false);
        speedLines.Stop();
    }

    private void EndGrapplePlus()
    {
        var grappleDirection = grapplePoint - playerCamera.transform.position;

        isTetheredPlus = false;

        verticalVelocity += grappleDirection.y * currentGrappleVelocity * Time.deltaTime;

        horizontalVelocity.x += grappleDirection.x * currentGrappleVelocity * Time.deltaTime;
        horizontalVelocity.y += grappleDirection.z * currentGrappleVelocity * Time.deltaTime;

        currentWeapon.GetComponent<GrapplingHook>().HookAnchor.transform.localPosition = new Vector3(-0.675f, 0, 0);
        AnimatorPullGrappling(false);
        speedLines.Stop();
    }
    
    private void ApplyGrapplePhysics()
    {
        currentWeapon.GetComponent<GrapplingHook>().HookAnchor.transform.position = grapplePoint;

        currentGrappleVelocity = currentGrappleVelocity > grappleVelocity ? grappleVelocity : currentGrappleVelocity + grappleVelocity * grappleEasing * Time.deltaTime;
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (currentGrappleVelocity * Time.deltaTime));

        if (Time.time - grappleTimer < 0.25f) return;
        
        Collider[] results = new Collider[10];
        var localScale = transform.localScale.x * 1.25f;
        if (Physics.OverlapBoxNonAlloc(transform.position + (characterController.center * transform.localScale.x), new Vector3(localScale * characterController.radius, localScale * characterController.height / 2, localScale * characterController.radius), results, Quaternion.identity, groundMask) > 1)
        {
            EndGrapple();
        }
    }

    private void ApplyGrapplePlusPhysics()
    {
        currentWeapon.GetComponent<GrapplingHook>().HookAnchor.transform.position = grapplePoint;

        currentGrappleVelocity = currentGrappleVelocity > grappleVelocity ? grappleVelocity : currentGrappleVelocity + grappleVelocity * grapplePlusEasing * Time.deltaTime;
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        
        input.x = InputHelper.GetKey(gameOptions.leftKey) ? -1 : InputHelper.GetKey(gameOptions.rightKey) ? 1 : 0;
        input.y = InputHelper.GetKey(gameOptions.backwardKey) ? -1 : InputHelper.GetKey(gameOptions.forwardKey) ? 1 : 0;
        
        if (input.x == 0)
        {
            if (horizontalVelocity.x is > -0.1f and < 0.1f) horizontalVelocity.x = 0.0f;
            else if (horizontalVelocity.x < 0) horizontalVelocity.x += Time.deltaTime * grapplePlusEasing;
            else horizontalVelocity.x -= Time.deltaTime * grapplePlusEasing;
        }
        else
        {
            horizontalVelocity.x += input.x * Time.deltaTime * grapplePlusEasing;
        }

        if (input.y == 0)
        {
            if (horizontalVelocity.y is > -0.1f and < 0.1f) horizontalVelocity.y = 0.0f;
            else if (horizontalVelocity.y < 0) horizontalVelocity.y += Time.deltaTime * grapplePlusEasing;
            else horizontalVelocity.y -= Time.deltaTime * grapplePlusEasing;
        }
        else
        {
            horizontalVelocity.y += input.y * Time.deltaTime * grapplePlusEasing;
        }

        if (horizontalVelocity.magnitude > grapplePlusVelocity - grappleVelocity)
        {
            horizontalVelocity.Normalize();
            horizontalVelocity *= grapplePlusVelocity - grappleVelocity;
        }
        
        characterController.Move(Vector3.Normalize(grappleDirection) * (currentGrappleVelocity * Time.deltaTime) + (transform.forward * horizontalVelocity.y + transform.right * horizontalVelocity.x) * Time.deltaTime);

        if (Time.time - grappleTimer < 0.25f) return;
        
        Collider[] results = new Collider[10];
        var localScale = transform.localScale.x * 1.25f;
        if (Physics.OverlapBoxNonAlloc(transform.position + (characterController.center * transform.localScale.x), new Vector3(localScale * characterController.radius, localScale * characterController.height / 2, localScale * characterController.radius), results, Quaternion.identity, groundMask) > 1)
        {
            EndGrapplePlus();
        }
    }
}
