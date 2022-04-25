using Unity.Netcode;
using UnityEngine;

public class Furnace : NetworkBehaviour
{
    private bool hasItem;
    private float itemTimer;

    [ServerRpc(RequireOwnership = false)]
    public void CookItemServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;

        if (player.TryGet(out Player ply))
        {
            if (hasItem)
            {
                if (Time.time - itemTimer < 4f) return;

                ply.GiveItemClientRpc(6, 1);
                hasItem = false;
                Debug.Log("Cooked");
                
                return;
            }

            if (ply.GetItemAmount("raw_fish") > 0)
            {
                ply.RemoveItem("raw_fish", 1);
                itemTimer = Time.time;
                hasItem = true;
                Debug.Log("Cooking");
            }
        }
    }
}
