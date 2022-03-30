using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Graphics")]
    [SerializeField] private TextMeshProUGUI framesPerSecondLimit;
    
    public void FramesPerSecondLimit(float newValue)
    {
        if (newValue == 0) framesPerSecondLimit.text = "INF";
        else framesPerSecondLimit.text = newValue.ToString("N0");
    }
}