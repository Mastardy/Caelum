using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Animator")]
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private GameObject thirdPersonModel;
    [SerializeField] private Animator firstPersonAnimator;
    
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int sprintCache = Animator.StringToHash("Sprint");
    private static readonly int directionCache = Animator.StringToHash("Direction");
    private static readonly int crouchCache = Animator.StringToHash("Crouch");
    private static readonly int jumpCache = Animator.StringToHash("Jump");
    private static readonly int pitchCache = Animator.StringToHash("Pitch");
    private static readonly int yvelCache = Animator.StringToHash("YVelocity");

    private NetworkVariable<bool> isCrouchedNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> moveMagnitudeNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputXNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputYNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isGroundedNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> xRotationNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> velocityYNet = new(NetworkVariableReadPermission.Everyone);

    private bool tool = false;

    private void AnimatorStart()
    {        
        if (IsLocalPlayer)
        {
            //dar Hide no world model mantendo as sombras
            foreach (var smr in thirdPersonModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            
            //mover world model para trás para não fazer sombras nos braços
            thirdPersonModel.transform.position = new Vector3(0f, 0f, -0.5f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NetworkAnimatorUpdateServerRpc(bool _isCrouched, float _moveMagnitude, float inputX, float inputY,
        bool _isGrounded, float _xRotation, float velocityY)
    {
        if (!IsServer) return;
        
        isCrouchedNet.Value = _isCrouched;
        moveMagnitudeNet.Value = _moveMagnitude;
        inputXNet.Value = inputX;
        inputYNet.Value = inputY;
        isGroundedNet.Value = _isGrounded;
        xRotationNet.Value = _xRotation;
        velocityYNet.Value = velocityY;
    }

    private void AnimatorUpdate()
    {
        //TPS Model animator
        thirdPersonAnimator.SetFloat(speedCache, isCrouchedNet.Value ? moveMagnitudeNet.Value : Map(moveMagnitudeNet.Value, 0, 5));
        thirdPersonAnimator.SetFloat(directionCache, Map(Mathf.Atan2(inputXNet.Value, inputYNet.Value) * Mathf.Rad2Deg, -180, 180));
        thirdPersonAnimator.SetBool(crouchCache, isCrouchedNet.Value);
        thirdPersonAnimator.SetBool(jumpCache, !isGroundedNet.Value);
        thirdPersonAnimator.SetFloat(pitchCache, Map(Mathf.Clamp(-xRotationNet.Value, -50, 50), -90, 90));
        thirdPersonAnimator.SetFloat(yvelCache, Map(velocityYNet.Value, -4, 7));

        //View Model animator
        firstPersonAnimator.SetBool(sprintCache, isSprinting && input.y > 0); //mostrar braços correndo se estiver correndo apenas para frente
        firstPersonAnimator.SetBool(crouchCache, isCrouched);
        if(isGrounded) firstPersonAnimator.SetFloat(speedCache, horizontalVelocity.magnitude);
        firstPersonAnimator.SetBool(jumpCache, !isGrounded);
        firstPersonAnimator.SetFloat(yvelCache, verticalVelocity);
        firstPersonAnimator.SetBool("Tool", tool);

        //debug segurar tool
        if (Input.GetKeyDown(KeyCode.E))
            tool = !tool;

    }
    
    public static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
}
