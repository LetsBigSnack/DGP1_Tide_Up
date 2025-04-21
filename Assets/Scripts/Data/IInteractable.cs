using UnityEngine;

namespace Data
{
    public enum InteractableType
    {
        None,
        Pickup,
        NPC,
        Recycler,
        Upcycler,
        Exchange,
        Door
    }

    public abstract class Interactable : MonoBehaviour
    {
        //TODO: why extra method and not just an attribute -> CHANGE
        public abstract InteractableType Type { get; }
        public abstract void Interact();
        public abstract void ShowInteractability(bool show);
    }
}