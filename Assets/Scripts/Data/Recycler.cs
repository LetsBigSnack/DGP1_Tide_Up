using System;
using Data;
using UnityEngine;

public class Recycler : Interactable
{

    private bool _isInteractable = false;
    public bool isEnabled = true;

    public override InteractableType Type => InteractableType.Recycler;
    public override void Interact()
    {
        if (isEnabled)
        {
            UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Recycler);
        }
    }

    public override void ShowInteractability(bool show)
    {
        if (isEnabled)
        {
            _isInteractable = show;
        }
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
