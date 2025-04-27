using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Waves : MonoBehaviour
{
    [Header("Mesh Settings")]
    public int dimension = 50;
    [SerializeField] private int referenceDimension = 50;
    [SerializeField] private float quadSize = 2f;
    public float uvScale = 5f;

    [Header("Wave Settings")] 
    [SerializeField]
    private OceanType oceanType;
    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    
    [Header("Performance Settings")]
    private float _normalUpdateTimer = 0f;
    [SerializeField] private float normalUpdateInterval = 0.1f; 
    
    
    private List<Octave> _octaves;

    [Header("Dynamic Ocean Settings")]
    public Transform player;
    public Transform boat;
    public float followThreshold = 5f;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector2[] _uvs;

    private Vector3 _meshOffset;

    void Awake()
    {
        GenerateMesh();
        OceanPreset oceanPreset =  OceanManager.Instance.GetOceanPreset(oceanType);
        _octaves = oceanPreset.octaves;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    void FixedUpdate()
    {
        AnimateWaves();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        
        _normalUpdateTimer += Time.deltaTime;
        if (_normalUpdateTimer >= normalUpdateInterval)
        {
            
            _normalUpdateTimer = 0f;
        }
        
    }

    private void LateUpdate()
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingCharacter)
        {
            MoveOceanWithPlayer();
        }else if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            MoveOceanWithBoat();
        }
    }

    private void MoveOceanWithBoat()
    {
        if (player == null) return;

        Vector3 boatXZ = new Vector3(boat.position.x, 0f, boat.position.z);
        Vector3 patchXZ = new Vector3(transform.position.x, 0f, transform.position.z);

        if (Vector3.Distance(boatXZ, patchXZ) > followThreshold)
        {
            transform.position = new Vector3(boat.position.x, transform.position.y, boat.position.z);
        }
    }


    void GenerateMesh()
    {
        _mesh = new Mesh
        {
            name = gameObject.name
        };

        int vertCount = (dimension + 1) * (dimension + 1);
        _vertices = new Vector3[vertCount];
        _uvs = new Vector2[vertCount];
        int[] triangles = new int[dimension * dimension * 6];

        float halfSize = dimension * quadSize / 2f;

        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                int i = Index(x, z);
                _vertices[i] = new Vector3(
                    (x * quadSize) - halfSize,
                    0,
                    (z * quadSize) - halfSize
                );
                _uvs[i] = new Vector2((float)x / uvScale, (float)z / uvScale);
            }
        }

        int t = 0;
        for (int x = 0; x < dimension; x++)
        {
            for (int z = 0; z < dimension; z++)
            {
                int i = Index(x, z);

                // Correct triangle winding (clockwise)
                triangles[t++] = i;
                triangles[t++] = i + 1;
                triangles[t++] = i + dimension + 1;

                triangles[t++] = i + 1;
                triangles[t++] = i + dimension + 2;
                triangles[t++] = i + dimension + 1;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = _uvs;

        GetComponent<MeshFilter>().mesh = _mesh;
    }

    void MoveOceanWithPlayer()
    {
        if (player == null) return;

        Vector3 playerXZ = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 patchXZ = new Vector3(transform.position.x, 0f, transform.position.z);

        if (Vector3.Distance(playerXZ, patchXZ) > followThreshold)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        }
    }

    void AnimateWaves()
    {
        float dimensionFactor = (float)referenceDimension / dimension;
        float maxDistance = dimension / 2f;

        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                int i = Index(x, z);

                float y = 0f;

                foreach (var octave in _octaves)
                {
                    if (octave.alternate)
                    {
                        float perl = Mathf.PerlinNoise(
                            (x * octave.scale.x * dimensionFactor) / dimension,
                            (z * octave.scale.y * dimensionFactor) / dimension
                        ) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + octave.speed.magnitude * Time.time * dimensionFactor) * octave.height;
                    }
                    else
                    {
                        float perl = Mathf.PerlinNoise(
                            (x * octave.scale.x * dimensionFactor + Time.time * octave.speed.x * dimensionFactor) / dimension,
                            (z * octave.scale.y * dimensionFactor + Time.time * octave.speed.y * dimensionFactor) / dimension
                        ) - 0.5f;
                        y += perl * octave.height;
                    }
                }

                // Calculate distance-based fade
                Vector2 pos = new Vector2(_vertices[i].x, _vertices[i].z);
                float distance = pos.magnitude;
                float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

                float fade = falloffCurve.Evaluate(normalizedDistance);

                _vertices[i].y = y * fade;
            }
        }

        _mesh.vertices = _vertices;
    }


    public float GetHeight(Vector3 worldPos)
    {
        
        if (_vertices == null || _vertices.Length == 0)
        {
            Debug.LogError("Vertices not initialized yet!");
            return 0f;
        }
        
        var scale = new Vector3(1f / transform.lossyScale.x, 0f, 1f / transform.lossyScale.z);
        var localPos = Vector3.Scale(worldPos - transform.position, scale);

        int x0 = Mathf.Clamp(Mathf.FloorToInt(localPos.x + dimension / 2f), 0, dimension);
        int z0 = Mathf.Clamp(Mathf.FloorToInt(localPos.z + dimension / 2f), 0, dimension);
        int x1 = Mathf.Clamp(x0 + 1, 0, dimension);
        int z1 = Mathf.Clamp(z0 + 1, 0, dimension);

        float dx = localPos.x + dimension / 2f - x0;
        float dz = localPos.z + dimension / 2f - z0;

        float h00 = _vertices[Index(x0, z0)].y;
        float h10 = _vertices[Index(x1, z0)].y;
        float h01 = _vertices[Index(x0, z1)].y;
        float h11 = _vertices[Index(x1, z1)].y;

        float h0 = Mathf.Lerp(h00, h10, dx);
        float h1 = Mathf.Lerp(h01, h11, dx);

        float finalHeight = Mathf.Lerp(h0, h1, dz);

        return finalHeight * transform.lossyScale.y;
    }
    
    public float GetWorldHeight(Vector3 worldPos)
    {
        return GetHeight(worldPos) + transform.position.y;
    }
    
    int Index(int x, int z)
    {
        return x * (dimension + 1) + z;
    }

    void OnDrawGizmos()
    {
        if (_vertices == null) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(_vertices[i]);
            Gizmos.DrawSphere(worldPos, 0.1f);
        }
    }


}

[System.Serializable]
public class Octave
{
    public Vector2 speed;
    public Vector2 scale;
    public float height;
    public bool alternate;
}
