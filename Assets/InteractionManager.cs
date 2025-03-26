using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float interactionRange = 4.0f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(204.0f/255.0f,85.0f/255.0f,0,1);
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
