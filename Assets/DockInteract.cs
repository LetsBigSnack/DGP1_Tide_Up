using System.Collections;
using System.Linq;
using Data;
using UnityEngine;

public class DockInteract : Interactable
{

    public override InteractableType Type => InteractableType.Dock;
    
    [SerializeField] private Transform playerDockPosition;
    [SerializeField] private Transform dockTarget;
    [SerializeField] private float dockingDuration = 2f;
    
    
    public override void Interact()
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            ExitBoat();
        }else if (GameStateManager.Instance.GetGameState() == GameStates.PlayingCharacter)
        {
            EnterBoat();
        }
        
    }

    private void ExitBoat()
    {
        GameObject player = Player.Instance.gameObject;

 
        player.transform.SetParent(null);
        DontDestroyOnLoad(player);
        
        
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true; 
            rb.MovePosition(playerDockPosition.position); 
            rb.MoveRotation(playerDockPosition.rotation);
        }

    
        MeshRenderer mesh = player.GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = true;

     
        Collider col = player.GetComponentInChildren<Collider>();
        if (col != null)
            col.enabled = true;

    
        InteractionManager.Instance.SetInteractionRadius(4f);
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        CamerController.Instance.SwitchTarget(CameraTarget.Player);

        
        StartCoroutine(ReenableAfterPhysics(player));
    }

    private IEnumerator ReenableAfterPhysics(GameObject player)
    {
        yield return new WaitForFixedUpdate(); 

        // Re-enable movement and physics
        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = true;

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        rb.detectCollisions = true;
    }

    
    
    private void EnterBoat()
    {
        GameObject player = Player.Instance.gameObject;

       
        InteractionManager.Instance.SetInteractionRadius(10);
        
        player.transform.SetParent(Boat.Instance.gameObject.transform);

  
        player.transform.localPosition = new Vector3(0, 0.5f, 0);
        player.transform.localRotation = Quaternion.identity;
        
        MeshRenderer mesh = player.GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;
        
        
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
        }
        
        player.GetComponent<Rigidbody>().detectCollisions = false;
        player.GetComponentInChildren<Collider>().enabled = false;
        
        GameStateManager.Instance.SetGameState(GameStates.PlayingBoat);
        CamerController.Instance.SwitchTarget(CameraTarget.Boat);
    }

    public override void ShowInteractability(bool show)
    {
        return;
    }
}
