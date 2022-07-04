using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct ChestItems
{
    public InventoryItem item;
    public int amount;
}
public class Chest : NetworkBehaviour
{
    public ChestItems[] items;
    private Animator animator;
    public NetworkVariable<bool> opened = new(readPerm: NetworkVariableReadPermission.Everyone);
    public string displayName = "Chest";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ServerRpc]
    public void GiveItemsServerRpc(NetworkBehaviourReference player)
    {
        if (opened.Value) return;
        if (items.Length == 0) return;

        opened.Value = true;

        animator.SetBool("Open", true);
        for (int i = 0; i < items.Length; i++)
        {
            if (player.TryGet(out Player ply)) ply.GiveItemServerRpc(player, items[i].item.itemName, items[i].amount);
        }
    }
}
