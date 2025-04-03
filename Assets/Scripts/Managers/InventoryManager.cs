using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxItems = 5;
    [SerializeField] private int maxTrashMaterials = 20;

    private List<ItemData> _items = new();
    private List<TrashMaterialEntry> _materialWallet = new();
    
    public static InventoryManager Instance;

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

    private void Start()
    {
        foreach (var material in DataUtil.Instance.GetMaterials())
        {
            if (!_materialWallet.Exists(e => e.TrashMaterialData == material))
            {
                _materialWallet.Add(new TrashMaterialEntry(material, 0));
            }
        }
    }

    public void TestAddTrashItem()
    {
        TrashData randomTrashData = DataUtil.Instance.GetRandomTrash();
        AddItem(randomTrashData);
    }

    public bool AddItem(ItemData item)
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        if (_items.Count >= maxItems)
        {
            Debug.Log("Inventory full! Can't pick up more trash.");
            return false;
        }

        _items.Add(item);
        Debug.Log("Picked up: " + item.name);
        return true;
    }

    public ItemData TestTrashItem()
    {
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return null;
        }

        ItemData itemToRemove = _items[0];
        return itemToRemove;
    }

    public bool RemoveItem(ItemData item)
    {
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return false;
        }

        _items.Remove(item);
        Debug.Log($"Removed trash: {item.name}");

        return true;
    }

    public void AddMaterial(TrashMaterialData material, int amount)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData == material);
        if (entry != null)
        {
            if (entry.Amount + amount > maxTrashMaterials)
            {
                Debug.LogWarning($"Can't add more {material.type}, limit reached.");
                return;
            }

            entry.Amount += amount;
        }
        else
        {
            _materialWallet.Add(new TrashMaterialEntry(material, amount));
        }

        Debug.Log($"+ {amount}x {material.type}");
    }

    public void RemoveMaterial(TrashMaterialData material, int amount)
    {
        
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData == material);

        if (entry == null)
        {
            Debug.LogWarning($"Material {material.type} not found in inventory.");
            return;
        }

        if (entry.Amount - amount < 0)
        {
            Debug.LogWarning($"Can't remove {material.type}, not enough trash.");
            return;
        }

        entry.Amount = Mathf.Max(0, entry.Amount - amount);
        Debug.Log($"Removed {amount}x {material.type}");
    }

    public void PrintInventory()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        Debug.Log("==== INVENTORY (" + _items.Count + "/" + maxItems + ") ====");
        foreach (var item in _items)
        {
            Debug.Log("Trash: " + item.name);
        }

        Debug.Log("--- Materials ---");
        foreach (var entry in _materialWallet)
        {
            Debug.Log(entry.TrashMaterialData.type + ": " + entry.Amount + " / " + maxTrashMaterials);
        }
    }

    public int GetMaterialAmount(TrashMaterialData material)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData == material);
        return entry != null ? entry.Amount : 0;
    }
    public void IncreaseMaxItems(int amount)
    {
        maxItems += amount;
        Debug.Log($"Max trash item slots increased to {maxItems}");
    }
    
    public void IncreaseMaxMaterials(int amount)
    {
        maxTrashMaterials += amount;
        Debug.Log($"Max trash materials slots increased to {maxTrashMaterials}");
    }

}
