using UnityEngine;

[CreateAssetMenu(fileName = "CookingRecipe", menuName = "ScriptableObjects/CookingRecipe", order = 2)]
public class CookingRecipe : ScriptableObject
{
    public InventoryItem raw;
    public InventoryItem cooked;
    public InventoryItem burnt;
}
