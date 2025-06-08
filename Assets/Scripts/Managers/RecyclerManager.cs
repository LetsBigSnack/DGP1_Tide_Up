using System;
using System.Collections.Generic;
using System.Linq;
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

        List<TrashMaterialData> paper = new();
        List<TrashMaterialData> wood = new();
        List<TrashMaterialData> metal = new();
        List<TrashMaterialData> glass = new();
        List<TrashMaterialData> plastic = new();

        foreach (ItemInstance item in storedItems)
        {
            foreach (TrashMaterialData mat in item.GetMaterials())
            {
                switch (mat.type)
                {
                    case TrashMaterialType.Glass:
                        glass.Add(mat);
                        break;
                    case TrashMaterialType.Metal:
                        metal.Add(mat);
                        break;
                    case TrashMaterialType.Paper:
                        paper.Add(mat);
                        break;
                    case TrashMaterialType.Plastic:
                        plastic.Add(mat);
                        break;
                    case TrashMaterialType.Wood:
                        wood.Add(mat);
                        break;
                }                
            }
        }

        if(glass.Count > 0)
        {
            InventoryManager.Instance.AddMaterial(TrashMaterialType.Glass, glass.Count);
        }

        if (metal.Count > 0)
        {
            InventoryManager.Instance.AddMaterial(TrashMaterialType.Metal, metal.Count);
        }

        if (paper.Count > 0)
        {
            InventoryManager.Instance.AddMaterial(TrashMaterialType.Paper, paper.Count);
        }

        if (plastic.Count > 0)
        {
            InventoryManager.Instance.AddMaterial(TrashMaterialType.Plastic, plastic.Count);
        }

        if (wood.Count > 0)
        {
            InventoryManager.Instance.AddMaterial(TrashMaterialType.Wood, wood.Count);
        }

        storedItems = null;
        OnStoredItemCleared?.Invoke(true);
        SoundManager.Instance.PlaySFX("Recycle");
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
