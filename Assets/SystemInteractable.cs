using Data;
using System;
using UnityEngine;

public class SystemInteractable : Interactable
{
    [SerializeField] private string ID;
    [TextArea(10, 300)]
    [SerializeField] private string text;
    [SerializeField] private Color labelColor;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Sprite img;
    [SerializeField] private bool hasBeenTriggered;

    public override InteractableType Type => InteractableType.System;

    private void Start()
    {
        SystemInteractionManager.Instance.CreateSystemInteractableData(this);
        GetInteractionData();
    }

    public bool HasBeenTriggered()
    {
        return hasBeenTriggered;
    }

    public string GetID()
    {
        return ID;
    }

    public void GetInteractionData()
    {
        InteractionData data = SystemInteractionManager.Instance.GetDataByID(ID);
        hasBeenTriggered = data.hasBeenTriggered;
    }
    public override void Interact()
    {
        if (!isOpen && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
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
        SystemInteractionManager.Instance.SetInteractableData(ID, hasBeenTriggered);
        isOpen = false;
        gameObject.SetActive(false);
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
        if (other.CompareTag("Player") && !isOpen)
        {
            Interact();
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = GenerateID();
        }
    }
#endif

    private string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }
}
