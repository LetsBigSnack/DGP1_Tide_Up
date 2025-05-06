using System.Linq;
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
        DockInteract dock = FindObjectsByType<DockInteract>(FindObjectsInactive.Include,FindObjectsSortMode.None).FirstOrDefault();
        if (dock != null) dock.enabled = true;
        Debug.Log("Interact");
        CamerController.Instance.SwitchTarget(CameraTarget.Boat);
        EnterBoat();
        GameStateManager.Instance.SetGameState(GameStates.PlayingBoat);
        enabled = false;
    }
    
    
    
    private void EnterBoat()
    {
        GameObject player = Player.Instance.gameObject;

        //TODO: chaneg for later 
        InteractionManager.Instance.SetInteractionRadius(10);
        
        player.transform.SetParent(Boat.Instance.gameObject.transform);

  
        player.transform.localPosition = new Vector3(0, 0.5f, 0);
        player.transform.localRotation = Quaternion.identity;
        
        MeshRenderer mesh = player.GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;
        
        // Disable PlayerController
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        // Disable Rigidbody physics
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // disables physics
            rb.linearVelocity = Vector3.zero; // stop previous motion
        }
        
        player.GetComponent<Rigidbody>().detectCollisions = false;
        player.GetComponentInChildren<Collider>().enabled = false;
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
