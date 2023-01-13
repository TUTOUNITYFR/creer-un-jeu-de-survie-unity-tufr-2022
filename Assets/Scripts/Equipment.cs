using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Equipment : MonoBehaviour
{
    [Header("OTHER SCRIPTS REFERENCES")]

    [SerializeField]
    private ItemActionsSystem itemActionsSystem;

    [SerializeField]
    private PlayerStats playerStats;

    [Header("EQUIPMENT SYSTEM VARIABLES")]

    [SerializeField]
    private EquipmentLibrary equipmentLibrary;

    [SerializeField]
    private Image headSlotImage;

    [SerializeField]
    private Image chestSlotImage;

    [SerializeField]
    private Image handsSlotImage;

    [SerializeField]
    private Image legsSlotImage;

    [SerializeField]
    private Image feetSlotImage;

    [SerializeField]
    private Image weaponSlotImage;

    // Garde une trace des équipements actuels
    [HideInInspector]
    public ItemData equipedHeadItem;
    [HideInInspector]
    public ItemData equipedChestItem;
    [HideInInspector]
    public ItemData equipedHandsItem;
    [HideInInspector]
    public ItemData equipedLegsItem;
    [HideInInspector]
    public ItemData equipedFeetItem;
    [HideInInspector]
    public ItemData equipedWeaponItem;

    [SerializeField]
    private Button headSlotDesequipButton;

    [SerializeField]
    private Button chestSlotDesequipButton;

    [SerializeField]
    private Button handsSlotDesequipButton;

    [SerializeField]
    private Button legsSlotDesequipButton;

    [SerializeField]
    private Button feetSlotDesequipButton;

    [SerializeField]
    private Button weaponSlotDesequipButton;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip equipSound;

    private void DisablePreviousEquipedEquipment(ItemData itemToDisable)
    {
        if (itemToDisable == null)
        {
            return;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == itemToDisable).First();

        if (equipmentLibraryItem != null)
        {
            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }

            equipmentLibraryItem.itemPrefab.SetActive(false);
        }

        playerStats.currentArmorPoints -= itemToDisable.armorPoints;
        Inventory.instance.AddItem(itemToDisable);
    }

    public void DesequipEquipment(EquipmentType equipmentType)
    {

        if (Inventory.instance.IsFull())
        {
            Debug.Log("L'inventaire est plein, impossible de se déséquiper de cet élément");
            return;
        }

        ItemData currentItem = null;

        switch (equipmentType)
        {
            case EquipmentType.Head:
                currentItem = equipedHeadItem;
                equipedHeadItem = null;
                headSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;

            case EquipmentType.Chest:
                currentItem = equipedChestItem;
                equipedChestItem = null;
                chestSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;

            case EquipmentType.Hands:
                currentItem = equipedHandsItem;
                equipedHandsItem = null;
                handsSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;

            case EquipmentType.Legs:
                currentItem = equipedLegsItem;
                equipedLegsItem = null;
                legsSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;

            case EquipmentType.Feet:
                currentItem = equipedFeetItem;
                equipedFeetItem = null;
                feetSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;

            case EquipmentType.Weapon:
                currentItem = equipedWeaponItem;
                equipedWeaponItem = null;
                weaponSlotImage.sprite = Inventory.instance.emptySlotVisual;
                break;
        }

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == currentItem).FirstOrDefault();

        if (equipmentLibraryItem != null)
        {
            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }

            equipmentLibraryItem.itemPrefab.SetActive(false);
        }

        if(currentItem)
        {
            playerStats.currentArmorPoints -= currentItem.armorPoints;
            Inventory.instance.AddItem(currentItem);
        }
        
        Inventory.instance.RefreshContent();
    }

    public void UpdateEquipmentsDesequipButtons()
    {
        headSlotDesequipButton.onClick.RemoveAllListeners();
        headSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Head); });
        headSlotDesequipButton.gameObject.SetActive(equipedHeadItem);

        chestSlotDesequipButton.onClick.RemoveAllListeners();
        chestSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Chest); });
        chestSlotDesequipButton.gameObject.SetActive(equipedChestItem);

        handsSlotDesequipButton.onClick.RemoveAllListeners();
        handsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Hands); });
        handsSlotDesequipButton.gameObject.SetActive(equipedHandsItem);

        legsSlotDesequipButton.onClick.RemoveAllListeners();
        legsSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Legs); });
        legsSlotDesequipButton.gameObject.SetActive(equipedLegsItem);

        feetSlotDesequipButton.onClick.RemoveAllListeners();
        feetSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Feet); });
        feetSlotDesequipButton.gameObject.SetActive(equipedFeetItem);

        weaponSlotDesequipButton.onClick.RemoveAllListeners();
        weaponSlotDesequipButton.onClick.AddListener(delegate { DesequipEquipment(EquipmentType.Weapon); });
        weaponSlotDesequipButton.gameObject.SetActive(equipedWeaponItem);
    }

    public void EquipAction(ItemData equipment = null)
    {
        ItemData itemToEquip = equipment ? equipment : itemActionsSystem.itemCurrentlySelected;

        print("Equip item : " + itemToEquip.name);

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == itemToEquip).First();

        if (equipmentLibraryItem != null)
        {
            switch (itemToEquip.equipmentType)
            {
                case EquipmentType.Head:
                    DisablePreviousEquipedEquipment(equipedHeadItem);
                    headSlotImage.sprite = itemToEquip.visual;
                    equipedHeadItem = itemToEquip;
                    break;

                case EquipmentType.Chest:
                    DisablePreviousEquipedEquipment(equipedChestItem);
                    chestSlotImage.sprite = itemToEquip.visual;
                    equipedChestItem = itemToEquip;
                    break;

                case EquipmentType.Hands:
                    DisablePreviousEquipedEquipment(equipedHandsItem);
                    handsSlotImage.sprite = itemToEquip.visual;
                    equipedHandsItem = itemToEquip;
                    break;

                case EquipmentType.Legs:
                    DisablePreviousEquipedEquipment(equipedLegsItem);
                    legsSlotImage.sprite = itemToEquip.visual;
                    equipedLegsItem = itemToEquip;
                    break;

                case EquipmentType.Feet:
                    DisablePreviousEquipedEquipment(equipedFeetItem);
                    feetSlotImage.sprite = itemToEquip.visual;
                    equipedFeetItem = itemToEquip;
                    break;

                case EquipmentType.Weapon:
                    DisablePreviousEquipedEquipment(equipedWeaponItem);
                    weaponSlotImage.sprite = itemToEquip.visual;
                    equipedWeaponItem = itemToEquip;
                    break;
            }

            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(false);
            }

            equipmentLibraryItem.itemPrefab.SetActive(true);

            playerStats.currentArmorPoints += itemToEquip.armorPoints;

            Inventory.instance.RemoveItem(itemToEquip);

            audioSource.PlayOneShot(equipSound);
        }
        else
        {
            Debug.LogError("Equipment : " + itemToEquip.name + " non existant dans la librairie des équipements");
        }

        itemActionsSystem.CloseActionPanel();
    }

    public void LoadEquipments(ItemData[] savedEquipments)
    {
        // 1. On efface le contenu de l'inventaire pour éviter que DesequipEquipment ne soit bloqué par un inventaire full
        Inventory.instance.ClearContent();

        // 2. On déséquipe tout ce qu'il y a actuellement sur le joueur
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            DesequipEquipment(type);
        }

        // 3. Chargement des équipements
        foreach (ItemData item in savedEquipments)
        {
            if(item)
            {
                EquipAction(item);
            }
        }
    }
}
