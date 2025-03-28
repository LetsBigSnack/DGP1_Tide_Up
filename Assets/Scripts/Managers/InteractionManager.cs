using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 4.0f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Interactable currentInteractable;
    
    public static InteractionManager Instance;
    
    public static Action<bool> OnInteractionChanged;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FixedUpdate()
    {

        CheckCurrentInteractable();

        GetInteractablesInRadius();

        if (currentInteractable != null)
        {
            currentInteractable.ShowInteractability(true);
            OnInteractionChanged?.Invoke(true);
        }
    }

    private void GetInteractablesInRadius()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider hit in hits)
        {
            Interactable item = hit.GetComponentInParent<Interactable>();
            if (item != null)
            {
                float dist = Vector3.Distance(transform.position, item.transform.position);
                item.ShowInteractability(false);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    currentInteractable = item;
                    
                }
            }
        }
    }

    private void CheckCurrentInteractable()
    {
        if (currentInteractable == null)
        {
            OnInteractionChanged?.Invoke(false);
            return;
        }
        
        float dist = Vector3.Distance(transform.position, currentInteractable.transform.position);
        if (dist > interactionRadius)
        {
            currentInteractable?.ShowInteractability(false);
            OnInteractionChanged?.Invoke(false);
            currentInteractable = null;
        }
    }

    public void Interact()
    {
        if (currentInteractable == null)
        {
            return;
        }
        currentInteractable.Interact();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(204.0f/255.0f,85.0f/255.0f,0,1);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
