using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Calculate a fake delta time, so we can Shake while game is paused.
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame;
    }

    public void PlayShake(int amount)
    {
        if (!instance.enabled) return;
        
        instance._originalPos = instance.gameObject.transform.localPosition;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(0.1f, amount));
    }

    public void StartShake(float dur, int am)
    {
        if (!instance.enabled) return;

        instance._originalPos = instance.gameObject.transform.localPosition;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.cShake(dur, am));
    }

    public void StopShake()
    {
        if (!instance.enabled) return;
        instance.StopAllCoroutines();
        instance.gameObject.transform.localPosition = instance._originalPos;
    }

    public IEnumerator cShake(float duration, int amount)
    {
        float endTime = Time.time + duration;

        while (duration > 0)
        {
            instance.gameObject.transform.localPosition = _originalPos + Random.insideUnitSphere * amount/200.0f;

            duration -= _fakeDelta;

            yield return null;
        }

        instance.gameObject.transform.localPosition = _originalPos;
    }
}
