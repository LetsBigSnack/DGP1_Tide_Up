using UnityEngine;

namespace Data
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void Interact();
        public abstract void ShowInteractability(bool show);
    }
}