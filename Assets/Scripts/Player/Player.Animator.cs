using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Animator")]
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private GameObject thirdPersonModel;
    [SerializeField] private Animator firstPersonAnimator;
    
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int directionCache = Animator.StringToHash("Direction");
    private static readonly int crouchCache = Animator.StringToHash("Crouch");
    private static readonly int jumpCache = Animator.StringToHash("Jump");
    private static readonly int pitchCache = Animator.StringToHash("Pitch");
    private static readonly int yvelCache = Animator.StringToHash("YVelocity");

    private NetworkVariable<bool> isCrouchedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> moveMagnitudeNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputXNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputYNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isGroundedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> xRotationNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> velocityYNet = new(readPerm: NetworkVariableReadPermission.Everyone);

    private void AnimatorStart()
    {
        foreach (var smr in thirdPersonModel.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NetworkAnimatorUpdateServerRpc(bool crouched, float moveMag, float xInput, float yInput,
        bool grounded, float rotationX, float velocityY)
    {
        if (!IsServer) return;
        
        isCrouchedNet.Value = crouched;
        moveMagnitudeNet.Value = moveMag;
        inputXNet.Value = xInput;
        inputYNet.Value = yInput;
        isGroundedNet.Value = grounded;
        xRotationNet.Value = rotationX;
        velocityYNet.Value = velocityY;
    }
    
    private void AnimatorUpdate()
    {
        thirdPersonAnimator.SetFloat(speedCache, isCrouchedNet.Value ? moveMagnitudeNet.Value : Map(moveMagnitudeNet.Value, 0, 8));
        thirdPersonAnimator.SetFloat(directionCache, Map(Mathf.Atan2(inputXNet.Value, inputYNet.Value) * Mathf.Rad2Deg, -180, 180));
        thirdPersonAnimator.SetBool(crouchCache, isCrouchedNet.Value);
        thirdPersonAnimator.SetBool(jumpCache, !isGroundedNet.Value);
        thirdPersonAnimator.SetFloat(pitchCache, Map(Mathf.Clamp(-xRotationNet.Value, -50, 50), -90, 90));
        thirdPersonAnimator.SetFloat(yvelCache, Map(velocityYNet.Value, -4, 7)); //velocidade vertical
    }
    
    // Maps a value from some arbitrary range to the 0 to 1 range
    public static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
}
