using UnityEngine;

[CreateAssetMenu()]
public class CropItem : ScriptableObject
{
    public string cropItem;
    public GameObject cropPrefab;
    public string cropResult;
    public float timeToGrow;
    public int resultSeedAmount;
    public int resultCropAmount;
}