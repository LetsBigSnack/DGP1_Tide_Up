using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxItems = 5;
    [SerializeField] private int maxTrashMaterials = 20;

    [SerializeField] List<ItemInstance> _items = new();
    [SerializeField] private List<TrashMaterialEntry> _materialWallet = new();
    //TODO Implement Dictionary Wallet
    private Dictionary<TrashMaterialType, int> _materials;

    public static InventoryManager Instance;

    public List<ItemInstance> Items
    {
        get => _items;
        set => _items = value;
    }
    public int MaxItems
    {
        get => maxItems;
        set => maxItems = value;
    }

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
        ItemInstance randomTrashData = DataUtil.Instance.GetRandomTrash();
        AddItem(randomTrashData);
    }

    public bool AddItem(ItemInstance item)
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
        Debug.Log("Picked up: " + item.ItemData.name);
        return true;
    }

    public ItemInstance TestTrashItem()
    {
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return null;
        }

        ItemInstance itemToRemove = _items[0];
        return itemToRemove;
    }

    public bool RemoveItem(ItemInstance item)
    {
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return false;
        }

        _items.Remove(item);
        Debug.Log($"Removed trash: {item.ItemData.name}");

        return true;
    }

    public void AddMaterial(TrashMaterialType materialType, int amount)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData.type == materialType);
        if (entry != null)
        {
            if (entry.Amount + amount > maxTrashMaterials)
            {
                Debug.LogWarning($"Can't add more {materialType}, limit reached.");
                return;
            }

            entry.Amount += amount;
        }
        else
        {
            TrashMaterialData newTrashMaterialData = DataUtil.Instance.GetMaterialByType(materialType);
            _materialWallet.Add(new TrashMaterialEntry(newTrashMaterialData, amount));
        }

        Debug.Log($"+ {amount}x {materialType}");
    }

    public bool RemoveMaterial(TrashMaterialType materialType, int amount)
    {
        
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData.type == materialType);

        if (entry == null)
        {
            Debug.LogWarning($"Material {materialType} not found in inventory.");
            return false;
        }

        if (entry.Amount - amount < 0)
        {
            Debug.LogWarning($"Can't remove {materialType}, not enough trash.");
            return false;
        }

        entry.Amount = Mathf.Max(0, entry.Amount - amount);
        Debug.Log($"Removed {amount}x {materialType}");
        return true;
    }

    public void PrintInventory()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        Debug.Log("==== INVENTORY (" + _items.Count + "/" + maxItems + ") ====");
        foreach (var item in _items)
        {
            Debug.Log("Trash: " + item.ItemData.name);
        }

        Debug.Log("--- Materials ---");
        foreach (var entry in _materialWallet)
        {
            Debug.Log(entry.TrashMaterialData.type + ": " + entry.Amount + " / " + maxTrashMaterials);
        }
    }

    public int GetMaterialAmount(TrashMaterialType materialType)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData.type == materialType);
        return entry != null ? entry.Amount : 0;
    }

    public bool HasSpaceForMaterial(TrashMaterialType materialType, int amount)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData.type == materialType);
        return entry.Amount + amount > maxTrashMaterials;
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

    public bool HasItem(ItemInstance questItem)
    {
        return _items.Contains(questItem);
    }
}
