using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    public InventoryItemData ItemData;

    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<InventoryHolder>();

        if (inventory == null)
        {
            return false;
        }

        if (inventory.InventorySystem.AddToInventory(ItemData, 1))
        {

            Debug.Log("Picking up Item!");
            Destroy(this.gameObject);
        }

        return true;
    }
}
