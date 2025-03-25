using UnityEditor;
using UnityEngine;

public class WireFrameComponent : MonoBehaviour
{
    
    private void OnDrawGizmos()
    {
        MeshCollider collider = GetComponent<MeshCollider>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireMesh(collider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);

    }
}
