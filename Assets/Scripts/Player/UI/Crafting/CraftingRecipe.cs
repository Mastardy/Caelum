using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeRequirements
{
    public InventoryItem item;
    public int amount;
}

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/CraftingRecipe", order = 3)]
public class CraftingRecipe : ScriptableObject
{
    public List<RecipeRequirements> requirements;
    public InventoryItem result;
    public int amount = 1;
}
