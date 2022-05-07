using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [SerializeField] private float grappleVelocity;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float grappleMaxLength = 30;
    private Vector3 grapplePoint;
    private float grappleTimer;
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
            grappleTimer = Time.time;
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
        }
    }

    private void EndGrapple()
    {
        isTethered = false;

        horizontalVelocity = Vector2.zero;
        verticalVelocity = 0;
    }

    private void EndGrapplePlus()
    {
        isTetheredPlus = false;
    }
    
    private void ApplyGrapplePhysics()
    {
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (grappleVelocity * Time.deltaTime));

        if (Time.time - grappleTimer < 0.1f) return;
        
        Collider[] results = new Collider[10];
        var localScale = transform.localScale.x * 1.25f;
        if (Physics.OverlapBoxNonAlloc(transform.position + (characterController.center * transform.localScale.x), new Vector3(localScale * characterController.radius, localScale * characterController.height / 2, localScale * characterController.radius), results) > 1)
        {
            EndGrapple();
        }
    }

    private void ApplyGrapplePlusPhysics()
    {
        var grappleDirection = grapplePoint - playerCamera.transform.position;
        characterController.Move(Vector3.Normalize(grappleDirection) * (grappleVelocity * Time.deltaTime));
    }
}
