using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicWater : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Wave Settings")]
    [Range(0f, (float)Math.PI*2)]
    public float waveFrequency = 1f;
    public float waveAmplitude = 0.5f;
    public float waveSpeed = 1f;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3[] _originalVertices;
    private int[] _triangles;

    void Start()
    {
        GenerateMesh();
    }

    void Update()
    {
        AnimateWaves();
        UpdateMesh();
    }

    void GenerateMesh()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _vertices = new Vector3[(width + 1) * (height + 1)];
        _originalVertices = new Vector3[_vertices.Length];
        _triangles = new int[width * height * 6];

        for (int z = 0, i = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                Vector3 vert = new Vector3(x * cellSize, 0, z * cellSize);
                _vertices[i] = vert;
                _originalVertices[i] = vert;
            }
        }

        int tris = 0;
        for (int z = 0, i = 0; z < height; z++, i++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                _triangles[tris + 0] = i;
                _triangles[tris + 1] = i + width + 1;
                _triangles[tris + 2] = i + 1;

                _triangles[tris + 3] = i + 1;
                _triangles[tris + 4] = i + width + 1;
                _triangles[tris + 5] = i + width + 2;

                tris += 6;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();
    }

    void AnimateWaves()
    {
        float time = Time.time * waveSpeed;

        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector3 original = _originalVertices[i];

            // Combine x and z for diagonal wave movement
            float wave = Mathf.Sin((original.x + original.z) * waveFrequency + time) * waveAmplitude;

            _vertices[i].y = wave;
        }
    }

    void UpdateMesh()
    {
        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
    }
    
    void OnDrawGizmos()
    {
        if (_vertices == null) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < _vertices.Length; i++)
        {
            // Convert local mesh vertex to world position
            Vector3 worldPos = transform.TransformPoint(_vertices[i]);
            Gizmos.DrawSphere(worldPos, 0.05f);
        }
    }

}
