using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.Windows.WebCam;

public class Player : NetworkBehaviour
{
    private Transform playerCamera;
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

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>().gameObject.transform;
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;
        
        if (lookingAt == null) return;
        
        if (lookingAt.TryGetComponent(out Resource resource))
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                resource.HitResourceServerRpc(this, resource.resourceName, 2);
            }
        }
    }

    [ClientRpc]
    public void GatherResourcesClientRpc(string resourceType, int resourceGathered)
    {
        switch (resourceType)
        {
            case "Tree":
                Wood += resourceGathered;
                break;
            case "Stone":
                Stone += resourceGathered;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;
        
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

    private void Start()
    {
        var cameraTransform = GetComponentInChildren<Camera>().transform;
        
        //
        // TODO: REFACTOR ALL OF THIS CODE
        //
        
        if (!IsLocalPlayer)
        {
            cameraTransform.gameObject.SetActive(false);
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            Destroy(this);
        }
        else
        {
            var cameraMain = Camera.main;
            if(cameraMain != null)
                if(cameraMain != cameraTransform.GetComponent<Camera>())
                    cameraMain.gameObject.SetActive(false);   
        }
    }
}
