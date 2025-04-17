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
    }
}
