using System;
using Data;
using UnityEngine;

public class ItemInteractable : Interactable
{
    
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;
    public override InteractableType Type => InteractableType.Pickup;

    public override void Interact()
    {
        if (!InventoryManager.Instance.HasSpaceForItem())
        {
            //TODO: maybe play sound
            return;
        }
        
        MiniGameManager.Instance.StartMiniGame(MiniGameType.PickUp, success =>
        {
            TrashItemInstance trash = new TrashItemInstance(trashData, success);
            
            if (InventoryManager.Instance.AddItem(trash))
            {
                EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
                Destroy(gameObject);
            }
        });
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

    private void OnDrawGizmos()
    {
        if (highlight)
        {
            BoxCollider collider = GetComponentInChildren<BoxCollider>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
        
    }
}
