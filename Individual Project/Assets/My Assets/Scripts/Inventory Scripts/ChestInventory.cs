using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem);
        Debug.Log("Chest opened!");
        interactSuccessful = true;
    }

    public void EndInteraction()
    {

    }









    // "Old code"
    //[SerializeField] private string _prompt;
    //public string InteractionPrompt => _prompt;

    //public bool Interact(Interactor interactor)
    //{
    //    OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem);

    //    Debug.Log("Opening chest!");
    //    return true;
    //}
}
