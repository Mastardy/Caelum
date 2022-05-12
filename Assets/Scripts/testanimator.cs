using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testanimator : MonoBehaviour
{
    private Animator animator;
    [Range(0, 1)] public float speed = 0.5f;
    [Range(0, 1)] public float direction = 0.5f;
    [Range(0, 1)]public float pitch = 0.5f;
    public bool crouch;
    public bool jump;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", direction);
        animator.SetFloat("Pitch", pitch);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("Jump", jump);
    }
}
