using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxItems = 5;
    [SerializeField] private int maxTrashMaterials = 20;

    [SerializeField] List<ItemInstance> _items = new();
    [SerializeField] private List<TrashMaterialEntry> _materialWallet = new();

    public static event Action<List<ItemInstance>> OnInventoryChanged;
    public static event Action<List<TrashMaterialEntry>> OnTrashMaterialChanged;
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

    public List<TrashMaterialEntry> GetWallet()
    {
        return _materialWallet;
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
            //TODO: Add error sound
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Your inventory is full!");
            
            Debug.Log("Inventory full! Can't pick up more trash.");
            return false;
        }

        _items.Add(item);
        OnInventoryChanged?.Invoke(_items);

        //TODO: Add sound
        UI_ToastManager.Instance?.SpawnToastMessage(ToastType.Item, item.ItemData.title, sprite: item.ItemData.sprite);
        
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
        Debug.Log("Removing: " + item);
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return false;
        }
        
        ItemInstance itemToRemove = _items.Find(x => x.ItemData == item.ItemData);

        if (itemToRemove == null)
        {
            return false;
        }
        
        _items.Remove(itemToRemove);
        OnInventoryChanged?.Invoke(_items);
        Debug.Log($"Removed trash: {item.ItemData.name}");

        return true;
    }

    public bool IsItemInInventory(ItemInstance item, int amount)
    {
        int amountInInventory = 0;

        foreach(ItemInstance i in _items)
        {
            if(i.ItemData == item.ItemData)
            {
                amountInInventory += 1;
            }
        }
        return amountInInventory >= amount;
    }

    public bool AddMaterial(TrashMaterialType materialType, int amount)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.TrashMaterialData.type == materialType);
        if (entry != null)
        {
            entry.Amount += amount;
        }
        else
        {
            TrashMaterialData newTrashMaterialData = DataUtil.Instance.GetMaterialByType(materialType);
            _materialWallet.Add(new TrashMaterialEntry(newTrashMaterialData, amount));
        }

        OnTrashMaterialChanged?.Invoke(_materialWallet);
        Debug.Log($"+ {amount}x {materialType}");

        //TODO: Add sound
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Item, entry.TrashMaterialData.name, sprite: entry.TrashMaterialData.sprite);
        return true;
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
        OnTrashMaterialChanged?.Invoke(_materialWallet);
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
        return entry.Amount + amount <= maxTrashMaterials;
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
        return _items.Any(item => item.ItemData == questItem.ItemData);
    }

    public bool HasSpaceForItem()
    {
        return _items.Count+1 <= maxItems;
    }
    
}
