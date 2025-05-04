using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OceanFloorTile : MonoBehaviour
{
    public float tileSize = 50f;
    public int resolution = 50;
    public float noiseScale = 10f;
    public float heightMultiplier = 5f;

    public void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = CreateMesh();
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "OceanFloorTileMesh";

        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] triangles = new int[resolution * resolution * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++, i++)
            {
                float xPos = ((float)x / resolution - 0.5f) * tileSize;
                float zPos = ((float)z / resolution - 0.5f) * tileSize;

                float yPos = Mathf.PerlinNoise(xPos / noiseScale, zPos / noiseScale) * heightMultiplier;

                vertices[i] = new Vector3(xPos, yPos, zPos);
                uvs[i] = new Vector2((float)x / resolution, (float)z / resolution);
            }
        }

        int tris = 0;
        int vert = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + resolution + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + resolution + 1;
                triangles[tris + 5] = vert + resolution + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
