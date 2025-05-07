using System;
using Data;
using UnityEngine;

public class ItemInteractable : Interactable
{
    
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;
    [SerializeField] private bool isInteractable = true;

    public bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public override InteractableType Type => InteractableType.Pickup;

    public override void Interact()
    {
        if (isInteractable)
        {
            if (InventoryManager.Instance.AddItem(new TrashItemInstance(trashData, false)))
            {
                EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
                Destroy(gameObject);

                GameObject newToast = UI_ToastManager.Instance.CreateToast(UI_ToastManager.Instance.ItemToastPrefab, UI_ToastManager.Instance.ItemToastParent);
                newToast.GetComponent<ToastNotificationItem>().SetToast(titleText: trashData.title, sprite: trashData.sprite);
            }
        }
    }

    public override void ShowInteractability(bool show)
    {
        if (isInteractable)
        {
            highlight = show;
        }
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
