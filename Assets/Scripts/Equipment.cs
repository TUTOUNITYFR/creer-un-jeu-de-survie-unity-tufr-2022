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
    private ItemData equipedHeadItem;
    private ItemData equipedChestItem;
    private ItemData equipedHandsItem;
    private ItemData equipedLegsItem;
    private ItemData equipedFeetItem;
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

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == currentItem).First();

        if (equipmentLibraryItem != null)
        {
            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(true);
            }

            equipmentLibraryItem.itemPrefab.SetActive(false);
        }

        playerStats.currentArmorPoints -= currentItem.armorPoints;

        Inventory.instance.AddItem(currentItem);
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

    public void EquipAction()
    {
        print("Equip item : " + itemActionsSystem.itemCurrentlySelected.name);

        EquipmentLibraryItem equipmentLibraryItem = equipmentLibrary.content.Where(elem => elem.itemData == itemActionsSystem.itemCurrentlySelected).First();

        if (equipmentLibraryItem != null)
        {
            switch (itemActionsSystem.itemCurrentlySelected.equipmentType)
            {
                case EquipmentType.Head:
                    DisablePreviousEquipedEquipment(equipedHeadItem);
                    headSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedHeadItem = itemActionsSystem.itemCurrentlySelected;
                    break;

                case EquipmentType.Chest:
                    DisablePreviousEquipedEquipment(equipedChestItem);
                    chestSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedChestItem = itemActionsSystem.itemCurrentlySelected;
                    break;

                case EquipmentType.Hands:
                    DisablePreviousEquipedEquipment(equipedHandsItem);
                    handsSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedHandsItem = itemActionsSystem.itemCurrentlySelected;
                    break;

                case EquipmentType.Legs:
                    DisablePreviousEquipedEquipment(equipedLegsItem);
                    legsSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedLegsItem = itemActionsSystem.itemCurrentlySelected;
                    break;

                case EquipmentType.Feet:
                    DisablePreviousEquipedEquipment(equipedFeetItem);
                    feetSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedFeetItem = itemActionsSystem.itemCurrentlySelected;
                    break;

                case EquipmentType.Weapon:
                    DisablePreviousEquipedEquipment(equipedWeaponItem);
                    weaponSlotImage.sprite = itemActionsSystem.itemCurrentlySelected.visual;
                    equipedWeaponItem = itemActionsSystem.itemCurrentlySelected;
                    break;
            }

            for (int i = 0; i < equipmentLibraryItem.elementsToDisable.Length; i++)
            {
                equipmentLibraryItem.elementsToDisable[i].SetActive(false);
            }

            equipmentLibraryItem.itemPrefab.SetActive(true);

            playerStats.currentArmorPoints += itemActionsSystem.itemCurrentlySelected.armorPoints;

            Inventory.instance.RemoveItem(itemActionsSystem.itemCurrentlySelected);

            audioSource.PlayOneShot(equipSound);
        }
        else
        {
            Debug.LogError("Equipment : " + itemActionsSystem.itemCurrentlySelected.name + " non existant dans la librairie des équipements");
        }

        itemActionsSystem.CloseActionPanel();
    }
}
