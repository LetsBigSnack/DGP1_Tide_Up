using System;
using Data;
using UnityEngine;

public class Recycler : Interactable
{

    private bool _isInteractable = false;

    public override void Interact()
    {
        UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Recycler);
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
