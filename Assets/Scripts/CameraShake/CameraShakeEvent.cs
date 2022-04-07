using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeEvent : MonoBehaviour
{
    //Este script deve estar contido no objeto que contem o Animator (ex no HumanoidArms dentro do prefab do player)
    //Deve-se passar a intensidade em inteiro, no script CameraShake será convertido para float pela conta: amount/200.0f para ter maior controle disso
    //Ex: o camera shake para bater em arvore com machado está com valor 5

    public CameraShake shake;

    public void PlayShake(int amount)
    {
        shake.PlayShake(amount);
    }
}
