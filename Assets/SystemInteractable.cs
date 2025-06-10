using Data;
using UnityEngine;

public class SystemInteractable : Interactable
{
    [TextArea(10,300)]
    [SerializeField] private string text;
    [SerializeField] private Color labelColor;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Sprite img;

    public override InteractableType Type => InteractableType.System;

    public override void Interact()
    {
        if (!isOpen)
        {
            isOpen = true;
            SystemInteractionManager.Instance.SetCurrentInteractable(this);
            GameStateManager.Instance.SetGameState(GameStates.Dialogue);
            UIDialogueManager.Instance.SetSystemDialogueBox(img, "System", text, labelColor);
            return;
        }
        EndInteraction();
    }

    public void EndInteraction()
    {
        UIDialogueManager.Instance.ShowDialogueBox(false);
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        SystemInteractionManager.Instance.SetCurrentInteractable(null);
        isOpen = false;
        Destroy(gameObject);
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public override void ShowInteractability(bool show)
    {
        //no vision
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isOpen)
        {
            Interact();
        }
    }
}
