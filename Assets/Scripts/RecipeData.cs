using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipes/New Recipe")]
public class RecipeData : ScriptableObject
{
    public ItemData craftableItem;
    public ItemInInventory[] requiredItems;
}
