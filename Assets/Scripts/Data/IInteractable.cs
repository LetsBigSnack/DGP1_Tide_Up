using UnityEngine;

namespace Data
{
    public enum InteractableType
    {
        None,
        Pickup,
        NPC,
        Recycler,
        Upcycler
    }

    public abstract class Interactable : MonoBehaviour
    {
        public abstract InteractableType Type { get; }
        public abstract void Interact();
        public abstract void ShowInteractability(bool show);
    }
}