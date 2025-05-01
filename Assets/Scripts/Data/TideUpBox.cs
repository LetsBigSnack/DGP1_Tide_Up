using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class TideUpBox : Interactable
{
    private List<TrashItemInstance> _boxInventory = new();
    [SerializeField] private int maxTotalTrash = 12;

    [SerializeField] private int awarenessBoxMultiplier = 3;

    private bool _isInteractable = false;

    public List<TrashItemInstance> BoxInventory
    {
        get => _boxInventory;
        set => _boxInventory = value;
    }
    public int MaxTotalTrash
    {
        get => maxTotalTrash;
        set => maxTotalTrash = value;
    }
    public int AwarenessBoxMultiplier
    {
        get => awarenessBoxMultiplier;
        set => awarenessBoxMultiplier = value;
    }


    public override InteractableType Type => InteractableType.None;

    public override void Interact()
    {
        OpenBoxInventory();
    }

    public override void ShowInteractability(bool show)
    {
        _isInteractable = show;
    }

    public void OpenBoxInventory()
    {
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UITideUpBoxManager.Instance.OpenTideUpBox(this);
    }

    public void CollectAllItems()
    {
        if (_boxInventory.Count == 0)
        {
            Debug.Log("There are no items to collect, wait until the next morning");
            return;
        }

        int spaceLeft = InventoryManager.Instance.MaxItems - InventoryManager.Instance.Items.Count;

        if (_boxInventory.Count > spaceLeft)
        {
            Debug.Log("Not enough space free to collect all items");
            return;
        }

        foreach (TrashItemInstance trash in _boxInventory)
        {
            if (!InventoryManager.Instance.AddItem(trash))
            {
                return;
            }
        }
        _boxInventory.Clear();

        Debug.Log("Collected all items from Tide-Up-Box");

        // If we want to collect trash till inventory is full:
        //List<TrashData> itemsToKeep = new();

        //foreach (TrashData trash in _boxInventory)
        //{
        //    if (!InventoryManager.Instance.AddItem(trash))
        //    {
        //        itemsToKeep.Add(trash);
        //    }
        //}

        //int itemsCollected = _boxInventory.Count - itemsToKeep.Count;
        //_boxInventory = itemsToKeep;

        //Debug.Log("Collected " + itemsCollected + " items. " + itemsToKeep.Count + " left in the Tide-Up-Box.");
    }

    public void CollectOneItem(TrashItemInstance itemToCollect)
    {
        if (itemToCollect == null)
        {
            Debug.Log("There are no items to collect, wait until the next morning");
            return;
        }

        if (!_boxInventory.Contains(itemToCollect))
        {
            Debug.Log("Asked item to collect is not in this box");
            return;
        }

        if (!InventoryManager.Instance.AddItem(itemToCollect))
        {
            //Debug log about no space left in AddItem()
            return;
        }

        _boxInventory.Remove(itemToCollect);
    }

    private void OnDrawGizmos()
    {
        if (_isInteractable)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(1.5f, 0.7f, 0.7f));
        }
    }

}
