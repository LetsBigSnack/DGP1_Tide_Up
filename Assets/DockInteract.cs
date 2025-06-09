using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class DockInteract : Interactable
{

    public override InteractableType Type => InteractableType.Dock;
    
    [Header("Dock Alignment Settings")]
    [SerializeField] private Transform playerDockPosition;
    [SerializeField] private Transform dockTarget;
    [SerializeField] private float dockingDuration = 2f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector3 obstacleCheckSize = new Vector3(2f, 2f, 2f);
    
    public override void Interact()
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            //ExitBoat();

            StartCoroutine(PerformDockingSequence());

        }else if (GameStateManager.Instance.GetGameState() == GameStates.PlayingCharacter)
        {
            EnterBoat();
        }
        
    }

    private void ExitBoat()
    {
        GameObject player = Player.Instance.gameObject;
        
        SkinnedMeshRenderer[] meshes = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            if (mesh != null)
                mesh.enabled = true;
        }
        
        InteractionManager.Instance.SetInteractionRadius(4f);
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        CamerController.Instance.SwitchTarget(CameraTarget.Player);
        
        BoatController.Instance.AnchorBoat(true);
        
        //TODO: Add sound
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Welcome to:", EnvironmentManager.Instance.CurrentIsland.IslandName);
    }
    

    
    
    private void EnterBoat()
    {
        GameObject player = Player.Instance.gameObject;

       
        InteractionManager.Instance.SetInteractionRadius(10);
        BoatController.Instance.AnchorBoat(false);
        
        SkinnedMeshRenderer[] meshes = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            if (mesh != null)
                mesh.enabled = false;
        }
        
        
        GameStateManager.Instance.SetGameState(GameStates.PlayingBoat);
        CamerController.Instance.SwitchTarget(CameraTarget.Boat);

        //TODO: Add sound
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Welcome to:", "Delkid Sea");
    }
    
    
    private IEnumerator PerformDockingSequence()
    {
        BoatController boat = BoatController.Instance;
        Transform boatTransform = boat.transform;

        // Check for space at docking area
        if (Physics.CheckBox(dockTarget.position, obstacleCheckSize * 0.5f, dockTarget.rotation, obstacleLayer))
        {
            Debug.LogWarning("Docking obstructed. Cancelling.");
            yield break;
        }

        GameObject player = Player.Instance.gameObject;

        // Enable player mesh again
        foreach (SkinnedMeshRenderer mesh in player.GetComponentsInChildren<SkinnedMeshRenderer>())
            if (mesh != null) mesh.enabled = true;

        InteractionManager.Instance.SetInteractionRadius(4f);
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        CamerController.Instance.SwitchTarget(CameraTarget.Player);

        // Smoothly move and rotate the boat
        Vector3 startPos = boatTransform.position;
        Quaternion startRot = boatTransform.rotation;
        Vector3 endPos = dockTarget.position;
        Quaternion endRot = dockTarget.rotation;

        float elapsed = 0f;
        while (elapsed < dockingDuration)
        {
            float t = elapsed / dockingDuration;
            boatTransform.position = Vector3.Lerp(startPos, endPos, t);
            boatTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to dockTarget and anchor
        boatTransform.position = endPos;
        boatTransform.rotation = endRot;
        boat.AnchorBoat(true);

        // Feedback
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Welcome to:", EnvironmentManager.Instance.CurrentIsland.IslandName);
    }

    public override void ShowInteractability(bool show)
    {
        return;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (dockTarget == null) return;

        // Draw docking target position and forward
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(dockTarget.position, 0.5f);
        Gizmos.DrawLine(dockTarget.position, dockTarget.position + dockTarget.forward * 2f);

        // Draw obstacle check box
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(dockTarget.position, dockTarget.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, obstacleCheckSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
