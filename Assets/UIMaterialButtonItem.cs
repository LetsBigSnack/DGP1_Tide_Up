using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UIMaterialButtonItem : MonoBehaviour
{
    [SerializeField] private TrashMaterialType type;

    private void OnEnable()
    {
        if(UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            gameObject.GetComponentInChildren<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponentInChildren<Button>().interactable = true;
        }
    }
    public void OnClick()
    {
        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Recycler)
        {
            UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Upcycler);
        }

        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Upcycler)
        {
            TrashMaterialEntry currentData = InventoryManager.Instance.GetWallet().Where(t => t.TrashMaterialData.type == type).FirstOrDefault();
            if(currentData.Amount > 0)
            {
                UIUpcyclerController.Instance.AddMaterial(currentData.TrashMaterialData);
            }
        }

        SoundManager.Instance.PlaySFX("Click");
    }
}
