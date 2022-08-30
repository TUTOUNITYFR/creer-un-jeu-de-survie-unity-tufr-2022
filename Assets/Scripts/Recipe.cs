using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Recipe : MonoBehaviour
{
    private RecipeData currentRecipe;

    [SerializeField]
    private Image craftableItemImage;

    [SerializeField]
    private GameObject elementRequiredPrefab;

    [SerializeField]
    private Transform elementsRequiredParent;

    [SerializeField]
    private Button craftButton;

    [SerializeField]
    private Sprite canBuildIcon;

    [SerializeField]
    private Sprite cantBuildIcon;

    [SerializeField]
    private Color missingColor;

    [SerializeField]
    private Color availableColor;

    public void Configure(RecipeData recipe)
    {
        currentRecipe = recipe;

        craftableItemImage.sprite = recipe.craftableItem.visual;

        // Slot permet l'affichage du tooltip lorsqu'on lui passe un Item
        craftableItemImage.transform.parent.GetComponent<Slot>().item = recipe.craftableItem;

        bool canCraft = true;

        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            // Récupère tous les éléments nécessaires pour cette recette
            GameObject requiredItemGO = Instantiate(elementRequiredPrefab, elementsRequiredParent);
            Image requiredItemGOImage = requiredItemGO.GetComponent<Image>();
            ItemData requiredItem = recipe.requiredItems[i].itemData;
            ElementRequired elementRequired = requiredItemGO.GetComponent<ElementRequired>();

            // Slot permet l'affichage du tooltip lorsqu'on lui passe un Item
            requiredItemGO.GetComponent<Slot>().item = requiredItem;

            // Si l'inventaire contient l'élément requis on le retire de l'inventaire et on passe au suivant
            ItemInInventory[] itemInInventory = Inventory.instance.GetContent().Where(elem => elem.itemData == requiredItem).ToArray();

            int totalRequiredItemQuantityInInventory = 0;
            for (int y = 0; y < itemInInventory.Length; y++)
            {
                totalRequiredItemQuantityInInventory += itemInInventory[y].count;
            }

            if (totalRequiredItemQuantityInInventory >= recipe.requiredItems[i].count)
            {
                requiredItemGOImage.color = availableColor;
            }
            else
            {
                requiredItemGOImage.color = missingColor;
                canCraft = false;
            }

            // Configure le visuel de l'élément requis
            elementRequired.elementImage.sprite = recipe.requiredItems[i].itemData.visual;
            elementRequired.elementCountText.text = recipe.requiredItems[i].count.ToString();
        }

        // Gestion de l'affichage du bouton
        craftButton.image.sprite = canCraft ? canBuildIcon : cantBuildIcon;
        craftButton.enabled = canCraft;

        ResizeElementsRequiredParent();
    }

    private void ResizeElementsRequiredParent()
    {
        Canvas.ForceUpdateCanvases();
        elementsRequiredParent.GetComponent<ContentSizeFitter>().enabled = false;
        elementsRequiredParent.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void CraftItem()
    {
        for (int i = 0; i < currentRecipe.requiredItems.Length; i++)
        {
            for (int y = 0; y < currentRecipe.requiredItems[i].count; y++)
            {
                Inventory.instance.RemoveItem(currentRecipe.requiredItems[i].itemData);
            }
        }

        Inventory.instance.AddItem(currentRecipe.craftableItem);
    }
}
