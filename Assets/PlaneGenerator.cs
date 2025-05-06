using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomPlaneGenerator : MonoBehaviour
{
    [Header("Plane Settings")]
    public float width = 150;
    public float length = 150;
    public int widthSegments = 50;
    public int lengthSegments = 50;
    public float uvScale = 2f;
    public bool generateTangents = true;

    [Header("Mesh Saving")]
    public bool saveMeshInPlayMode = false;
    private static bool meshSaved = false;

    private void Awake()
    {
        GenerateAndAssignMesh();

#if UNITY_EDITOR
        if (saveMeshInPlayMode && Application.isPlaying && !meshSaved)
        {
            SaveMeshAsset("CustomPlane_" + widthSegments + "x" + lengthSegments);
            meshSaved = true;
        }
#endif
    }

    private void GenerateAndAssignMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedPlane";

        int vertsX = widthSegments + 1;
        int vertsZ = lengthSegments + 1;

        Vector3[] vertices = new Vector3[vertsX * vertsZ];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[widthSegments * lengthSegments * 6];

        for (int z = 0; z < vertsZ; z++)
        {
            for (int x = 0; x < vertsX; x++)
            {
                int i = x + z * vertsX;

                float xPos = ((float)x / widthSegments - 0.5f) * width;
                float zPos = ((float)z / lengthSegments - 0.5f) * length;

                vertices[i] = new Vector3(xPos, 0f, zPos);
                normals[i] = Vector3.up;
                uvs[i] = new Vector2(x / uvScale, z / uvScale);
            }
        }

        int tri = 0;
        for (int z = 0; z < lengthSegments; z++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                int i = x + z * vertsX;

                triangles[tri++] = i;
                triangles[tri++] = i + vertsX;
                triangles[tri++] = i + 1;

                triangles[tri++] = i + 1;
                triangles[tri++] = i + vertsX;
                triangles[tri++] = i + vertsX + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        if (generateTangents)
            mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }

#if UNITY_EDITOR
    private void SaveMeshAsset(string assetName)
    {
        string folderPath = "Assets/Meshes/";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string assetPath = folderPath + assetName + ".asset";

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Mesh meshCopy = Object.Instantiate(mesh);

        AssetDatabase.CreateAsset(meshCopy, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CustomPlaneGenerator] Saved mesh to {assetPath}");
    }
#endif
}
