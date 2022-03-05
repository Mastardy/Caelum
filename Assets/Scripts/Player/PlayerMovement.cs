using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3;
    
    void Start()
    {
        if(Camera.main) if(Camera.main != GetComponentInChildren<Camera>()) Camera.main.gameObject.SetActive(false);
    }
    
    void Update()
    {
        
    }
}
