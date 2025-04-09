using System.Collections.Generic;
using Data;
using UnityEngine;

public class RecyclerManager : MonoBehaviour
{
    private ItemInstance _storedItem;

    public static RecyclerManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool StoreItemRecycler(ItemInstance item)
    {
        if (_storedItem != null)
        {
            return false;
        }

        _storedItem = item;
        return true;
    }

    public bool RemoveStoredItem()
    {
        if (_storedItem == null)
        {
            return false;
        }
        InventoryManager.Instance.AddItem(_storedItem);
        _storedItem = null;
        return true;
    }

    public void StoreItem()
    {
        if (_storedItem != null)
        {
            Debug.Log(_storedItem.ItemData.name + " is currently stored inside the recycler. Can't add a extra one");
            return;
        }

        ItemInstance item = InventoryManager.Instance.TestTrashItem();
        if (item == null) return;

        if (!InventoryManager.Instance.RemoveItem(item))
        {
            Debug.Log("Can not remove the item");
        }

        if (!StoreItemRecycler(item))
        {
            InventoryManager.Instance.AddItem(item);
            Debug.Log("Item could not be stored inside recycler");
        }

        Debug.Log("You have put " + item.ItemData.name + " into the recycler, press F to confirm");
        
    }

    public void RecycleStoredItem()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        if (_storedItem == null) {
            Debug.Log("No item stored to be recycled. Press R to add item to recycler");
            return;
        }

        Debug.Log("You recycled " + _storedItem);

        foreach (TrashMaterialData mat in _storedItem.GetMaterials())
        {
            InventoryManager.Instance.AddMaterial(mat.type, 1);
        }
        _storedItem = null;
    }

}
