using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    public void Interact(Interactor interactor, out bool interactSuccesful)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if (inventory == null)
        {
            interactSuccesful = false;
        }

        if (inventory.HasKey)
        {
            Debug.Log("Opening door!");
            interactSuccesful = true;
        }

        Debug.Log("No key found!");
        interactSuccesful = false;
    }

    public void EndInteraction()
    {
        
    }



    //[SerializeField] private string _prompt;
    //public string InteractionPrompt => _prompt;

    //public bool Interact(Interactor interactor)
    //{
    //    var inventory = interactor.GetComponent<Inventory>();

    //    if (inventory == null)
    //    {
    //        return false;
    //    }

    //    if (inventory.HasKey)
    //    {
    //        Debug.Log("Opening door!");
    //        return true;
    //    }

    //    Debug.Log("No key found!");
    //    return false;
    //}
}
