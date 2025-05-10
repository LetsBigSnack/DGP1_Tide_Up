using System.Collections;
using System.Collections.Generic;
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
        
        SkinnedMeshRenderer[] meshes = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            if (mesh != null)
                mesh.enabled = true;
        }
        
        InteractionManager.Instance.SetInteractionRadius(4f);
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        CamerController.Instance.SwitchTarget(CameraTarget.Player);

        //TODO: Add sound
        UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Welcome to:", EnvironmentManager.Instance.CurrentIsland.IslandName);
    }
    

    
    
    private void EnterBoat()
    {
        GameObject player = Player.Instance.gameObject;

       
        InteractionManager.Instance.SetInteractionRadius(10);
      
        
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

    public override void ShowInteractability(bool show)
    {
        return;
    }
}
