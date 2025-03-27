using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxTrashItems = 5;

    private List<InventoryItem> _trashItems = new();
    private List<TrashMaterialEntry> _materialWallet = new();
    [SerializeField] private List<TrashMaterialData> allMaterialTypes = new();
    [SerializeField] private List<TrashData> listOfDummyTrash = new();

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

        foreach (var material in allMaterialTypes)
        {
            if (!_materialWallet.Exists(e => e.trashMaterialData == material))
            {
                _materialWallet.Add(new TrashMaterialEntry(material, 0));
            }
        }
    }
    public void AddTrashItem()
    {
        ClearConsole();

        if (_trashItems.Count >= maxTrashItems)
        {
            Debug.Log("Inventory full! Can't pick up more trash.");
            return;
        }

        TrashData randomTrashData = listOfDummyTrash[Random.Range(0, listOfDummyTrash.Count)];

        _trashItems.Add(new InventoryItem(randomTrashData));
        Debug.Log("Picked up: " + randomTrashData.name);
    }

    public void RemoveTrashItem()
    {
        ClearConsole();

        if (_trashItems.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return;
        }

        InventoryItem itemToRemove = _trashItems[0];
        _trashItems.RemoveAt(0);

        Debug.Log($"Removed trash: {itemToRemove.trashData.name}");

        foreach (TrashMaterialData mat in itemToRemove.trashData.materials)
        {
            AddMaterial(mat, 1);
        }
    }

    public void AddMaterial(TrashMaterialData material, int amount)
    {
        TrashMaterialEntry entry = _materialWallet.Find(e => e.trashMaterialData == material);
        if (entry != null)
        {
            entry.amount += amount;
        }
        else
        {
            _materialWallet.Add(new TrashMaterialEntry(material, amount));
        }

        Debug.Log($"+{amount}x {material.type}");
    }

    public void PrintInventory()
    {
        ClearConsole();

        Debug.Log("==== INVENTORY ====");
        foreach (var item in _trashItems)
        {
            Debug.Log("Trash: " + item.trashData.name);
        }

        Debug.Log("--- Materials ---");
        foreach (var entry in _materialWallet)
        {
            Debug.Log($"{entry.trashMaterialData.type}: {entry.amount}");
        }
    }

    private void ClearConsole()
    {
#if UNITY_EDITOR
        // This calls Unity's internal "ClearConsole" menu command
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod?.Invoke(null, null);
#endif
    }
}
