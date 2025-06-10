using Data;
using UnityEngine;

public class WaterItemInteractable : Interactable
{
    [SerializeField] private bool highlight;
    [SerializeField] private int itemAmount;
    public override InteractableType Type => InteractableType.WaterPickup;

    public override void Interact()
    {
        if (!InventoryManager.Instance.HasSpaceForItem(1))
        {
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Not enough space free in your inventory to collect all items");
            return;
        }

        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            MiniGameManager.Instance.StartMiniGame(MiniGameType.Boat, success =>
            {

                for (int i = 0; i < itemAmount; i++)
                {
                    if (InventoryManager.Instance.HasSpaceForItem(1))
                    {
                        TrashData trashData = DataUtil.Instance.GetRandomTrashData();
                        TrashItemInstance trash = new TrashItemInstance(trashData, success);
                        InventoryManager.Instance.AddItem(trash);
                    }
                }
                
                if (EnvironmentManager.Instance != null)
                {
                    EnvironmentManager.Instance?.AddCleanlinessScore(EnvironmentActionType.PickUp);
                }
                Destroy(gameObject);
               
            });
        }
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

}
