using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public partial class Player
{
    private int itemId;
    private bool inOven;
    private Oven cooker;
    
    [Header("Oven")]
    [SerializeField] private Transform recipesContent;
    [SerializeField] private GameObject recipePrefab;

    private List<CookingRecipe> cookingRecipes = new();

    private Dictionary<int, FoodItem> foodItems = new ();
    
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
        cookerReference.TryGet(out cooker);
        if(cooker != null) OpenOven();
    }

    [ClientRpc]
    public void CloseOvenClientRpc()
    {
        HideOven();
    }

    private float currentTimer;
    
    private void PrepareOven()
    {
        foreach (var cookingRecipe in cookingRecipes)
        {
            var recipe = Instantiate(recipePrefab, recipesContent);
            var recipeUI = recipe.GetComponent<CookingRecipeUI>();
            recipeUI.image.sprite = cookingRecipe.cooked.sprite;
            recipeUI.title.SetText(cookingRecipe.cooked.name);
            recipe.GetComponent<Button>().onClick.AddListener(() => PrepareRecipe(foodItems[cookingRecipe.cooked.id]));
        }
        // Lista 
    }

    private void PrepareRecipe(FoodItem foodItem)
    {
        
    }
    
    private void PrepareOvenMinigame()
    {
        currentTimer = 0.0f;
        int randomValue = Random.Range(5, 95);
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
            Debug.Log(curValue > (tempCurrentTimer - curOffset) && curValue < (tempCurrentTimer + curOffset) ? "Acertou" : "Errou");
        }
    }
}
