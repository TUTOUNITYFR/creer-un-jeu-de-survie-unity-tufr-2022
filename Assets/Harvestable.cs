using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    [SerializeField]
    public Ressource[] harvestableItems;
}

[System.Serializable]
public class Ressource
{
    public ItemData itemData;
    public int minAmountSpawned;
    public int maxAmountSpawned;
}