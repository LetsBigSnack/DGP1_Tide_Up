using System;
using Data;
using UnityEngine;

public class FishingSpot : Interactable
{
    
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;
    [SerializeField] private float elapsedTime = 0.0f;
    [SerializeField] private float fishingCooldown = 10f;
    [SerializeField] private bool canFish = true;
    public override InteractableType Type => InteractableType.Fishing;

    public override void Interact()
    {
        if (!InventoryManager.Instance.HasSpaceForItem() || !canFish)
        {
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Not enough space free in your inventory to collect all items");
            return;
        }
        
        MiniGameManager.Instance.StartMiniGame(MiniGameType.Fishing, success =>
        {
            trashData = DataUtil.Instance.GetRandomTrashData();
            TrashItemInstance trash = new TrashItemInstance(trashData, success);
            
            if (InventoryManager.Instance.AddItem(trash))
            {
                canFish = false;
                elapsedTime = 0.0f;
                EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
            }
        });
    }

    private void FixedUpdate()
    {
        if (elapsedTime >= fishingCooldown)
        {
            canFish = true;
        }

        if (!canFish)
        {
            elapsedTime += Time.fixedDeltaTime;
        }
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }
    
}