using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class Player
{
    [Header("HUD")]
    [SerializeField] private Image healthBackgroundImage;
    [SerializeField] private Image healthForegroundImage;
    
    [SerializeField] private Image hungerBackgroundImage;
    [SerializeField] private Image hungerForegroundImage;
    
    [SerializeField] private Image thirstBackgroundImage;
    [SerializeField] private Image thirstForegroundImage;

    [SerializeField] private DamageFilter damageFilter;

    private float lastHealthValue;
    private float lastThirstValue;
    private float lastHungerValue;
    
    private void HUDUpdate()
    {
        UpdateDonut(ref lastHealthValue, currentHealth.Value / (float)maxHealth, healthForegroundImage, healthBackgroundImage);
        UpdateDonut(ref lastThirstValue, currentHunger / (float)maxHunger, hungerForegroundImage, hungerBackgroundImage);
        UpdateDonut(ref lastHungerValue, currentThirst / (float)maxThirst, thirstForegroundImage, thirstBackgroundImage);

        // if (lastHealthValue > currentHealth.Value / 100f)
        // {
        //     healthForegroundImage.fillAmount = currentHealth.Value / 100f;
        //     healthBackgroundImage.fillAmount -= (healthBackgroundImage.fillAmount - healthForegroundImage.fillAmount) * Time.deltaTime * 2;
        //     lastHealthValue = healthBackgroundImage.fillAmount;
        // }
        // else
        // {
        //     healthBackgroundImage.fillAmount = currentHealth.Value / 100f;
        //     healthForegroundImage.fillAmount -= (healthForegroundImage.fillAmount - healthBackgroundImage.fillAmount) * Time.deltaTime * 2;
        //     lastHealthValue = healthForegroundImage.fillAmount;
        // }
    }

    private void UpdateDonut(ref float lastValue, float currentValue, Image foregroundImage, Image backgroundImage)
    {
        if (Mathf.Abs(lastValue - currentValue) < 0.001f) return;

        if (lastValue > currentValue)
        {
            foregroundImage.fillAmount = currentValue;
            backgroundImage.fillAmount -= (backgroundImage.fillAmount - foregroundImage.fillAmount) * Time.deltaTime * 2;
            lastValue = backgroundImage.fillAmount;
        }
        else
        {
            backgroundImage.fillAmount = currentValue;
            foregroundImage.fillAmount -= (foregroundImage.fillAmount - backgroundImage.fillAmount) * Time.deltaTime * 2;
            lastValue = foregroundImage.fillAmount;
        }
    }
}
