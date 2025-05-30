using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UIMaterialButtonItem : MonoBehaviour
{
    [SerializeField] private TrashMaterialType type;
    [SerializeField] private GameObject circle;
    private Button _button;

    private void OnEnable()
    {
        _button = gameObject.GetComponentInChildren<Button>();

        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            Navigation nav = _button.navigation;
            nav.mode = Navigation.Mode.None;
            _button.navigation = nav;
        }
        else
        {
            gameObject.GetComponentInChildren<Button>().interactable = true;
            Navigation nav = _button.navigation;
            nav.mode = Navigation.Mode.Automatic;
            _button.navigation = nav;
        }

        circle.SetActive(false);
    }
    public void OnClick()
    {
        if (UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Recycler)
        {
            UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Upcycler);
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Switched to upcycler!");
            return;
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
