using System;
using Data;
using UnityEngine;

public class DiggingSpot : Interactable
{
    
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;

    public override InteractableType Type => InteractableType.Digging;

    public override void Interact()
    {
        if (!InventoryManager.Instance.HasSpaceForItem())
        {
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Not enough space free in your inventory to collect all items");
            return;
        }
        
        MiniGameManager.Instance.StartMiniGame(MiniGameType.Digging, success =>
        {
            TrashItemInstance trash = new TrashItemInstance(trashData, success);
            
            if (InventoryManager.Instance.AddItem(trash))
            {
                EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
                Destroy(gameObject);
                
            }
        });
    }

    public void Awake()
    {
        trashData = DataUtil.Instance.GetRandomTrashData();
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
