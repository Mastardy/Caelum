using System.Collections;
using TMPro;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Invoke(nameof(DestroyGameObject), 2f);
    }

    private void DestroyGameObject()
    {
        StartCoroutine(DestroyGameObjectWithFade());
    }

    private IEnumerator DestroyGameObjectWithFade()
    {
        while (true)
        {
            canvasGroup.alpha -= Time.deltaTime * 2f;
            if (canvasGroup.alpha <= 0) Destroy(gameObject);
            yield return canvasGroup.alpha <= 0;
        }
    }
}