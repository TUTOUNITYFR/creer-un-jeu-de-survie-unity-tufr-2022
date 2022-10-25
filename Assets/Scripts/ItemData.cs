using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/New item")]
public class ItemData : ScriptableObject
{
    [Header("Data")]
    public string name;
    public string description;
    public Sprite visual;
    public GameObject prefab;
    public bool stackable;
    public int maxStack;

    [Header("Effects")]
    public float healthEffect;
    public float hungerEffect;
    public float thirstEffect;

    [Header("Armor Stats")]
    public float armorPoints;

    [Header("Attack Stats")]
    public float attackPoints;

    [Header("Types")]
    public ItemType itemType;
    public EquipmentType equipmentType;
}

public enum ItemType
{
    Ressource,
    Equipment,
    Consumable
}

public enum EquipmentType
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
    Weapon
}