using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private Equipment equipment;

    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    [SerializeField]
    private CraftingSystem craftingSystem;

    [SerializeField]
    private BuildSystem buildSystem;

    [Header("INVENTORY SYSTEM VARIABLES")]

    [SerializeField]
    private List<ItemInInventory> content = new List<ItemInInventory>();

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform inventorySlotsParent;

    public Sprite emptySlotVisual;

    public static Inventory instance;


    const int InventorySize = 24;
    private bool isOpen = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseInventory();
        RefreshContent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(isOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    public void AddItem(ItemData item)
    {
        ItemInInventory[] itemInInventory = content.Where(elem => elem.itemData == item).ToArray();

        bool itemAdded = false;

        if(itemInInventory.Length > 0 && item.stackable)
        {
            for (int i = 0; i < itemInInventory.Length; i++)
            {
                if(itemInInventory[i].count < item.maxStack)
                {
                    itemAdded = true;
                    itemInInventory[i].count++;
                    break;
                }
            }

            if(!itemAdded)
            {
                content.Add(
                    new ItemInInventory
                    {
                        itemData = item,
                        count = 1
                    }
                );
            }
        }
        else
        {
            content.Add(
                new ItemInInventory
                {
                    itemData = item,
                    count = 1
                }
            );
        }

        RefreshContent();
    }

    public void RemoveItem(ItemData item)
    {
        ItemInInventory itemInInventory = content.Where(elem => elem.itemData == item).FirstOrDefault();

        if(itemInInventory != null && itemInInventory.count > 1)
        {
            itemInInventory.count--;
        }
        else
        {
            content.Remove(itemInInventory);
        }

        RefreshContent();
    }

    public List<ItemInInventory> GetContent()
    {
        return content;
    }

    private void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        isOpen = true;
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        itemActionsSystem.actionPanel.SetActive(false);
        TooltipSystem.instance.Hide();
        isOpen = false;
    }

    public void RefreshContent()
    {
        // On vide tous les slots / visuels
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = null;
            currentSlot.itemVisual.sprite = emptySlotVisual;
            currentSlot.countText.enabled = false;
        }

        // On peuple le visuel des slots selon le contenu réel de l'inventaire
        for (int i = 0; i < content.Count; i++)
        {
            Slot currentSlot = inventorySlotsParent.GetChild(i).GetComponent<Slot>();

            currentSlot.item = content[i].itemData;
            currentSlot.itemVisual.sprite = content[i].itemData.visual;

            if(currentSlot.item.stackable)
            {
                currentSlot.countText.enabled = true;
                currentSlot.countText.text = content[i].count.ToString();
            }
        }

        equipment.UpdateEquipmentsDesequipButtons();
        craftingSystem.UpdateDisplayedRecipes();
        buildSystem.UpdateDisplayedCosts();
    }

    public bool IsFull()
    {
        return InventorySize == content.Count;
    }

}

[System.Serializable]
public class ItemInInventory
{
    public ItemData itemData;
    public int count;
}