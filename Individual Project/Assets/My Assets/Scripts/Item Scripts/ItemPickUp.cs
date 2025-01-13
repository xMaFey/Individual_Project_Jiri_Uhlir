using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public InventoryItemData ItemData;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    public void Interact(Interactor interactor, out bool interactSuccesful)
    {
        var inventory = interactor.GetComponent<PlayerInventoryHolder>();

        if (inventory == null)
        {
            interactSuccesful = false;
            return;
        }

        if (inventory.AddToInventory(ItemData, 1))
        {

            Debug.Log("Picking up Item!");
            Destroy(this.gameObject);
        }

        interactSuccesful = true;
    }

    public void EndInteraction()
    {

    }






    //public bool Interact(Interactor interactor)
    //{
    //    var inventory = interactor.GetComponent<InventoryHolder>();

    //    if (inventory == null)
    //    {
    //        return false;
    //    }

    //    if (inventory.InventorySystem.AddToInventory(ItemData, 1))
    //    {

    //        Debug.Log("Picking up Item!");
    //        Destroy(this.gameObject);
    //    }

    //    return true;
    //}
}
