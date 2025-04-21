using Data;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private bool highlight;
    [SerializeField] private Scenes scene;


    public override InteractableType Type => InteractableType.Door;

    public override void Interact()
    {
        LocationManager.Instance.TravelToScene(scene);
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

    private void OnDrawGizmos()
    {
        if (highlight)
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
        
    }
}
