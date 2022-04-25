using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class Player
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;

    private void HUDUpdate()
    {
        healthText.SetText(currentHealth.Value.ToString("F0"));
    }
}
