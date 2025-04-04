using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class TideUpBoxManager : MonoBehaviour
{
    [SerializeField] private List<TrashData> possibleTrash;
    private List<TrashData> _boxInventory;
    [SerializeField] private int awarenessScore = 1;
    [SerializeField] private int maxTotalTrash = 10;

    [SerializeField] private float timeToAddItems = 6f;
    [SerializeField] private float resetDayTime = 0.1f;

    private bool _hasAddedItemsToday = false;

    public static TideUpBoxManager Instance;

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

    private void OnEnable()
    {
        TimeManager.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(float currentTime)
    {
        if (currentTime >= timeToAddItems && !_hasAddedItemsToday)
        {
            AddDailyItems();
            _hasAddedItemsToday = true;
        }

        if (currentTime < resetDayTime && _hasAddedItemsToday)
        {
            _hasAddedItemsToday = false;
        }
    }

    public void AddDailyItems()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        int spaceLeft = maxTotalTrash - _boxInventory.Count;
        if (spaceLeft <= 0)
        {
            Debug.Log("Tide-Up-Box is full. No materials could be added.");
            return;
        }

        int maxAddableToday = Mathf.Min(spaceLeft, awarenessScore * 3);

        for (int i = 0; i < maxAddableToday; i++)
        {
            TrashData randomTrash = possibleTrash[Random.Range(0, possibleTrash.Count)];

            _boxInventory.Add(randomTrash);
            Debug.Log("Added " + randomTrash + " to the Tide-Up-Box");
        }

        Debug.Log("Tide-Up-Box currently has: " + _boxInventory.Count + " items");
    }

    public void CollectAllItems()
    {
        List<TrashData> itemsToKeep = new();

        foreach (TrashData trash in _boxInventory)
        {
            if (!InventoryManager.Instance.AddItem(trash))
            {
                itemsToKeep.Add(trash);
            }
        }
        
        int itemsCollected = _boxInventory.Count - itemsToKeep.Count;
        _boxInventory = itemsToKeep;

        Debug.Log("Collected " + itemsCollected + " items. " + itemsToKeep.Count + " left in the Tide-Up-Box.");
    }

    public void CollectOneItem()
    {
        TrashData itemToCollect = TestGetRandomTrashFromBox();

        if (!InventoryManager.Instance.AddItem(itemToCollect))
        {
            return;
        }

        _boxInventory.Remove(itemToCollect);
    }

    private TrashData TestGetRandomTrashFromBox()
    {
        if (_boxInventory.Count == 0)
        {
            Debug.Log("No trash to remove.");
            return null;
        }

        TrashData itemToCollect = _boxInventory[Random.Range(0, _boxInventory.Count)];
        return itemToCollect;
    }

}
