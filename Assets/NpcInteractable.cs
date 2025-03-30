using System;
using Data;
using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private bool highlight;
    
    private Npc _npc;


    private void Awake()
    {
        _npc = GetComponent<Npc>();
    }
    
    public override void Interact()
    {
        _npc.InteractDialogue();
    }
    
    public override void ShowInteractability(bool show)
    {
        highlight = show;
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
