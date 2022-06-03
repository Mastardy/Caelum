using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFilter : MonoBehaviour
{
    public float lastAttack;

    private CanvasGroup filter;

    private void Awake()
    {
        filter = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        float timer = (Time.time - lastAttack) * 2;

        filter.alpha = 1 - timer;
    }
}
