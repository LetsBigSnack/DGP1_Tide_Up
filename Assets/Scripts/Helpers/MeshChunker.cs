using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshChunker : MonoBehaviour
{
    public int chunksX = 2;
    public int chunksZ = 2;
    public float chunkPadding = 0.01f;

    [ContextMenu("Chunk Mesh")]
    public void ChunkMesh()
    {
#if UNITY_EDITOR
        
        
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshCollider originalCollider = GetComponent<MeshCollider>();

        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogError("No mesh found to chunk!");
            return;
        }

        Mesh mesh = mf.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Bounds bounds = mesh.bounds;
        float chunkWidth = bounds.size.x / chunksX;
        float chunkDepth = bounds.size.z / chunksZ;

        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                Vector3 min = new Vector3(bounds.min.x + chunkWidth * x, bounds.min.y, bounds.min.z + chunkDepth * z);
                Vector3 max = min + new Vector3(chunkWidth, bounds.size.y, chunkDepth);
                Bounds chunkBounds = new Bounds((min + max) / 2f, max - min);
                
                chunkBounds.Expand(chunkPadding);

                List<Vector3> chunkVerts = new();
                List<int> chunkTris = new();
                Dictionary<int, int> vertMap = new();

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int i0 = triangles[i];
                    int i1 = triangles[i + 1];
                    int i2 = triangles[i + 2];

                    Vector3 v0 = vertices[i0];
                    Vector3 v1 = vertices[i1];
                    Vector3 v2 = vertices[i2];

                    if (chunkBounds.Contains(v0) || chunkBounds.Contains(v1) || chunkBounds.Contains(v2))
                    {
                        int newI0 = AddVertex(i0, v0, chunkVerts, vertMap);
                        int newI1 = AddVertex(i1, v1, chunkVerts, vertMap);
                        int newI2 = AddVertex(i2, v2, chunkVerts, vertMap);

                        chunkTris.Add(newI0);
                        chunkTris.Add(newI1);
                        chunkTris.Add(newI2);
                    }
                }

                if (chunkVerts.Count > 0)
                {
                    Mesh chunkMesh = new Mesh();
                    chunkMesh.name = $"Chunk_{x}_{z}_Mesh";
                    chunkMesh.vertices = chunkVerts.ToArray();
                    chunkMesh.triangles = chunkTris.ToArray();
                    chunkMesh.RecalculateNormals();

                    GameObject chunkGO = new GameObject($"Chunk_{x}_{z}");
                    chunkGO.transform.SetParent(transform);
                    chunkGO.transform.localPosition = Vector3.zero;
                    chunkGO.transform.localRotation = Quaternion.identity;

                    MeshFilter mfNew = chunkGO.AddComponent<MeshFilter>();
                    MeshRenderer mrNew = chunkGO.AddComponent<MeshRenderer>();
                    MeshCollider mcNew = chunkGO.AddComponent<MeshCollider>();

                    mfNew.mesh = chunkMesh;
                    mrNew.sharedMaterial = mr.sharedMaterial;
                    mcNew.sharedMesh = chunkMesh;
                }
            }
        }
        
        
        mr.enabled = false;
        if (originalCollider != null)
        {
            originalCollider.enabled = false;
        }
        
        Debug.Log("Mesh chunking completed.");
#else
        Debug.LogWarning("This method can only be used in the Unity Editor.");
#endif
    }

    private int AddVertex(int originalIndex, Vector3 vertex, List<Vector3> newVerts, Dictionary<int, int> map)
    {
        if (map.ContainsKey(originalIndex))
            return map[originalIndex];

        int newIndex = newVerts.Count;
        newVerts.Add(vertex);
        map.Add(originalIndex, newIndex);
        return newIndex;
    }
}
