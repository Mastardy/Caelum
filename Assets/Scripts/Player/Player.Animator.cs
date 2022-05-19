using Unity.Netcode;
using UnityEngine;
using System.Collections;

public partial class Player
{
    [Header("Animator")]
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private GameObject thirdPersonModel;
    [SerializeField] private Animator firstPersonAnimator;
    private Animator bowAnimator;
    
    private static readonly int speedCache = Animator.StringToHash("Speed");
    private static readonly int sprintCache = Animator.StringToHash("Sprint");
    private static readonly int directionCache = Animator.StringToHash("Direction");
    private static readonly int crouchCache = Animator.StringToHash("Crouch");
    private static readonly int jumpCache = Animator.StringToHash("Jump");
    private static readonly int pitchCache = Animator.StringToHash("Pitch");
    private static readonly int yvelCache = Animator.StringToHash("YVelocity");
    private static readonly int toolCache = Animator.StringToHash("Tool");
    private static readonly int aimCache = Animator.StringToHash("Aim");
    private static readonly int spearCache = Animator.StringToHash("Spear");
    private static readonly int swordCache = Animator.StringToHash("Sword");
    private static readonly int comboCache = Animator.StringToHash("SwordCombo");
    private static readonly int bowCache = Animator.StringToHash("Bow");
    private static readonly int drawbowCache = Animator.StringToHash("Draw");
    private static readonly int shootCache = Animator.StringToHash("Shoot");
    private static readonly int grapplingCache = Animator.StringToHash("Grappling");
    private static readonly int grapplingPullCache = Animator.StringToHash("GrapplingPull");
    private static readonly int parachuteCache = Animator.StringToHash("Parachute");

    //tps model variables
    private static readonly int holdToolCache = Animator.StringToHash("HoldTool");
    private static readonly int useToolCache = Animator.StringToHash("UseTool");

    private NetworkVariable<bool> isCrouchedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> moveMagnitudeNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputXNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputYNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isGroundedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> xRotationNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> velocityYNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> holdToolNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> useToolNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> parachuteNet = new(readPerm: NetworkVariableReadPermission.Everyone);

    private float layerWeight;
    private bool canAimAnim;
    private bool animAim;
    private int animCombo = 1;

    private SkinnedMeshRenderer[] playerSkinnedMeshRenderers;
    private SkinnedMeshRenderer[] playerViewModelSkinnedMeshRenderers;
    
    public bool CanAim
    {
        get { return canAimAnim; }
        set { canAimAnim = value; }
    }

