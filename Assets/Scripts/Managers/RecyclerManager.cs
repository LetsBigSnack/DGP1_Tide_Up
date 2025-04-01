using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class RecyclerManager : MonoBehaviour
{
    [SerializeField] private List<Recycler> recyclers;

    private DEVInputs _devInputs; 

    public static RecyclerManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _devInputs = new DEVInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        _devInputs.Enable();

        _devInputs.DevInputs.StoreRecycleItem.performed += StoreItem;
        _devInputs.DevInputs.ConfirmRecycle.performed += RecycleStoredItem;

    }

    private void OnDisable()
    {
        _devInputs.Disable();

        _devInputs.DevInputs.StoreRecycleItem.performed -= StoreItem;
        _devInputs.DevInputs.ConfirmRecycle.performed -= RecycleStoredItem;

    }

    private void StoreItem(InputAction.CallbackContext context)
    {
        foreach (var recycler in recyclers)
        {
            if (!recycler.PlayerInRange) continue;
            if (recycler.HasStoredItem)
            {
                Debug.Log(recycler.StoredItem.name + " is currently stored inside the recycler. Can't add a extra one");
                continue;
            }

            ItemData item = InventoryManager.Instance.TestTrashItem();
            if (item == null) return;

            InventoryManager.Instance.TryRemoveItem(item);

            bool stored = recycler.StoreItem(item);
            if (!stored)
            {
                InventoryManager.Instance.TryAddItem(item);
                Debug.Log("Item could not be stored inside recycler");
            }

            Debug.Log("You have put " + item.name + " into the recycler, press F to confirm");
        }
    }

    private void RecycleStoredItem(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        foreach (var recycler in recyclers)
        {
            if (!recycler.HasStoredItem) {
                Debug.Log("No item stored to be recycled. Press R to add item to recycler");
                continue;
            }

            ItemData storedItem = recycler.StoredItem;
            recycler.StoredItem = null;

            Debug.Log("You recycled " + storedItem);

            foreach (TrashMaterialData mat in storedItem.materials)
            {
                InventoryManager.Instance.AddMaterial(mat, 1);
            }
        }
    }

}
