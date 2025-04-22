using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Data;
using System.Linq;

public enum FilterType
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
    [SerializeField] private FilterType currentSortingType;

    [Header("ScrollViewContent")]
    [SerializeField] private Transform itemParent;

    [Header("CurrentItemsInInventory")]
    [SerializeField] private List<GameObject> itemObjects;

    [Header("ItemPrefab")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject emptyItemPrefab;

    private List<GameObject> _currentItems = new List<GameObject>();
    private List<GameObject> _emptyItems = new List<GameObject>();

    private Dictionary<int, FilterType> _dropdownOptions = new Dictionary<int, FilterType>()
    {
        { 0,FilterType.All },
        { 1,FilterType.Glass },
        { 2,FilterType.Metal },
        { 3,FilterType.Paper },
        { 4,FilterType.Plastic },
        { 5,FilterType.Wood }
    };

    private Dictionary<FilterType, TrashMaterialType> __materialTypes = new Dictionary<FilterType, TrashMaterialType>()
    {
        {FilterType.Glass, TrashMaterialType.Glass},
        {FilterType.Metal, TrashMaterialType.Metal},
        {FilterType.Paper, TrashMaterialType.Paper},
        {FilterType.Plastic, TrashMaterialType.Plastic},
        {FilterType.Wood, TrashMaterialType.Wood},

    };

    private void Awake()
    {
        FillFilterOptions();
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

        int maxSpaces = InventoryManager.Instance.MaxItems;
        
        
        
        
        ClearInventory();
        items = FilterInventoy(items);


        for (int i = 0; i < items.Count; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.GetComponent<UIInventoryItem>().Setup(items[i]);
            _currentItems.Add(newItem);
        }
        
        for (int i = items.Count; i < maxSpaces; i++)
        {
            GameObject newItem = Instantiate(emptyItemPrefab, itemParent);
            _emptyItems.Add(newItem);
        }
        
       
    }
    private List<ItemInstance> FilterInventoy(List<ItemInstance> items)
    {
        List<ItemInstance> sortedItems;

        if (currentSortingType == FilterType.All)
        {
            return sortedItems = items.OrderBy(m => m.ItemQuality)
            .ToList();
        }

        return sortedItems = items
            .Where(item => item.GetMaterials()
            .Any(m => m.type == __materialTypes[currentSortingType]))
            .OrderBy(m => m.ItemQuality)
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
        
        foreach(GameObject item in _emptyItems)
        {
            Destroy(item);
        }
        _emptyItems.Clear();
    }

    private void FillFilterOptions()
    {
        List<string> dropdownOptions = new List<string>();
        foreach(int i in _dropdownOptions.Keys)
        {
            dropdownOptions.Add(_dropdownOptions[i].ToString());
        }
        dropdown.AddOptions(dropdownOptions);
    }
}