    private void EnableFirstPerson()
    {
        foreach (var smr in playerViewModelSkinnedMeshRenderers)
        {
            smr.enabled = true;
        }
        
        foreach (var smr in playerSkinnedMeshRenderers)
        {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        
        thirdPersonModel.transform.localPosition = new Vector3(0f, 0f, -0.5f);
    }
    
    private void DisableFirstPerson()
    {
        foreach (var smr in playerViewModelSkinnedMeshRenderers)
        {
            smr.enabled = false;
        }
        
        foreach (var smr in playerSkinnedMeshRenderers)
        {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        
        thirdPersonModel.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
    
    private void AnimatorStart()
    {
        playerSkinnedMeshRenderers = thirdPersonModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        playerViewModelSkinnedMeshRenderers = firstPersonAnimator.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        EnableFirstPerson();
    }

    [ServerRpc(RequireOwnership = false)]
    private void NetworkAnimatorUpdateServerRpc(bool crouched, float moveMag, float xInput, float yInput,
        bool grounded, float rotationX, float velocityY, bool holdTool, bool useTool, bool parachute)
    {
        if (!IsServer) return;
        
        isCrouchedNet.Value = crouched;
        moveMagnitudeNet.Value = moveMag;
        inputXNet.Value = xInput;
        inputYNet.Value = yInput;
        isGroundedNet.Value = grounded;
        xRotationNet.Value = rotationX;
        velocityYNet.Value = velocityY;
        holdToolNet.Value = holdTool;
        useToolNet.Value = useTool;
        parachuteNet.Value = parachute;
    }

    private void AnimatorUpdate()
    {
        thirdPersonAnimator.SetFloat(speedCache, moveMagnitudeNet.Value);
        thirdPersonAnimator.SetFloat(directionCache, Map(Mathf.Atan2(inputXNet.Value, inputYNet.Value) * Mathf.Rad2Deg, -180, 180));
        thirdPersonAnimator.SetBool(crouchCache, isCrouchedNet.Value);
        thirdPersonAnimator.SetBool(jumpCache, !isGroundedNet.Value);
        thirdPersonAnimator.SetFloat(pitchCache, Map(Mathf.Clamp(-xRotationNet.Value, -50, 50), -90, 90));
        thirdPersonAnimator.SetFloat(yvelCache, Map(velocityYNet.Value, -4, 7));
        thirdPersonAnimator.SetBool(parachuteCache, parachuteNet.Value);

        //View Model animator variables
        firstPersonAnimator.SetBool(sprintCache, isSprinting && input.y > 0);
        firstPersonAnimator.SetBool(crouchCache, isCrouched);
        if(isGrounded) firstPersonAnimator.SetFloat(speedCache, horizontalVelocity.magnitude);
        firstPersonAnimator.SetBool(jumpCache, !isGrounded);
        firstPersonAnimator.SetFloat(yvelCache, verticalVelocity);

        //tools and weapons variables
        thirdPersonAnimator.SetBool(holdToolCache, holdToolNet.Value);
        thirdPersonAnimator.SetBool(useToolCache, useToolNet.Value);
        firstPersonAnimator.SetBool(aimCache, animAim);
        thirdPersonAnimator.SetBool(aimCache, animAim);
    }

    private void AnimatorEquipTool(bool equip)
    {
        firstPersonAnimator.SetBool(toolCache, equip);
    }

    private void AnimatorUseAxe()
    {
        firstPersonAnimator.SetTrigger("UseAxe");
    }

    private void AnimatorUsePickaxe()
    {
        firstPersonAnimator.SetTrigger("UsePickaxe");
    }

    private void AnimatorEquipSpear(bool equip)
    {
        firstPersonAnimator.SetBool(spearCache, equip);
        SetRightArmWeight(1);
    }

    private void AnimatorEquipSword(bool equip)
    {
        firstPersonAnimator.SetBool(swordCache, equip);
        SetRightArmWeight(1);
    }

    private void AnimatorEquipBow(bool equip)
    {
        firstPersonAnimator.SetBool(bowCache, equip);
        canAimAnim = equip;
    }

    private void AnimatorEquipGrappling(bool equip)
    {
        firstPersonAnimator.SetBool(grapplingCache, equip);
        SetRightArmWeight(1);
    }

    private void AnimatorAim(bool aim)
    {
        animAim = aim;
        if(bowAnimator) bowAnimator.SetBool(drawbowCache, animAim);
        if (!animAim)
        {
            firstPersonAnimator.ResetTrigger(shootCache);
            thirdPersonAnimator.SetBool(shootCache, false);
            if (bowAnimator) bowAnimator.ResetTrigger(shootCache);
        }
    }

    private void AnimatorUseSpear()
    {
        firstPersonAnimator.SetTrigger("UseSpear");
    }

    private void AnimatorThrowSpear()
    {
        firstPersonAnimator.SetTrigger("ThrowSpear");
    }

    private void AnimatorUseSword()
    {
        firstPersonAnimator.SetTrigger("UseSword");

        switch (animCombo)
        {
            case 1:
                firstPersonAnimator.SetInteger(comboCache, animCombo = 2);
                break;
            case 2:
                firstPersonAnimator.SetInteger(comboCache, animCombo = 1);
                break;
        }
    }

    private void AnimatorShootBow()
    {
        firstPersonAnimator.SetTrigger(shootCache);
        thirdPersonAnimator.SetBool(shootCache, true);
        bowAnimator.SetTrigger(shootCache);
    }

    private void AnimatorShootGrappling()
    {
        firstPersonAnimator.SetTrigger(shootCache);
    }

    private void AnimatorPullGrappling(bool pull)
    {
        firstPersonAnimator.SetBool(grapplingPullCache, pull);
    }

    private void AnimatorCollect()
    {
        SetLeftArmWeight(1);
        firstPersonAnimator.SetTrigger("Collect");
    }

    private void AnimatorEat() //not done
    {
        SetLeftArmWeight(1);
        firstPersonAnimator.SetTrigger("Eat");
    }

    #region support_functions
    public void SetLeftArmWeight(float w)
    {
        firstPersonAnimator.SetLayerWeight(2, w);
    }

    public void SetRightArmWeight(float w)
    {
        firstPersonAnimator.SetLayerWeight(1, w);
    }

    public void SetTPSArmsWeight(float w)
    {
        thirdPersonAnimator.SetLayerWeight(2, w);
    }

    private static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }
    #endregion
}
