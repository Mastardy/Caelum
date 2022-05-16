using UnityEngine;
using UnityEngine.Events;

public class ChatEntry : MonoBehaviour
{
    public bool active;
    public UnityEvent OnDisabling;
    
    private void Start()
    {
        active = true;
        Invoke(nameof(Disable), 5f);
    }

    private void Disable()
    {
        active = false;
        gameObject.SetActive(false);
        Debug.Log("Disable");
        OnDisabling.Invoke();
    }
}
