using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class Player
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBackgroundImage;
    [SerializeField] private Image healthForegroundImage;

    private float lastValue;
    
    private void HUDUpdate()
    {
        if (Math.Abs(lastValue - currentHealth.Value / 100f) < 0.001) return;
        
        if (lastValue > currentHealth.Value / 100f)
        {
            healthForegroundImage.fillAmount = currentHealth.Value / 100f;
            healthBackgroundImage.fillAmount -= (healthBackgroundImage.fillAmount - healthForegroundImage.fillAmount) * Time.deltaTime * 2;
            lastValue = healthBackgroundImage.fillAmount;
        }
        else
        {
            healthBackgroundImage.fillAmount = currentHealth.Value / 100f;
            healthForegroundImage.fillAmount -= (healthForegroundImage.fillAmount - healthBackgroundImage.fillAmount) * Time.deltaTime * 2;
            lastValue = healthForegroundImage.fillAmount;
        }
    }
}
