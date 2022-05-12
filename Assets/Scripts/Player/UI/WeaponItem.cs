using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "ScriptableObjects/WeaponItem")]
public class WeaponItem : ScriptableObject
{
    public string itemName;
    public GameObject weaponPrefab;
}