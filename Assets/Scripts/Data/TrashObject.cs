using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class TrashObject : MonoBehaviour
{
    [SerializeField] private TrashItemInstance data;
    private bool _collected = false;

    private void OnEnable()
    {
        //To get random trash, for testing
        data = DataUtil.Instance.GetRandomTrash();
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(25f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            TryCollect();
        }
    }

    private void TryCollect()
    {
        if (_collected) return;
        _collected = true;

        bool added = InventoryManager.Instance.AddItem(data);
        if (added)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full � can't collect this trash.");
        }
    }
}
