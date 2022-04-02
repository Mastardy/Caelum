using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    public Animator animator;
    [Range(0, 1)]public float speed = 0;
    [Range(-180,180)] public float direction = 0;
    public bool crouch = false;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", direction);
        animator.SetBool("Crouch", crouch);
    }
}
