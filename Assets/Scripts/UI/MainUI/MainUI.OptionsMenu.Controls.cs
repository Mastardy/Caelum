using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Controls")]
    [SerializeField] private TextMeshProUGUI mouseSensitivity;
    
    public void MouseSensitivity(float newValue)
    {
        mouseSensitivity.text = newValue.ToString("F1").Replace(",", ".");
    }
    
    public void ToggleDuck(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }
    
    public void ToggleSprint(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
    }
}
