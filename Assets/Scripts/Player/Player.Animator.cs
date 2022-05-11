using Unity.Netcode;
using UnityEngine;
using System.Collections;

public partial class Player
{
    [Header("Animator")]
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private GameObject thirdPersonModel;
    [SerializeField] private Animator firstPersonAnimator;
    [SerializeField] private Animator bowAnimator;
    
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

    private NetworkVariable<bool> isCrouchedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> moveMagnitudeNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputXNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputYNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isGroundedNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> xRotationNet = new(readPerm: NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> velocityYNet = new(readPerm: NetworkVariableReadPermission.Everyone);

    //debug
    private float layerWeight;
    private bool canAimAnim;
    private bool animAim;
    private bool animCoroutine;
    private int animCombo = 1;

    private SkinnedMeshRenderer[] playerSkinnedMeshRenderers;
    
    public bool CanAim
    {
        get { return canAimAnim; }
        set { canAimAnim = value; }
    }

    private void EnableFirstPerson()
    {
        firstPersonAnimator.gameObject.SetActive(true);
        
        foreach (var smr in playerSkinnedMeshRenderers)
        {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
        
        thirdPersonModel.transform.localPosition = new Vector3(0f, 0f, -0.5f);
    }
    
    private void DisableFirstPerson()
    {
        firstPersonAnimator.gameObject.SetActive(false);
        
        foreach (var smr in playerSkinnedMeshRenderers)
        {
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        
        thirdPersonModel.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
    
    private void AnimatorStart()
    {
        playerSkinnedMeshRenderers = thirdPersonModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        EnableFirstPerson();
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
        thirdPersonAnimator.SetFloat(speedCache, isCrouchedNet.Value ? moveMagnitudeNet.Value : Map(moveMagnitudeNet.Value, 0, 5));
        thirdPersonAnimator.SetFloat(directionCache, Map(Mathf.Atan2(inputXNet.Value, inputYNet.Value) * Mathf.Rad2Deg, -180, 180));
        thirdPersonAnimator.SetBool(crouchCache, isCrouchedNet.Value);
        thirdPersonAnimator.SetBool(jumpCache, !isGroundedNet.Value);
        thirdPersonAnimator.SetFloat(pitchCache, Map(Mathf.Clamp(-xRotationNet.Value, -50, 50), -90, 90));
        thirdPersonAnimator.SetFloat(yvelCache, Map(velocityYNet.Value, -4, 7));

        //View Model animator variables
        firstPersonAnimator.SetBool(sprintCache, isSprinting && input.y > 0);
        firstPersonAnimator.SetBool(crouchCache, isCrouched);
        if(isGrounded) firstPersonAnimator.SetFloat(speedCache, horizontalVelocity.magnitude);
        firstPersonAnimator.SetBool(jumpCache, !isGrounded);
        firstPersonAnimator.SetFloat(yvelCache, verticalVelocity);
        
        //View model tools and weapons variables
        firstPersonAnimator.SetBool(aimCache, animAim);

        //layer weight corountine
        if (animCoroutine)
            firstPersonAnimator.SetLayerWeight(1, layerWeight);
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
        AnimEquip(equip);
    }

    private void AnimatorEquipSword(bool equip)
    {
        firstPersonAnimator.SetBool(swordCache, equip);
        AnimEquip(equip);
    }

    private void AnimatorEquipBow(bool equip)
    {
        firstPersonAnimator.SetBool(bowCache, equip);
        canAimAnim = equip;
    }

    private void AnimatorEquipGrappling(bool equip)
    {
        firstPersonAnimator.SetBool(grapplingCache, equip);
        AnimEquip(equip);
    }

    private void AnimatorAim(bool aim)
    {
        animAim = aim;
        bowAnimator.SetBool(drawbowCache, animAim);
        if (!animAim)
        {
            firstPersonAnimator.ResetTrigger(shootCache);
            bowAnimator.ResetTrigger(shootCache);
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

    private static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }

    private bool AnimEquip(bool e)
    {
        //Essa fun��o trata da corotina que faz o blend da layer do animator baseado se equipou ou desequipou
        if (!e)
        {
            StopAllCoroutines();
            StartCoroutine(BlendWeight(1));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(BlendWeight(-1));
        }

        return e;
    }

    private IEnumerator BlendWeight(int direction)
    {
        //direction -1: vai de 1 a 0
        //direction 1: vai de 0 a 1
        //usado para o blend da layer weight do animator, utilizando a variavel layerWeight
        
        float amount = 0.01f;
        animCoroutine = true;

        switch (direction)
        {
            case 1: layerWeight = 1; break;
            case -1: layerWeight = 0; break;
        }

        while (layerWeight is >= 0 and <= 1)
        {
            switch (direction)
            {
                case 1: layerWeight -= amount; break;
                case -1: layerWeight += amount; break;
            }

            yield return null;
        }

        switch (direction)
        {
            case 1: layerWeight = 0; break;
            case -1: layerWeight = 1; break;
        }
        
        animCoroutine = false;
    }
    #endregion
}
