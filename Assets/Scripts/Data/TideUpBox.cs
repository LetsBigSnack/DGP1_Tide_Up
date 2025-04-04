using System;
using Data;
using UnityEngine;

public class TideUpBox : Interactable
{

    private bool _isInteractable = false;

    public override void Interact()
    {
        Debug.Log("Tide up box, GO");
        TideUpBoxManager.Instance.CollectAllItems();
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
            Gizmos.DrawWireCube(transform.position, new Vector3(1.5f, 0.7f, 0.7f));
        }
    }
}
