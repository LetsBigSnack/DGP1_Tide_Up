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
    

    [Header("ItemPrefab")]
    [SerializeField] private GameObject itemPrefab;

    private List<UIInventoryItem> _currentItems = new List<UIInventoryItem>();
    private List<GameObject> _emptyItems = new List<GameObject>();

    private UIInventoryItem _currentSelectedItem;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        CreateInventoryObjects();
        _currentSelectedItem = null;
        UpdateInventory(InventoryManager.Instance.Items);
        InventoryManager.OnInventoryChanged += UpdateInventory;
    }

    private void CreateInventoryObjects()
    {
        _currentItems = new List<UIInventoryItem>();
        for (int i = 0; i < InventoryManager.Instance?.MaxItems; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.name = $"Item {i}";
            UIInventoryItem uiInventoryItem = newItem.GetComponent<UIInventoryItem>();
            uiInventoryItem.Setup();
            _currentItems.Add(uiInventoryItem);
        }
    }

    private void OnDisable()
    {
        ClearInventoryObjects();
        InventoryManager.OnInventoryChanged -= UpdateInventory;
    }

    private void ClearInventoryObjects()
    {
        for (int i = 0; i < _currentItems.Count; i++)
        {
            Destroy(_currentItems[i].gameObject);
            _currentItems[i] = null;
        }
        _currentItems.Clear();
    }
    

    private void UpdateInventory(List<ItemInstance> items)
    {
        if(items == null)
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].ItemData.title.Contains("Tutorial"))
            {
                _currentItems[i].Setup(items[i], false);

                if(i == 0 && InputDeviceHelper.Instance.IsController())
                {
                    UIEventSystemHelper.Instance.SetFirstSelectedItem(_currentItems[i].gameObject);
                }
            }
        }

        for (int i = items.Count; i < _currentItems.Count; i++)
        {
            _currentItems[i].Setup();
        }
    }
    public void SetGameObjectAsSelected(UIInventoryItem item)
    {
        if(item == null)
        {
            return;
        }

        if (item.IsEmpty())
        {
            return;
        }

        if (_currentSelectedItem == null)
        {
            _currentSelectedItem = item;
            item.ToggleIcon(true);
            return;
        }

        _currentSelectedItem.ToggleIcon(false);
        _currentSelectedItem = item;
        _currentSelectedItem.ToggleIcon(true);
    }
}
