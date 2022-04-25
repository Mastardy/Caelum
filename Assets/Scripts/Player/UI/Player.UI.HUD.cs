using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class Player
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;

    private void HUDUpdate()
    {
        healthText.color = Color.Lerp(Color.red, Color.green, currentHealth.Value / 100f);
        healthText.SetText(currentHealth.Value.ToString("F0"));
    }
}
