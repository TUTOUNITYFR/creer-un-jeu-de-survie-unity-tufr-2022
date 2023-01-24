using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Equipment equipmentSystem;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private BuildSystem buildSystem;

    [SerializeField]
    private MainMenu mainMenu;

    private void Start()
    {
        if(MainMenu.loadSavedData)
        {
            LoadData();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            SaveData();
        }

        if(Input.GetKeyDown(KeyCode.F9))
        {
            LoadData();
        }
    }

    public void SaveData()
    {
        SavedData savedData = new SavedData
        {
            playerPositions = playerTransform.position,
            inventoryContent = Inventory.instance.GetContent(),
            equipedHeadItem = equipmentSystem.equipedHeadItem,
            equipedChestItem = equipmentSystem.equipedChestItem,
            equipedHandsItem = equipmentSystem.equipedHandsItem,
            equipedLegsItem = equipmentSystem.equipedLegsItem,
            equipedFeetItem = equipmentSystem.equipedFeetItem,
            equipedWeaponItem = equipmentSystem.equipedWeaponItem,
            currentHealth = playerStats.currentHealth,
            currentHunger = playerStats.currentHunger,
            currentThirst = playerStats.currentThirst,
            placedStructures = buildSystem.placedStructures.ToArray()
        };

        string jsonData = JsonUtility.ToJson(savedData);
        string filePath = Application.persistentDataPath + "/SavedData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, jsonData);
        Debug.Log("Sauvegarde effectuée");

        mainMenu.loadGameButton.interactable = true;
        mainMenu.clearSavedDataButton.interactable = true;
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + "/SavedData.json";
        string jsonData = System.IO.File.ReadAllText(filePath);

        SavedData savedData = JsonUtility.FromJson<SavedData>(jsonData);

        // Chargement des données
        playerTransform.position = savedData.playerPositions;

        equipmentSystem.LoadEquipments(new ItemData[] { 
            savedData.equipedHeadItem,
            savedData.equipedChestItem,
            savedData.equipedHandsItem,
            savedData.equipedLegsItem,
            savedData.equipedFeetItem,
            savedData.equipedWeaponItem
        });

        Inventory.instance.LoadData(savedData.inventoryContent);

        playerStats.currentHealth = savedData.currentHealth;
        playerStats.currentHunger = savedData.currentHunger;
        playerStats.currentThirst = savedData.currentThirst;
        playerStats.UpdateHealthBarFill();

        buildSystem.LoadStructures(savedData.placedStructures);

        Debug.Log("Chargement terminé");
    }
}

public class SavedData
{
    public Vector3 playerPositions;
    public List<ItemInInventory> inventoryContent;
    public ItemData equipedHeadItem;
    public ItemData equipedChestItem;
    public ItemData equipedHandsItem;
    public ItemData equipedLegsItem;
    public ItemData equipedFeetItem;
    public ItemData equipedWeaponItem;
    public float currentHealth;
    public float currentHunger;
    public float currentThirst;
    public PlacedStructure[] placedStructures;
}