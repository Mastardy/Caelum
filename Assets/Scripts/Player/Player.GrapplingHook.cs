using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [SerializeField] private float grappleVelocity;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float grappleMaxLength = 30;
    private Vector3 grapplePoint;
    private float grappleLength;
    
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
            grappleLength = hit.distance;
        }
    }

    private void BeginGrapplePlus()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grappleMaxLength))
        {
            isTetheredPlus = true;
            grapplePoint = hit.point;
            grappleLength = hit.distance;
        }
    }

    private void EndGrapple()
    {
        isTethered = false;
    }

    private void EndGrapplePlus()
    {
        isTetheredPlus = false;
    }
    
    private void ApplyGrapplePhysics()
    {
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (grappleVelocity * Time.deltaTime));
        if (grappleDirection.magnitude < 2) EndGrapple();
    }

    private void ApplyGrapplePlusPhysics()
    {
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (grappleVelocity * Time.deltaTime));
        if (grappleDirection.magnitude < 2) EndGrapple();
    }
}
