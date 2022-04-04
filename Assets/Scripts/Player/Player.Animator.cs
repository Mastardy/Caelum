using UnityEngine;

public partial class Player
{
    [Header("Animator")]
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private Animator firstPersonAnimator;
    
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int directionCache = Animator.StringToHash("Direction");
    private static readonly int crouchCache = Animator.StringToHash("Crouch");
    private static readonly int jumpCache = Animator.StringToHash("Jump");
    private static readonly int pitchCache = Animator.StringToHash("Pitch");
    private static readonly int yvelCache = Animator.StringToHash("YVelocity");

    private void AnimatorUpdate()
    {
        thirdPersonAnimator.SetFloat(speedCache, isCrouched ? move.magnitude : Map(move.magnitude, 0, 5));
        thirdPersonAnimator.SetFloat(directionCache, Map(Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg, -180, 180));
        thirdPersonAnimator.SetBool(crouchCache, isCrouched);
        thirdPersonAnimator.SetBool(jumpCache, !isGrounded);
        thirdPersonAnimator.SetFloat(pitchCache, Map(Mathf.Clamp(-xRotation, -50, 50), -90, 90));
        thirdPersonAnimator.SetFloat(yvelCache, Map(velocity.y, -4, 7)); //velocidade vertical
    }
    
    // Maps a value from some arbitrary range to the 0 to 1 range
    public static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
}
