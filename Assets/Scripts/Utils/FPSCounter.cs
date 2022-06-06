using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsCounter;
    private float lastCount;

    private void Awake()
    {
        fpsCounter = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Time.time - lastCount < 0.35f) return;
        lastCount = Time.time;
        fpsCounter.SetText((1/Time.deltaTime).ToString("F0"));
    }
}
