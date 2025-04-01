using System;
using Data;
using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private bool highlight;
    
    private Npc _npc;
    private NPC_Controller _npcController;
    private bool _previousState;
    

    private void Awake()
    {
        _npc = GetComponent<Npc>();
        _npcController = GetComponent<NPC_Controller>();
    }
    
    public override void Interact()
    {
        _npc.InteractDialogue();
    }
    
    //TODO: investigate why this gets triggered multiple times
    //TODO: investigate why it is iffy when at the edge of the radius 
    public override void ShowInteractability(bool show)
    {
        if (show == _previousState)
        {
            return;
        }
        
        highlight = show;
        _npcController.CanMove = !show;
        _previousState = show;
        
        if (!show)
        {
            _npc.ResetDialogue();
        }
        
    }

    private void OnDrawGizmos()
    {
        if (highlight)
        {
            CapsuleCollider collider = GetComponentInChildren<CapsuleCollider>();
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position+new Vector3(0,1,0), collider.radius);
        }
        
    }
}
