using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBehaviour : MonoBehaviour
{
    [SerializeField]
    private MoveBehaviour playerMoveBehaviour;

    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Inventory inventory;

    private Item currentItem;
    private Harvestable currentHarvestable;

    [SerializeField]
    private GameObject pickaxeVisual;

    public void DoPickup(Item item)
    {
        if(inventory.IsFull())
        {
            Debug.Log("Inventory full, can't pick up : " + item.name);
            return;
        }

        currentItem = item;

        playerAnimator.SetTrigger("Pickup");
        playerMoveBehaviour.canMove = false;
    }

    public void DoHarvest(Harvestable harvestable)
    {
        pickaxeVisual.SetActive(true);
        currentHarvestable = harvestable;
        playerAnimator.SetTrigger("Harvest");
        playerMoveBehaviour.canMove = false;
    }

    public void BreakHarvestable()
    {
        for (int i = 0; i < currentHarvestable.harvestableItems.Length; i++)
        {
            Ressource ressource = currentHarvestable.harvestableItems[i];

            for (int y = 0; y < Random.Range(ressource.minAmountSpawned, (float)ressource.maxAmountSpawned); y++)
            {
                GameObject instantiatedRessource = GameObject.Instantiate(ressource.itemData.prefab);
                instantiatedRessource.transform.position = currentHarvestable.transform.position;
            }
        }

        Destroy(currentHarvestable.gameObject);
    }

    public void AddItemToInventory()
    {
        inventory.AddItem(currentItem.itemData);
        Destroy(currentItem.gameObject);
    }

    public void ReEnablePlayerMovement()
    {
        pickaxeVisual.SetActive(false);
        playerMoveBehaviour.canMove = true;
    }
}
