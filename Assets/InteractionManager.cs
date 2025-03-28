using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 4.0f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Interactable currentInteractable;
    
    public void FixedUpdate()
    {
        if (currentInteractable != null)
        {
            float dist = Vector3.Distance(transform.position, currentInteractable.transform.position);
            if (dist > interactionRadius)
            {
                currentInteractable?.ShowInteractability(false);
                currentInteractable = null;
            }
        }
        
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        
        List<Interactable> interactables = new List<Interactable>();
        float closestDistance = Mathf.Infinity;
        
        

        foreach (Collider hit in hits)
        {
            Debug.Log("Colliders");
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
        
        foreach (Interactable interactable in interactables)
        {
            interactable.ShowInteractability(false);
        }
        
        currentInteractable?.ShowInteractability(true);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(204.0f/255.0f,85.0f/255.0f,0,1);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
