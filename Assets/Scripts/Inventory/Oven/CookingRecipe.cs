using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public InventoryItem inventoryItem;
    public int quantity;
}

[CreateAssetMenu(fileName = "CookingRecipe", menuName = "ScriptableObjects/CookingRecipe", order = 2)]
public class CookingRecipe : ScriptableObject
{
    public List<Ingredient> ingredients;
    public InventoryItem cooked;
    public InventoryItem burnt;
}
