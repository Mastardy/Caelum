using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip musicClip;
    
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Cringei");
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Cringei Logo");
            AudioManager.Instance.PlayMusic(musicClip);
        }
    }
}
