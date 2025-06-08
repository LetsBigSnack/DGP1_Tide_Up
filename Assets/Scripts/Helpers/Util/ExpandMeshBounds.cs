using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class ExpandMeshBounds : MonoBehaviour
{
    [SerializeField] private float expandAmount = 15f;

    // Static cache so we only expand each unique mesh once
    private static Dictionary<Mesh, Mesh> meshCache = new();

    void Start()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        var original = mf.sharedMesh;

        if (!meshCache.TryGetValue(original, out Mesh modifiedMesh))
        {
            modifiedMesh = Instantiate(original);
            modifiedMesh.name = original.name + "_Expanded_" + expandAmount;
            var bounds = modifiedMesh.bounds;
            bounds.Expand(expandAmount);
            modifiedMesh.bounds = bounds;
            meshCache[original] = modifiedMesh;
        }

        mf.sharedMesh = modifiedMesh;
    }
}
