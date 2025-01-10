using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";

    public Inventory inventory;

    CustomActions input;

    NavMeshAgent agent;
    Animator animator;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8f;
    float animationThreshold = 0.1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new CustomActions();
        AssignInputs();
    }

    void AssignInputs()
    {
        input.Main.Move.performed += c1x => ClickToMove();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100, clickableLayers))
        {
            agent.destination = hit.point;
            if (clickEffect != null)
            {
                ParticleSystem newEffect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                Destroy(newEffect.gameObject, newEffect.main.duration);
            }
        }
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        FaceTarget();
        SetAnimations();
    }

    void FaceTarget()
    {
        if (agent.remainingDistance > animationThreshold)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    void SetAnimations()
    {
        if (agent.velocity.magnitude <= animationThreshold)
        {
            animator.SetBool("IsWalking", false);
            Debug.Log("Idle");
        }
        else
        {
            animator.SetBool("IsWalking", true);
            Debug.Log("Walking");
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
        if (item != null)
        {
            inventory.AddItem(item);
        }
    }
}