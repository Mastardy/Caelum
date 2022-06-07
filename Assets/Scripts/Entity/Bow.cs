using UnityEngine;

public class Bow : MonoBehaviour
{
    public Player player;
    public string currentArrow;
    
    public Animator bowAnimator;
    public Transform arrowAnchor;

    public void ThrowArrow()
    {
        if (currentArrow == string.Empty) return;
        player.ThrowArrowServerRpc(player, currentArrow);
    }
}
