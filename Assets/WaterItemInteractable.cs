using Data;
using UnityEngine;

public class WaterItemInteractable : Interactable
{
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;
    public override InteractableType Type => InteractableType.WaterPickup;

    public override void Interact()
    {
        if (!InventoryManager.Instance.HasSpaceForItem())
        {
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Not enough space free in your inventory to collect all items");
            return;
        }

        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            MiniGameManager.Instance.StartMiniGame(MiniGameType.Boat, success =>
            {
                TrashItemInstance trash = new TrashItemInstance(trashData, success);
            
                if (InventoryManager.Instance.AddItem(trash))
                {
                    if (EnvironmentManager.Instance != null)
                    {
                        EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
                    }
                    Destroy(gameObject);
                }
            });
        }
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

}
