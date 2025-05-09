using UnityEngine;
using System.Linq;

public class UIMaterialButtonItem : MonoBehaviour
{
    [SerializeField] private TrashMaterialType type;
    public void OnClick()
    {
        if(UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Upcycler)
        {
            TrashMaterialEntry currentData = InventoryManager.Instance.GetWallet().Where(t => t.TrashMaterialData.type == type).FirstOrDefault();
            if(currentData.Amount > 0)
            {
                UIUpcyclerController.Instance.AddMaterial(currentData.TrashMaterialData);
            }
        }
        else
        {
            //TODO: Add error sound
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Try doing that in the Upcycler in the other tab");
        }
    }
}
