using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Data;
using System.Linq;

public enum SortingType
{
    All,
    Glass,
    Metal,
    Paper,
    Plastic,
    Wood
}
public class UIInventoryHelper : MonoBehaviour
{
    public static UIInventoryHelper Instance;

    [Header("Filter")]
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private SortingType currentSortingType;

    [Header("ScrollViewContent")]
    [SerializeField] private Transform itemParent;

    [Header("CurrentItemsInInventory")]
    [SerializeField] private List<GameObject> itemObjects;

    [Header("ItemPrefab")]
    [SerializeField] private GameObject itemPrefab;

    private List<GameObject> _currentItems = new List<GameObject>();

    private Dictionary<int, SortingType> _dropdownOptions = new Dictionary<int, SortingType>()
    {
        { 0,SortingType.All },
        { 1,SortingType.Glass },
        { 2,SortingType.Metal },
        { 3,SortingType.Paper },
        { 4,SortingType.Plastic },
        { 5,SortingType.Wood }
    };

    private Dictionary<SortingType, TrashMaterialType> __materialTypes = new Dictionary<SortingType, TrashMaterialType>()
    {
        {SortingType.Glass, TrashMaterialType.Glass},
        {SortingType.Metal, TrashMaterialType.Metal},
        {SortingType.Paper, TrashMaterialType.Paper},
        {SortingType.Plastic, TrashMaterialType.Plastic},
        {SortingType.Wood, TrashMaterialType.Wood},

    };

    private void Awake()
    {
        FillSortingOptions();
    }

    private void OnEnable()
    {
        UpdateInventory(InventoryManager.Instance.Items);
        InventoryManager.OnInventoryChanged += UpdateInventory;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= UpdateInventory;
    }

    public void SelectSortingType()
    {
        currentSortingType = _dropdownOptions[dropdown.value];
        UpdateInventory(InventoryManager.Instance.Items);
    }

    private void UpdateInventory(List<ItemInstance> items)
    {
        if(items == null)
        {
            return;
        }

        ClearInventory();
        items = SortInventory(items);
        foreach(ItemInstance item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.GetComponent<UIInventoryItem>().Setup(item);
            _currentItems.Add(newItem);
        }
    }
    private List<ItemInstance> SortInventory(List<ItemInstance> items)
    {
        List<ItemInstance> sortedItems;

        if (currentSortingType == SortingType.All)
        {
            return sortedItems = items;
        }

        return sortedItems = items
            .OrderByDescending(item => item.GetMaterials()
            .Any(m => m.type == __materialTypes[currentSortingType]))
            .ToList();
    }

    private void ClearInventory()
    {
        if(_currentItems.Count <= 0)
        {
            return;
        }

        foreach(GameObject item in _currentItems)
        {
            Destroy(item);
        }
        _currentItems.Clear();
    }

    private void FillSortingOptions()
    {
        List<string> dropdownOptions = new List<string>();
        foreach(int i in _dropdownOptions.Keys)
        {
            dropdownOptions.Add(_dropdownOptions[i].ToString());
        }
        dropdown.AddOptions(dropdownOptions);
    }
}
