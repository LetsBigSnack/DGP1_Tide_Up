using System;
using System.Collections.Generic;
using UnityEngine;

public enum EdgeType
{
    CornerTopLeft,
    CornerTopRight,
    CornerBottomLeft,
    CornerBottomRight,
    EdgeTop,
    EdgeBottom,
    EdgeLeft,
    EdgeRight
}

[Serializable]
public class WaveAnimationSettings
{
    public bool animatedEdge = false;
    public List<EdgeSettings> edgeSettings;
    
}

[Serializable]
public class EdgeSettings
{
    public EdgeType edgeType;
    public bool animatedEdge;
}


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
    
    [SerializeField] private int edgeDistance = 1;
    
    private List<EdgeVertex> _edgeVertices = new List<EdgeVertex>();


    [SerializeField] private WaveAnimationSettings anitmationSetting;
    
    private Dictionary<EdgeType, List<EdgeVertex>> _edgeVertex = new Dictionary<EdgeType, List<EdgeVertex>>();
    
    
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

        if (anitmationSetting.animatedEdge)
        {
            AnimateEdges();
        }
        else
        {
            AnimateWaves();
        }
        
        
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        
        _normalUpdateTimer += Time.deltaTime;
        if (_normalUpdateTimer >= normalUpdateInterval)
        {
            
            _normalUpdateTimer = 0f;
        }
        
    }
    


    void GenerateMesh()
    {
        //Edges 
        List<EdgeVertex> _edgeTopVertices = new List<EdgeVertex>();
        List<EdgeVertex> _edgeBottomVertices = new List<EdgeVertex>();
        List<EdgeVertex> _edgeLeftVertices = new List<EdgeVertex>();
        List<EdgeVertex> _edgeRightVertices = new List<EdgeVertex>();

        //Corners
        List<EdgeVertex> _cornerTopLeftVertices = new List<EdgeVertex>();
        List<EdgeVertex> _cornerTopRightVertices = new List<EdgeVertex>();
        List<EdgeVertex> _cornerBottomLeftVertices = new List<EdgeVertex>();
        List<EdgeVertex> _cornerBottomRightVertices = new List<EdgeVertex>();
        
        
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


                if (x <= (edgeDistance - 1) && z <= (edgeDistance - 1))
                {
                    //topLeftCorner
                    _cornerTopLeftVertices.Add(new EdgeVertex(i,x,z));
                }else if (x <= (edgeDistance - 1) && z >= dimension - (edgeDistance - 1))
                {
                    //topRigthCorner
                    _cornerTopRightVertices.Add(new EdgeVertex(i, x, z));
                }else if (x >= dimension-(edgeDistance-1) && z <= (edgeDistance - 1))
                {
                    //bottomLeftCorner
                    _cornerBottomLeftVertices.Add(new EdgeVertex(i, x, z));
                }else if (x >= dimension-(edgeDistance-1) && z >= dimension - (edgeDistance - 1))
                {
                    //bottomRightCorner
                    _cornerBottomRightVertices.Add(new EdgeVertex(i, x, z));
                }else if (x <= (edgeDistance - 1))
                {
                    _edgeTopVertices.Add(new EdgeVertex(i,x,z));
                }else if (x >= dimension - (edgeDistance - 1))
                {
                    _edgeBottomVertices.Add(new EdgeVertex(i, x, z));
                }else if (z <= (edgeDistance - 1))
                {
                    _edgeLeftVertices.Add(new EdgeVertex(i,x,z));
                }else if (z >= dimension-(edgeDistance-1))
                {
                    _edgeRightVertices.Add(new EdgeVertex(i,x,z));
                }
                
                
                if (x <= (edgeDistance-1) || z <= (edgeDistance-1) || x >= dimension-(edgeDistance-1) || z >= dimension-(edgeDistance-1))
                {
                    _edgeVertices.Add(new EdgeVertex(i,x,z));
                }
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
        
        _edgeVertex.Add(EdgeType.CornerTopLeft, _cornerTopLeftVertices);
        _edgeVertex.Add(EdgeType.CornerTopRight, _cornerTopRightVertices);
        _edgeVertex.Add(EdgeType.CornerBottomLeft, _cornerBottomLeftVertices);
        _edgeVertex.Add(EdgeType.CornerBottomRight, _cornerBottomRightVertices);
        _edgeVertex.Add(EdgeType.EdgeTop, _edgeTopVertices);
        _edgeVertex.Add(EdgeType.EdgeBottom, _edgeBottomVertices);
        _edgeVertex.Add(EdgeType.EdgeLeft, _edgeLeftVertices);
        _edgeVertex.Add(EdgeType.EdgeRight, _edgeRightVertices);
        
        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = _uvs;

        GetComponent<MeshFilter>().mesh = _mesh;
    }
    

    void AnimateWaves()
    {
        float dimensionFactor = (float)referenceDimension / dimension;
        float maxDistance = dimension / 2f;

        float offsetX = transform.position.x / quadSize;
        float offsetZ = transform.position.z / quadSize;

        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                int i = Index(x, z);
                Vector3 vertex = _vertices[i];

                float sampleX = (x + offsetX);
                float sampleZ = (z + offsetZ);

                float y = 0f;

                foreach (var octave in _octaves)
                {
                    if (octave.alternate)
                    {
                        float perl = Mathf.PerlinNoise(
                            (sampleX * octave.scale.x * dimensionFactor) / dimension,
                            (sampleZ * octave.scale.y * dimensionFactor) / dimension
                        ) * Mathf.PI * 2f;

                        y += Mathf.Cos(perl + octave.speed.magnitude * Time.time * dimensionFactor) * octave.height;
                    }
                    else
                    {
                        float perl = Mathf.PerlinNoise(
                            (sampleX * octave.scale.x * dimensionFactor + Time.time * octave.speed.x * dimensionFactor) / dimension,
                            (sampleZ * octave.scale.y * dimensionFactor + Time.time * octave.speed.y * dimensionFactor) / dimension
                        ) - 0.5f;

                        y += perl * octave.height;
                    }
                }

                Vector2 pos = new Vector2(vertex.x, vertex.z);
                float distance = pos.magnitude;
                float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
                float fade = falloffCurve.Evaluate(normalizedDistance);

                vertex.y = y * fade;
                _vertices[i] = vertex;
            }
        }

        _mesh.vertices = _vertices;
    }


    private void AnimateEdges()
    {
        Debug.Log(_edgeVertices.Count + ":" + _vertices.Length);
        float dimensionFactor = (float)referenceDimension / dimension;
        float maxDistance = dimension / 2f;

        float offsetX = transform.position.x / quadSize;
        float offsetZ = transform.position.z / quadSize;


        foreach (EdgeSettings edgeSetting in anitmationSetting.edgeSettings)
        {
            if (edgeSetting.animatedEdge)
            {
                foreach (EdgeVertex edge in _edgeVertex[edgeSetting.edgeType])
                {
                    Vector3 vertex = _vertices[edge.index];

            
                    float sampleX = edge.x + offsetX;
                    float sampleZ = edge.z + offsetZ;

                    float y = 0f;

                    foreach (var octave in _octaves)
                    {
                        if (octave.alternate)
                        {
                            float perl = Mathf.PerlinNoise(
                                (sampleX * octave.scale.x * dimensionFactor) / dimension,
                                (sampleZ * octave.scale.y * dimensionFactor) / dimension
                            ) * Mathf.PI * 2f;

                            y += Mathf.Cos(perl + octave.speed.magnitude * Time.time * dimensionFactor) * octave.height;
                        }
                        else
                        {
                            float perl = Mathf.PerlinNoise(
                                (sampleX * octave.scale.x * dimensionFactor + Time.time * octave.speed.x * dimensionFactor) / dimension,
                                (sampleZ * octave.scale.y * dimensionFactor + Time.time * octave.speed.y * dimensionFactor) / dimension
                            ) - 0.5f;

                            y += perl * octave.height;
                        }
                    }

                    Vector2 pos = new Vector2(vertex.x, vertex.z);
                    float distance = pos.magnitude;
                    float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
                    float fade = falloffCurve.Evaluate(normalizedDistance);

                    vertex.y = y * fade;
                    _vertices[edge.index] = vertex;
                }
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
    
    public void SetAnimationSettings(WaveAnimationSettings settings)
    {
        this.anitmationSetting = settings;
        foreach (EdgeSettings edgeSetting in settings.edgeSettings)
        {
            if (!edgeSetting.animatedEdge && _edgeVertex.TryGetValue(edgeSetting.edgeType, out var verts))
            {
                foreach (var edge in verts)
                {
                    var v = _vertices[edge.index];
                    v.y = 0f;
                    _vertices[edge.index] = v;
                }
            }
        }

        // Apply changes to the mesh
        if (_mesh != null)
        {
            _mesh.vertices = _vertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }


    void OnDrawGizmos()
    {
        if (_vertices == null) return;

        // Draw all vertices (optional, can comment out)
        Gizmos.color = Color.green;
        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(_vertices[i]);
            Gizmos.DrawSphere(worldPos, 0.05f);
        }

        void DrawEdgeGroup(List<EdgeVertex> group, Color color, float size = 0.15f)
        {
            Gizmos.color = color;
            foreach (var edge in group)
            {
                Vector3 worldPos = transform.TransformPoint(_vertices[edge.index]);
                Gizmos.DrawSphere(worldPos, size);
            }
        }

        // Edges
        DrawEdgeGroup(_edgeVertex[EdgeType.EdgeTop], Color.blue);       // Top = Blue
        DrawEdgeGroup(_edgeVertex[EdgeType.EdgeBottom], Color.cyan);    // Bottom = Cyan
        DrawEdgeGroup(_edgeVertex[EdgeType.EdgeLeft], Color.yellow);    // Left = Yellow
        DrawEdgeGroup(_edgeVertex[EdgeType.EdgeRight], Color.magenta);  // Right = Magenta

        // Corners (larger + distinct)
        DrawEdgeGroup(_edgeVertex[EdgeType.CornerTopLeft], new Color(204.0f/255.0f,85.0f/255.0f,0,1), 0.25f);
        DrawEdgeGroup(_edgeVertex[EdgeType.CornerTopRight], new Color(104.0f/255.0f,85.0f/255.0f,0,1), 0.25f);
        DrawEdgeGroup(_edgeVertex[EdgeType.CornerBottomLeft], new Color(104.0f/255.0f,185.0f/255.0f,10.0f/255.0f,1), 0.25f);
        DrawEdgeGroup(_edgeVertex[EdgeType.CornerBottomRight], new Color(124.0f/255.0f,85.0f/255.0f,200.0f/255.0f,1), 0.25f);
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


public class EdgeVertex
{
    public int index;
    public int x;
    public int z;

    public EdgeVertex(int index, int x, int z)
    {
        this.index = index;
        this.x = x;
        this.z = z;
    }
}