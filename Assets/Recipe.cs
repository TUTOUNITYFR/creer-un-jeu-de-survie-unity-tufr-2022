using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        // On créer une copie de l'inventaire pour vérifier si on a chacun des éléments nécessaires au craft
        List<ItemData> inventoryCopy = new List<ItemData>(Inventory.instance.GetContent());

        for (int i = 0; i < recipe.requiredItems.Length; i++)
        {
            ItemData requiredItem = recipe.requiredItems[i];

            GameObject requiredItemGO = Instantiate(elementRequiredPrefab, elementsRequiredParent);

            // Slot permet l'affichage du tooltip lorsqu'on lui passe un Item
            requiredItemGO.GetComponent<Slot>().item = requiredItem;

            Image requiredItemGOImage = requiredItemGO.GetComponent<Image>();

            // Si la copie d'inventaire contient l'élément requis on le retire de l'inventaire et on passe au suivant
            if (inventoryCopy.Contains(requiredItem))
            {
                requiredItemGOImage.color = availableColor;
                inventoryCopy.Remove(requiredItem);
            }
            else
            {
                requiredItemGOImage.color = missingColor;
                canCraft = false;
            }

            requiredItemGO.transform.GetChild(0).GetComponent<Image>().sprite = recipe.requiredItems[i].visual;
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
            Inventory.instance.RemoveItem(currentRecipe.requiredItems[i]);
        }

        Inventory.instance.AddItem(currentRecipe.craftableItem);
    }
}
