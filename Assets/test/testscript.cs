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
        animator.SetFloat("Direction", map(direction, -180, 180));
        animator.SetBool("Crouch", crouch);
    }

    // Maps a value from some arbitrary range to the 0 to 1 range
    public static float map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
}
