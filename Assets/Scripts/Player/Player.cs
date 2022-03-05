using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    private void Start()
    {
        var cameraTransform = GetComponentInChildren<Camera>().transform;
        
        //
        // TODO: REFACTOR ALL OF THIS CODE
        //
        
        if (!IsLocalPlayer)
        {
            cameraTransform.gameObject.SetActive(false);
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
