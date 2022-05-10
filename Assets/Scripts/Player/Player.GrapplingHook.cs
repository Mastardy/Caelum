using UnityEngine;

public partial class Player
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float grappleMaxLength = 30;
    [SerializeField] private float grappleEasing;
    private Vector3 grapplePoint;
    private float grappleTimer;
    private float grappleLength;
    
    [SerializeField] private float grappleVelocity;
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
            lineRenderer.enabled = true;
            isTethered = true;
            grapplePoint = hit.point;
            grappleTimer = Time.time;
            currentGrappleVelocity = 0;
        }
    }

    private void BeginGrapplePlus()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grappleMaxLength))
        {
            lineRenderer.enabled = true;
            isTetheredPlus = true;
            grapplePoint = hit.point;
            grappleLength = hit.distance;
            grappleTimer = Time.time;
        }
    }

    private void EndGrapple()
    {
        lineRenderer.enabled = false;
        isTethered = false;

        horizontalVelocity = Vector2.zero;
        verticalVelocity = 0;
    }

    private void EndGrapplePlus()
    {
        lineRenderer.enabled = false;
        isTetheredPlus = false;
    }
    
    private void ApplyGrapplePhysics()
    {
        lineRenderer.SetPosition(0, playerCamera.transform.position + new Vector3(0, -0.2f, 0));
        lineRenderer.SetPosition(1, grapplePoint);
        
        currentGrappleVelocity = currentGrappleVelocity > grappleVelocity ? grappleVelocity : currentGrappleVelocity + grappleVelocity * grappleEasing * Time.deltaTime;
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (currentGrappleVelocity * Time.deltaTime));

        if (Time.time - grappleTimer < 0.25f) return;
        
        Collider[] results = new Collider[10];
        var localScale = transform.localScale.x * 1.25f;
        if (Physics.OverlapBoxNonAlloc(transform.position + (characterController.center * transform.localScale.x), new Vector3(localScale * characterController.radius, localScale * characterController.height / 2, localScale * characterController.radius), results) > 1)
        {
            EndGrapple();
        }
    }

    private void ApplyGrapplePlusPhysics()
    {
        lineRenderer.SetPosition(0, playerCamera.transform.position + new Vector3(0, -0.2f, 0));
        lineRenderer.SetPosition(1, grapplePoint);
        
        currentGrappleVelocity = currentGrappleVelocity > grappleVelocity ? grappleVelocity : currentGrappleVelocity + grappleVelocity * grappleEasing * Time.deltaTime;
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (currentGrappleVelocity * Time.deltaTime));

        if (Time.time - grappleTimer < 0.25f) return;
        
        Collider[] results = new Collider[10];
        var localScale = transform.localScale.x * 1.25f;
        if (Physics.OverlapBoxNonAlloc(transform.position + (characterController.center * transform.localScale.x), new Vector3(localScale * characterController.radius, localScale * characterController.height / 2, localScale * characterController.radius), results) > 1)
        {
            EndGrapplePlus();
        }
    }
}
