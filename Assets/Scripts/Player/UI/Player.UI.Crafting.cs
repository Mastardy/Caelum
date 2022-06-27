using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public partial class Player
{
    private bool inCrafting;
    private CraftingTable craftingTable;

    [Header("Crafting Menu")]
    [SerializeField] private GameObject craftingRecipePrefab;
    [SerializeField] private Transform craftingRecipesContent;

    [SerializeField] private Image craftingRecipeImage;
    [SerializeField] private TextMeshProUGUI craftingRecipeTitle;

    [SerializeField] private Transform craftingIngredientsContent;

    [SerializeField] private Button craftButton;
    
    private CraftingRecipe[] craftingRecipes;
    private CraftingRecipe currentCraftingRecipe;
    
    /// <summary>
    /// Hides Crafting
    /// </summary>
    public void HideCrafting()
    {
        craftingTable = null;
        Cursor.lockState = CursorLockMode.Locked;
        inCrafting = false;
        takeInput = true;
        craftingPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        tipsText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Opens Crafting
    /// </summary>
    public void OpenCrafting()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inCrafting = true;
        takeInput = false;
        craftingPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        tipsText.gameObject.SetActive(false);
        
        PrepareCraftingRecipe(currentCraftingRecipe ? currentCraftingRecipe : craftingRecipes[0]);
        PrepareCrafting();
    }

    [ClientRpc]
    public void OpenCraftingClientRpc(NetworkBehaviourReference craftTable)
    {
        craftTable.TryGet(out craftingTable);
        if(craftingTable) OpenCrafting();
    }

    [ClientRpc]
    public void CloseCraftingClientRpc()
    {
        HideCrafting();
    }

    private void PrepareCrafting()
    {
        foreach (var child in craftingRecipesContent.GetComponentsInChildren<Transform>())
        {
            if (child != craftingRecipesContent)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var craftingRecipe in craftingRecipes)
        {
            var recipe = Instantiate(craftingRecipePrefab, craftingRecipesContent);
            recipe.GetComponent<CraftingRecipeUI>().image.sprite = craftingRecipe.result.sprite;
            recipe.GetComponent<Button>().onClick.AddListener(() => PrepareCraftingRecipe(craftingRecipe));
        }
    }

    private void PrepareCraftingRecipe(CraftingRecipe craftingRecipe)
    {
        currentCraftingRecipe = craftingRecipe;

        foreach (var child in craftingIngredientsContent.GetComponentsInChildren<Transform>())
        {
            if (child != craftingIngredientsContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var ingredient in craftingRecipe.requirements)
        {
            var ingr = Instantiate(ingredientPrefab, craftingIngredientsContent);
            var ingrUI = ingr.GetComponent<IngredientUI>();
            ingrUI.title.SetText(ingredient.item.name);
            ingrUI.image.sprite = ingredient.item.sprite;
            ingrUI.quantity.SetText(GetItemAmount(ingredient.item.itemName) + "/" + ingredient.amount);
        }
        
        craftingRecipeTitle.SetText(craftingRecipe.result.name);

        craftingRecipeImage.sprite = craftingRecipe.result.sprite;
        
        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(CraftItem);
    }
    
    public void CraftItem()
    {
        foreach(var ingredient in currentCraftingRecipe.requirements)
        {
            if (GetItemAmount(ingredient.item.itemName) < ingredient.amount) return;
        }

        foreach (var ingredient in currentCraftingRecipe.requirements)
        {
            RemoveItem(ingredient.item.itemName, ingredient.amount);
        }
        
        GiveItemServerRpc(this, currentCraftingRecipe.result.itemName, durability: currentCraftingRecipe.result.itemTag is ItemTag.Axe or ItemTag.Bow or ItemTag.Grappling or ItemTag.Pickaxe or ItemTag.Spear or ItemTag.Sword or ItemTag.Armor ? 1 : 0);
        
        PrepareCraftingRecipe(currentCraftingRecipe);
    }
}
