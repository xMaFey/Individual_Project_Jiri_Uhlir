using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Transform InteractionPoint;
    public LayerMask InteractionLayer;
    public float InteractionPointRadius = 1f;
    public IInteractable closestInteractable;
    private bool IsInteracting;
    private IInteractable currentInteractable;

    [SerializeField] private InteractionPromptUI interactionPromptUI;

    private void Update()
    {
        var colliders = Physics.OverlapSphere(InteractionPoint.position, InteractionPointRadius, InteractionLayer);
        IInteractable nearestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            var interactable = collider.GetComponent<IInteractable>();
            if(interactable != null)
            {
                float distance = Vector3.Distance(InteractionPoint.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }

        // Update the interaction prompt
        if (nearestInteractable != closestInteractable)
        {
            if (closestInteractable != null)
            {
                // Hide the old prompt
                closestInteractable.EndInteraction();
                interactionPromptUI.Close();
            }

            closestInteractable = nearestInteractable;

            if (closestInteractable != null)
            {
                interactionPromptUI.SetUp(closestInteractable.InteractionPrompt);
            }
            else
            {
                interactionPromptUI.Close();
            }
        }

        // Handle interaction
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(IsInteracting && currentInteractable != null)
            {
                currentInteractable.EndInteraction();
                currentInteractable = null;
                IsInteracting = false;
            }
            else if(nearestInteractable != null)
            {
                StartInteraction(nearestInteractable);
            }
        }
    }

    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out bool interactSuccessful);
        if (interactSuccessful)
        {
            IsInteracting = true;
            interactionPromptUI.Close();
        }
    }

    void EndInteraction()
    {
        IsInteracting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(InteractionPoint.position, InteractionPointRadius);
    }



    //[SerializeField] private Transform _interactionPoint;
    //[SerializeField] private float _interactionPointRadius = 0.5f;
    //[SerializeField] private LayerMask _interactableMask;
    //[SerializeField] private InteractionPromptUI _interactionPromptUI;

    //private readonly Collider[] _colliders = new Collider[3];
    //[SerializeField] private int _numFound;

    //private IInteractable _interactable;

    //private void Update()
    //{
    //    _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

    //    if (_numFound > 0)
    //    {
    //        _interactable = _colliders[0].GetComponent<IInteractable>();

    //        if (_interactable != null)
    //        {
    //            if (!_interactionPromptUI.IsDisplayed)
    //            {
    //                _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
    //            }

    //            if (Keyboard.current.eKey.wasPressedThisFrame)
    //            {
    //                _interactable.Interact(this);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (_interactable != null)
    //        {
    //            _interactable = null;
    //        }

    //        if (_interactionPromptUI.IsDisplayed)
    //        {
    //            _interactionPromptUI.Close();
    //        }
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    //}
}
