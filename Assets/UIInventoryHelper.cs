using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Data;

public enum FilterType
{

}
public class UIInventoryHelper : MonoBehaviour
{
    public static UIInventoryHelper Instance;

    [Header("Filter")]
    [SerializeField] private TMP_Dropdown dropdown;

    [Header("ScrollViewContent")]
    [SerializeField] private Transform itemParent;

    [Header("CurrentItemsInInventory")]
    [SerializeField] private List<GameObject> itemObjects;

    [Header("ItemPrefab")]
    [SerializeField] private GameObject itemPrefab;

    private List<GameObject> _currentItems = new List<GameObject>();

    private void OnEnable()
    {
        UpdateInventory(InventoryManager.Instance.Items);
        InventoryManager.OnInventoryChanged += UpdateInventory;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= UpdateInventory;
    }

    private void UpdateInventory(List<ItemInstance> items)
    {
        if(items == null)
        {
            return;
        }

        ClearInventory();
        foreach(ItemInstance item in items)
        {
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.GetComponent<UIInventoryItem>().Setup(item);
            _currentItems.Add(newItem);
        }
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

}
