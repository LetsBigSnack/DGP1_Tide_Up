using Data;
using UnityEngine;

public class Shop : Interactable
{
    private bool _isInteractable = false;

    public override InteractableType Type => InteractableType.Exchange;

    public override void Interact()
    {
        UIShopManager.Instance.SwitchState(ShopType.Exchange);
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
