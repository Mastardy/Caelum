using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public partial class Player
{
    private int itemId;
    private bool inOven;
    private bool inOvenMinigame;
    private Oven cooker;
    private CookingRecipe currentRecipe;
    
    [Header("Oven")]
    [SerializeField] private Transform recipesContent;
    [SerializeField] private GameObject recipePrefab;

    [SerializeField] private Transform ingredientsContent;
    [SerializeField] private GameObject ingredientPrefab;

    [SerializeField] private GameObject recipeTitle;

    [SerializeField] private TextMeshProUGUI recipeDescription;

    [SerializeField] private FoodAttributeUI hungerAttribute;
    [SerializeField] private FoodAttributeUI thirstAttribute;
    [SerializeField] private FoodAttributeUI temperatureAttribute;

    [SerializeField] private Button cookingMinigameButton;

    [SerializeField] private GameObject ovenMinigame;
    
    private CookingRecipe[] cookingRecipes;

    private Dictionary<string, FoodItem> foodItems = new ();
    
    /// <summary>
    /// Opens Oven
    /// </summary>
    public void OpenOven()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inOven = true;
        takeInput = false;
        ovenPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        
        PrepareOvenRecipe(currentRecipe ? currentRecipe : cookingRecipes[0]);
        PrepareOven();
    }

    /// <summary>
    /// Hides Oven
    /// </summary>
    public void HideOven()
    {
        cooker = null;
        Cursor.lockState = CursorLockMode.Locked;
        inOven = false;
        takeInput = true;
        ovenPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void OpenOvenClientRpc(NetworkBehaviourReference cookerReference)
    {
        if (cooker != null) return;
        cookerReference.TryGet(out cooker);
        if (cooker != null) OpenOven();
    }

    [ClientRpc]
    public void CloseOvenClientRpc()
    {
        HideOven();
    }

    private float currentTimer;
    
    private void PrepareOven()
    {
        foreach (var child in recipesContent.GetComponentsInChildren<Transform>())
        {
            if (child != recipesContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var cookingRecipe in cookingRecipes)
        {
            var recipe = Instantiate(recipePrefab, recipesContent);
            var recipeUI = recipe.GetComponent<CookingRecipeUI>();
            recipeUI.image.sprite = cookingRecipe.cooked.sprite;
            recipeUI.title.SetText(cookingRecipe.cooked.name);
            recipe.GetComponent<Button>().onClick.AddListener(() => PrepareOvenRecipe(cookingRecipe));
        }
    }

    private void PrepareOvenRecipe(CookingRecipe cookingRecipe)
    {
        currentRecipe = cookingRecipe;
        
        foreach (var child in ingredientsContent.GetComponentsInChildren<Transform>())
        {
            if (child != ingredientsContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (var ingredient in cookingRecipe.ingredients)
        {
            var ingr = Instantiate(ingredientPrefab, ingredientsContent);
            var ingrUI = ingr.GetComponent<IngredientUI>();
            ingrUI.title.SetText(ingredient.inventoryItem.name);
            ingrUI.image.sprite = ingredient.inventoryItem.sprite;
            ingrUI.quantity.SetText(GetItemAmount(ingredient.inventoryItem.itemName) + "/" + ingredient.quantity);
        }

        recipeTitle.gameObject.SetActive(true);
        
        recipeDescription.gameObject.SetActive(true);
        recipeDescription.SetText(cookingRecipe.cooked.description);

        hungerAttribute.gameObject.SetActive(false);
        thirstAttribute.gameObject.SetActive(false);
        temperatureAttribute.gameObject.SetActive(false);
        
        if (foodItems[cookingRecipe.cooked.itemName].hunger != 0)
        {
            hungerAttribute.gameObject.SetActive(true);
            hungerAttribute.quantity.SetText(foodItems[cookingRecipe.cooked.itemName].hunger.ToString("F0") + "%");
        }
        if (foodItems[cookingRecipe.cooked.itemName].thirst != 0)
        {
            thirstAttribute.gameObject.SetActive(true);
            thirstAttribute.quantity.SetText(foodItems[cookingRecipe.cooked.itemName].thirst.ToString("F0") + "%");
        }
        if (foodItems[cookingRecipe.cooked.itemName].temperature != 0)
        {
            temperatureAttribute.gameObject.SetActive(true);
            temperatureAttribute.quantity.SetText(foodItems[cookingRecipe.cooked.itemName].temperature.ToString("F0") + "ºC");
        }

        cookingMinigameButton.gameObject.SetActive(true);
        cookingMinigameButton.onClick.RemoveAllListeners();
        cookingMinigameButton.onClick.AddListener(PrepareOvenMinigame);
    }

    private void PrepareOvenMinigame()
    {
        foreach (var ingredient in currentRecipe.ingredients)
        {
            if(GetItemAmount(ingredient.inventoryItem.itemName) < ingredient.quantity) return;
        }
        
        inOvenMinigame = true;
        currentTimer = 0.0f;
        
        int randomValue = Random.Range(5, 95);
        ovenMinigame.SetActive(true);
        ovenScaler.localScale = new Vector2(0, 1);
        ovenArrow.anchoredPosition = new Vector2(randomValue * 4, 0);
    }

    private void OvenMinigameUpdate()
    {
        currentTimer += Time.deltaTime / ovenSpeed;
        var currentTimerFloor = Mathf.FloorToInt(currentTimer);
        var tempCurrentTimer = currentTimer - currentTimerFloor;

        if (currentTimerFloor % 2 == 1) tempCurrentTimer = 1 - tempCurrentTimer;

        ovenScaler.localScale = new Vector2(tempCurrentTimer, 1);

        if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.1f))
        {
            var curValue = ovenArrow.anchoredPosition.x / 400f;
            var curOffset = ovenArrow.rect.width / 800f;

            foreach (var ingredient in currentRecipe.ingredients)
            {
                RemoveItem(ingredient.inventoryItem.itemName, ingredient.quantity);
            }
            
            if (curValue > (tempCurrentTimer - curOffset) && curValue < (tempCurrentTimer + curOffset))
            {
                GiveItemServerRpc(this, currentRecipe.cooked.itemName, 1);
            }
            else
            {
                GiveItemServerRpc(this, currentRecipe.burnt.itemName, 1);
            }
            
            ovenMinigame.SetActive(false);
            inOvenMinigame = false;
            
            PrepareOvenRecipe(currentRecipe);
        }
    }
}
