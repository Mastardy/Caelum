using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Options")] 
    [SerializeField] private TextMeshProUGUI fieldOfViewLabel;
    
    public void FieldOfView(float newValue)
    {
        fieldOfViewLabel.text = newValue.ToString("F1").Replace(",", ".");
    }

    public void CompassVisibility(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }

    public void ShowChat(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }

    public void ShowNameTags(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }
    
    public void ShowGameTips(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }
}
