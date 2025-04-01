using System;
using Data;
using UnityEngine;

public class Recycler : Interactable
{
    private bool _playerInRange = false;

    private ItemData _storedItem;
    public bool HasStoredItem => _storedItem != null;

    private bool _isInteractable = false;
    
    
    public bool PlayerInRange
    {
        get { return _playerInRange; }
        set { _playerInRange = value; }
    }
    public ItemData StoredItem
    {
        get { return _storedItem; }
        set { _storedItem = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            _playerInRange = false; 
            if (_storedItem != null)
            {
                InventoryManager.Instance.TryAddItem(_storedItem);
                _storedItem = null;
            }
        }
            
    }

    public bool StoreItem(ItemData item)
    {
        if (_storedItem != null) return false;

        _storedItem = item;
        return true;
    }

    public override void Interact()
    {
        Debug.Log("Recycler");
    }

    public override void ShowInteractability(bool show)
    {
        _isInteractable = show;
    }

    private void OnDrawGizmos()
    {
        if (_isInteractable)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 2, 1));
        }
    }
}
