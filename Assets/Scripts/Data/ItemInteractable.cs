using System;
using Data;
using UnityEngine;

public class ItemInteractable : Interactable
{
    
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;
    
    public override void Interact()
    {
        Debug.Log("Interact");
        if (InventoryManager.Instance.AddItem(new TrashItemInstance(trashData)))
        {
            Destroy(this.gameObject);
        }
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

    private void OnDrawGizmos()
    {
        if (highlight)
        {
            BoxCollider collider = GetComponentInChildren<BoxCollider>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
        
    }
}
