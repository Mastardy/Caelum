using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }
    public static bool IsInitialized => Instance != null;

    protected virtual void Awake()
    {
        if (Instance != null) Debug.LogError("[Singleton] Possible duplicate of a singleton: " + typeof(T));
        else Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}