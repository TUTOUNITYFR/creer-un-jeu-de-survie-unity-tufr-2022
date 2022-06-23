using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private float pickupRange = 2.6f;

    public PickupBehaviour playerPickupBehaviour;

    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            if(hit.transform.CompareTag("Item"))
            {
                Debug.Log("There is an item in front of us");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    playerPickupBehaviour.DoPickup(hit.transform.gameObject.GetComponent<Item>());
                }
            }
        }
    }
}
