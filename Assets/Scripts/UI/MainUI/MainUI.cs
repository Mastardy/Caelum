using System;
using UnityEngine;

public partial class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private void Awake()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        
        optionsMenu.SetActive(false);
        
        LoadOptions();
    }
}
