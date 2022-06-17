using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject ropeTip;
    public GameObject ropeStart;
    public GameObject Hook;
    public GameObject HookAnchor;

    void Update()
    {
        ropeTip.transform.position = Hook.transform.position;
    }
}
