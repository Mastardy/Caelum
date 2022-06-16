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

    public void DrawBow()
    {
        if (player.currentArrow != null)
            player.currentArrow.SetActive(true);
    }

    public void UndrawBow()
    {
        if (player.currentArrow != null)
            player.currentArrow.SetActive(false);
    }
}
