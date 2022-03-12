using TMPro;
using UnityEngine;

public partial class Player
{
    [Header("UI")]
    [SerializeField] private Canvas playerCanvas;
    
    [SerializeField] private TextMeshProUGUI aimText;
    [SerializeField] private LayerMask resourceMask;
    
    [HideInInspector] public GameObject lookingAt;
    
    [SerializeField] private TextMeshProUGUI woodText;
    private int wood;
    public int Wood
    {
        get => wood;
        set
        {
            woodText.SetText(value.ToString());
            wood = value;
        }
    }
    
    [SerializeField] private TextMeshProUGUI stoneText;
    private int stone;
    private int Stone
    {
        get => stone;
        set
        {
            stoneText.SetText(value.ToString());
            stone = value;
        }
    }
}
