using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxTrashItems = 5;
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

    private void AddItem(ItemData item)
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        if (_items.Count >= maxTrashItems)
        {
            Debug.Log("Inventory full! Can't pick up more trash.");
            return;
        }

        _items.Add(item);
        Debug.Log("Picked up: " + item.name);
    }

    public void TestRemoveTrashItem()
    {
        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return;
        }

        ItemData itemToRemove = _items[0];
        RemoveItem(itemToRemove);
    }

    private void RemoveItem(ItemData item)
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif

        if (_items.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return;
        }

        _items.Remove(item);
        
        
        //TODO: remove after testing
        Debug.Log($"Removed trash: {item.name}");

        foreach (TrashMaterialData mat in item.materials)
        {
            AddMaterial(mat, 1);
        }
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

        Debug.Log($"+{amount}x {material.type}");
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

        Debug.Log("==== INVENTORY ====");
        foreach (var item in _items)
        {
            Debug.Log("Trash: " + item.name);
        }

        Debug.Log("--- Materials ---");
        foreach (var entry in _materialWallet)
        {
            Debug.Log($"{entry.TrashMaterialData.type}: {entry.Amount}");
        }
    }

}
