using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ExpandMeshBounds : MonoBehaviour
{
    [SerializeField] private float expandAmount = 20f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private MeshFilter mf;

    // Static cache so we only expand each unique mesh once
    private static Dictionary<Mesh, Mesh> meshCache = new();

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        if(mf == null) return;

        if (mesh == null) 
        {
            mesh = mf.sharedMesh;
        }
        else
        {
            mf.sharedMesh = mesh;
        }

        if (!meshCache.TryGetValue(mesh, out Mesh modifiedMesh))
        {
            modifiedMesh = Instantiate(mesh);
            modifiedMesh.name = mesh.name + "_Expanded_" + expandAmount;
            var bounds = modifiedMesh.bounds;
            bounds.Expand(expandAmount);
            modifiedMesh.bounds = bounds;
            meshCache[mesh] = modifiedMesh;
        }

        mf.sharedMesh = modifiedMesh;
    }

    private void OnDestroy()
    {
        mf.sharedMesh = mesh;
    }
}
