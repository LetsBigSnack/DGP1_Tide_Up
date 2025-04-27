using Data;
using TMPro.Examples;
using UnityEngine;

public class BoatInteractable : Interactable
{
    [SerializeField] private TrashData trashData;
    [SerializeField] private bool highlight;

    public override InteractableType Type => InteractableType.Boat;

    public override void Interact()
    {
        Debug.Log("Interact");
        CamerController.Instance.SwitchTarget(CameraTarget.Boat);
        GameStateManager.Instance.SetGameState(GameStates.PlayingBoat);
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
