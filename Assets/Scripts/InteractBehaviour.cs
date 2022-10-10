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
    private Tool currentTool;

    private Vector3 spawnItemOffset = new Vector3(0, 0.5f, 0);

    private bool isBusy = false;

    [Header("Tools Visuals")]
    [SerializeField]
    private GameObject pickaxeVisual;

    [SerializeField]
    private GameObject axeVisual;

    public void DoPickup(Item item)
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;

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
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        currentTool = harvestable.tool;
        EnableToolGameObjectFromEnum(currentTool);

        currentHarvestable = harvestable;
        playerAnimator.SetTrigger("Harvest");
        playerMoveBehaviour.canMove = false;
    }

    // Coroutine appelée depuis l'animation "Harvesting"
    IEnumerator BreakHarvestable()
    {
        Harvestable currentlyHarvesting = currentHarvestable;

        // Permet de désactiver la possibilité d'intéragir avec ce Harvestable + d'un fois (passage du layer Harvestable à Default)
        currentlyHarvesting.gameObject.layer = LayerMask.NameToLayer("Default");

        if(currentlyHarvesting.disableKinematicOnHarvest)
        {
            Rigidbody rigidbody = currentlyHarvesting.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(transform.forward * 800, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(currentlyHarvesting.destroyDelay);

        for (int i = 0; i < currentlyHarvesting.harvestableItems.Length; i++)
        {
            Ressource ressource = currentlyHarvesting.harvestableItems[i];

            if(Random.Range(1, 101) <= ressource.dropChance)
            {
                GameObject instantiatedRessource = Instantiate(ressource.itemData.prefab);
                instantiatedRessource.transform.position = currentlyHarvesting.transform.position + spawnItemOffset;
            }
        }

        Destroy(currentlyHarvesting.gameObject);
    }

    public void AddItemToInventory()
    {
        inventory.AddItem(currentItem.itemData);
        Destroy(currentItem.gameObject);
    }

    public void ReEnablePlayerMovement()
    {
        EnableToolGameObjectFromEnum(currentTool, false);
        playerMoveBehaviour.canMove = true;
        isBusy = false;
    }

    private void EnableToolGameObjectFromEnum(Tool toolType, bool enabled = true)
    {
        switch(toolType)
        {
            case Tool.Pickaxe:
                pickaxeVisual.SetActive(enabled);
                break;
            case Tool.Axe:
                axeVisual.SetActive(enabled);
                break;
        }
    }
}
