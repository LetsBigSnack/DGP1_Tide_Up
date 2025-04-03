using UnityEngine;
using UnityEngine.InputSystem;

public class DevControlls : MonoBehaviour 
{
    private DEVInputs _devInputs;


    private void Awake()
    {
            _devInputs = new DEVInputs();
    }

    private void OnEnable()
    {
        _devInputs.Enable();
        _devInputs.DevInputs.RemoveRecycleItem.Enable();
        _devInputs.DevInputs.ConfirmRecycle.Enable();

        _devInputs.DevInputs.RemoveRecycleItem.performed += RemoveItemFromRecycler;
        _devInputs.DevInputs.ConfirmRecycle.performed += RecycleStoredItem;
        _devInputs.DevInputs.Upgrade.performed += UpgradeInventory;
    }

    private void OnDisable()
    {
        _devInputs.Disable();
        _devInputs.DevInputs.RemoveRecycleItem.Disable();
        _devInputs.DevInputs.ConfirmRecycle.Disable();

        _devInputs.DevInputs.RemoveRecycleItem.performed -= RemoveItemFromRecycler;
        _devInputs.DevInputs.ConfirmRecycle.performed -= RecycleStoredItem;
        _devInputs.DevInputs.Upgrade.performed -= UpgradeInventory;

    }

    private void RemoveItemFromRecycler(InputAction.CallbackContext context)
    {
        RecyclerManager.Instance.RemoveStoredItem();
    }
    private void RecycleStoredItem(InputAction.CallbackContext context)
    {
        RecyclerManager.Instance.RecycleStoredItem();
    }
    private void UpgradeInventory(InputAction.CallbackContext context)
    {
        UpgradeManager.Instance.Upgrade(UpgradeManager.Instance.GetUpgrade(0));
    }
}
