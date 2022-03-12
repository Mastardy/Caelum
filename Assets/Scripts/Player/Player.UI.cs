using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    private void EyeTrace()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, 4, resourceMask))
        {
            if (hitInfo.transform.TryGetComponent(out Resource resource))
            {
                aimText.SetText(resource.resourceName);
                lookingAt = resource.gameObject;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);
    }

    private void EyeTraceInfo()
    {
        if (lookingAt == null) return;
        
        if (lookingAt.TryGetComponent(out Resource resource))
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                resource.HitResourceServerRpc(this, resource.resourceName, 2);
            }
        }
    }
}
