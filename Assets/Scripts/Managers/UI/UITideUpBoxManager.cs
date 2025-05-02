using Data;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UITideUpBoxManager : MonoBehaviour
{
    [SerializeField] private GameObject tideUpBoxView;
    [SerializeField] private Transform tideUpBoxInventory;

    [SerializeField] private GameObject itemSlotPrefab;    
    [SerializeField] private GameObject emptySlotPrefab;

    private bool _isOpen;
    private TideUpBox _currBox;

    public bool IsOpen
    {
        get { return _isOpen; }
        set { _isOpen = value; }
    }

    public static UITideUpBoxManager Instance;
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

    public void OpenTideUpBox(TideUpBox box)
    {
        tideUpBoxView.SetActive(true);
        _isOpen = true;

        _currBox = box;
        UpdateTideUpBox();
        UIJournalManager.Instance.SwitchState(JournalType.Inventory);
    }

    public void UpdateTideUpBox()
    {
        ClearGrid();

        List<TrashItemInstance> items = _currBox.BoxInventory;

        foreach (TrashItemInstance item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, tideUpBoxInventory);
            slot.GetComponent<UITideUpBoxItem>().Setup(item, _currBox);
        }

        int emptySlots = _currBox.MaxTotalTrash - items.Count;
        for (int i = 0; i < emptySlots; i++)
        {
            Instantiate(emptySlotPrefab, tideUpBoxInventory);
        }
    }

    public void CollectAll()
    {
        _currBox.CollectAllItems();
        UpdateTideUpBox();
    }

    public void CloseTideUpBox()
    {
        tideUpBoxView.SetActive(false);
        _isOpen = false;
    }

    private void ClearGrid()
    {
        foreach (Transform child in tideUpBoxInventory)
        {
            Destroy(child.gameObject);
        }
    }
}
