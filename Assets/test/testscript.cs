using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    public Animator animator;
    [Range(0, 1)]public float speed = 0;
    [Range(-180,180)] public float direction = 0;
    public bool crouch = false;
    public bool jump = false;
    [Range(-90, 90)] public float pitch = 0;

    public Vector3 original_position = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", map(direction, -180, 180));
        animator.SetBool("Crouch", crouch);
        animator.SetBool("Jump", jump);
        animator.SetFloat("Pitch", map(pitch, -90, 90));

        //transform.position = new Vector3(Mathf.Lerp(-5, -8, Mathf.Sin(Time.time*2)), original_position.y, original_position.z);
    }

    // Maps a value from some arbitrary range to the 0 to 1 range
    public static float map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
}
