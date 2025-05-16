using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class RecyclerManager : MonoBehaviour
{
    public static RecyclerManager Instance;
    [Header("maxItemStorage")]
    [SerializeField] private int maxStorage;

    [Header("currentItems")]
    [SerializeField] private List<ItemInstance> storedItems;
    public static event Action<ItemInstance, bool> OnStoredItemsChanged;
    public static event Action<bool> OnStoredItemCleared;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItemToRecycler(ItemInstance item)
    {
        if(storedItems == null)
        {
            storedItems = new List<ItemInstance>();
        }

        if (storedItems.Count >= maxStorage)
        {
            return false;
        }

        InventoryManager.Instance.RemoveItem(item);
        storedItems.Add(item);
        OnStoredItemsChanged?.Invoke(item, false);
        return true;
    }

    public bool RemoveItemFromRecycler(ItemInstance item)
    {
        if (!storedItems.Contains(item))
        {
            SoundManager.Instance.PlaySFX("Error");
            return false;
        }
        
        storedItems.Remove(item);
        InventoryManager.Instance.AddItem(item);
        OnStoredItemsChanged?.Invoke(item, true);
        return true;
    }

    public void RecycleItems()
    {
        if((storedItems == null || storedItems.Count <= 0))
        {
            SoundManager.Instance.PlaySFX("Error");
            return;
        }

        foreach (ItemInstance item in storedItems)
        {
            foreach(TrashMaterialData mat in item.GetMaterials())
            {
                InventoryManager.Instance.AddMaterial(mat.type, 1);
            }
        }
        storedItems = null;
        OnStoredItemCleared?.Invoke(true);
    }

    public void RemoveAllItems()
    {
        if (storedItems == null || storedItems.Count <= 0)
        {
            return;
        }

        for (int i = storedItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = storedItems[i];
            storedItems.RemoveAt(i);
            InventoryManager.Instance.AddItem(item);
        }
        OnStoredItemCleared?.Invoke(true);
    }

    public void RemoveAllItemsBtn()
    {
        if (storedItems == null || storedItems.Count <= 0)
        {
            SoundManager.Instance.PlaySFX("Error");
            return;
        }

        for (int i = storedItems.Count - 1; i >= 0; i--)
        {
            ItemInstance item = storedItems[i];
            storedItems.RemoveAt(i);
            InventoryManager.Instance.AddItem(item);
        }
        OnStoredItemCleared?.Invoke(true);
    }

}
