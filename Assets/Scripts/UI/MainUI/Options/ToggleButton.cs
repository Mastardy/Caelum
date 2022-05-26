using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    private bool flag;
    public bool Value
    {
        get => flag;
        set
        {
            flag = value;
            if (flag)
            {
                label.text = "on";
                label.color = new Color(0.95f, 0.8f, 0.6f);
                return;
            }

            label.text = "off";
            label.color = new Color(0.7f, 0.7f, 0.7f);
        }
    }
    
    [SerializeField] private TextMeshProUGUI label;
}