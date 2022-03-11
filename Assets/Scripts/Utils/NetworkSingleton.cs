using UnityEngine;
using Unity.Netcode;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T>
{
    public static T Instance { get; private set; }
    public static bool IsInitialized => Instance != null;

    protected virtual void Awake()
    {
        if (IsLocalPlayer)
        {
            if (Instance != null) Debug.LogError("[Singleton] Possible duplicate of a singleton: " + typeof(T));
            else Instance = (T)this;
        }
        else
        {
            enabled = false;
        }
    }

    public override void OnDestroy()
    {
        if (IsLocalPlayer)
        {
            if (Instance == this) Instance = null;
        }
    }
}