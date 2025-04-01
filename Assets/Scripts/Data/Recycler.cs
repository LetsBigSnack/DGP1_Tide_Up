using UnityEngine;

public class Recycler : MonoBehaviour
{
    private bool _playerInRange = false;

    private ItemData _storedItem;
    public bool HasStoredItem => _storedItem != null;

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

}
