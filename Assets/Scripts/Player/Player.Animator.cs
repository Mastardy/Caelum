using Unity.Netcode;
using UnityEngine;
using System.Collections;

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
    private static readonly int toolCache = Animator.StringToHash("Tool");
    private static readonly int aimCache = Animator.StringToHash("Aim");
    private static readonly int spearCache = Animator.StringToHash("Spear");
    private static readonly int swordCache = Animator.StringToHash("Sword");
    private static readonly int comboCache = Animator.StringToHash("SwordCombo");

    private NetworkVariable<bool> isCrouchedNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> moveMagnitudeNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputXNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> inputYNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> isGroundedNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> xRotationNet = new(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<float> velocityYNet = new(NetworkVariableReadPermission.Everyone);

    //debug
    private bool animTool = false;
    public bool animSpear = false;
    private bool animSword = false;
    private bool animBow = false;
    private int animCombo = 1;
    private bool animAim = false;
    private float layerWeight = 0;
    private bool animCoroutine = false;

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

        //View model tools and weapons
        firstPersonAnimator.SetBool(toolCache, animTool);
        firstPersonAnimator.SetBool(aimCache, animAim);
        firstPersonAnimator.SetBool(spearCache, animSpear);
        firstPersonAnimator.SetBool(swordCache, animSword);
        firstPersonAnimator.SetInteger(comboCache, animCombo);

        //layer weight corountine
        if (animCoroutine)
            firstPersonAnimator.SetLayerWeight(1, layerWeight);

        //debug

        //tool
        //if (Input.GetKeyDown(KeyCode.E) && !spear && !sword)
        //{
        //    tool = !tool;
        //    toolObject.SetActive(tool);
        //}

        //spear
        //if (Input.GetKeyDown(KeyCode.Q) && !sword && !tool)
        //{
        //    spear = !spear;
        //    spear = EquipWeapon(spear);
        //    spearObject.SetActive(spear);
        //}


        //animAim = (animSpear && InputHelper.GetKey(gameOptions.secondaryAttackKey));

        //if (animSpear && !animAim && InputHelper.GetKeyDown(gameOptions.primaryAttackKey))
        //  firstPersonAnimator.SetTrigger("UseSpear");


        //if (animAim && InputHelper.GetKeyDown(gameOptions.primaryAttackKey))
        //    firstPersonAnimator.SetTrigger("ThrowSpear");

        //sword
        //if (Input.GetKeyDown(KeyCode.F) && !animSpear && !animTool)
        //{
        //    animSword = !animSword;
        //    animSword = AnimEquip(animSword);
        //}

        //if (animSword && InputHelper.GetKeyDown(gameOptions.primaryAttackKey))
        //{
        //    firstPersonAnimator.SetTrigger("UseSword");
        //    switch (animCombo)
        //    {
        //        case 1:
        //            animCombo = 2;
        //            break;
        //        case 2:
        //            animCombo = 1;
        //            break;
        //    }
        //}
    }

    private void AnimatorEquipTool(bool e)
    {
        animTool = e;
    }

    private void AnimatorEquipSpear(bool e)
    {
        animSpear = e;
        AnimEquip(e);
    }

    private void AnimatorEquipSword(bool e)
    {
        animSword = e;
        AnimEquip(e);
    }

    private void AnimatorAim(bool e)
    {
        animAim = e;
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
                animCombo = 2;
                break;
            case 2:
                animCombo = 1;
                break;
        }
    }

    #region support_functions
    private static float Map(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }

    private bool AnimEquip(bool e)
    {
        //Essa função trata da corotina que faz o blend da layer do animator baseado se equipou ou desequipou
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
