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
    
    public static Action<bool, InteractableType?, GameObject> OnInteractionChanged;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FixedUpdate()
    {

        if (GameStateManager.Instance == null)
        {
            return;
        }
        
        gameObject.transform.position = GameStateManager.Instance.TargetTransform.position;
        
        CheckCurrentInteractable();

        GetInteractablesInRadius();

        if (currentInteractable != null)
        {
            currentInteractable?.ShowInteractability(true);
            OnInteractionChanged?.Invoke(true, currentInteractable.Type, currentInteractable.gameObject);
        }
    }

    private void GetInteractablesInRadius()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        float closestDistance = Mathf.Infinity;
        List<Interactable> interactables = new List<Interactable>();
        
        foreach (Collider hit in hits)
        {
            Interactable item = hit.GetComponentInParent<Interactable>();
            if (item != null)
            {
                interactables.Add(item);
                float dist = Vector3.Distance(transform.position, item.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    currentInteractable = item;
                    
                }
            }
        }

        foreach (Interactable item in interactables)
        {
            item.ShowInteractability(item == currentInteractable);
        }
        
        
    }

    private void CheckCurrentInteractable()
    {
        if (currentInteractable == null)
        {
            OnInteractionChanged?.Invoke(false, null, null);
            return;
        }
        
        float dist = Vector3.Distance(transform.position, currentInteractable.transform.position);
        if (dist > interactionRadius)
        {
            currentInteractable?.ShowInteractability(false);
            OnInteractionChanged?.Invoke(false, null, null);
            currentInteractable = null;
        }
    }

    public void SetInteractionRadius(float radius)
    {
        interactionRadius = radius;
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
