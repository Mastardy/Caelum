using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private Canvas playerCanvas;

    private void Start()
    {
        if (!IsLocalPlayer)
        {
            playerCanvas.gameObject.SetActive(false);
        }
    }
}
