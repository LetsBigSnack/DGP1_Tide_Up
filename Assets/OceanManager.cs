using System;
using System.Collections.Generic;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    public static OceanManager Instance;

    [SerializeField] private List<OceanPreset> oceanPresets;

    [Header("Dynamic Waves")]
    public GameObject oceanTilePrefab;
    public int viewDistance = 3;

    [Header("Static Padding")]
    public GameObject staticOceanTilePrefab;
    public int staticPaddingDistance = 2;

    [Header("Common Settings")]
    public Transform targetTransform;
    public float tileSize = 150f;
    public float oceanHeight = -6.8f;

    private Waves currentTile;
    private Dictionary<Vector2, GameObject> dynamicTiles = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> staticTiles = new Dictionary<Vector2, GameObject>();
    private Vector2 previousCoord;
    
    [Header("Movement Barrier")]
    public bool useMovementBarrier = true;
    public float barrierDistance = 1.5f;
    public float barrierHeight = 50f;
    public float barrierThickness = 10f;
    public LayerMask barrierLayer;
    
    [Header("Barrier Decorations")]
    public bool spawnEdgePrefabs = true;
    public GameObject edgePrefab;
    public int objectsPerEdge = 100;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public OceanPreset GetOceanPreset(OceanType oceanType)
    {
        OceanPreset oceanPreset = oceanPresets.Find(o => o.type == oceanType);
        if (oceanPreset == null) throw new NullReferenceException();
        return oceanPreset;
    }

    void Start()
    {
        if (targetTransform == null && GameStateManager.Instance != null)
            targetTransform = GameStateManager.Instance.TargetTransform;

        previousCoord = GetCoord();
        UpdateTiles();
        
        if (useMovementBarrier)
        {
            GenerateBarrierWalls();
        }
    }
    
    private void GenerateBarrierWalls()
    {
        float size = (barrierDistance + 0.5f) * tileSize;
        
        CreateBarrier(new Vector3(0, oceanHeight + barrierHeight / 2, size), new Vector3(size * 2, barrierHeight, barrierThickness)); // Front
        CreateBarrier(new Vector3(0, oceanHeight + barrierHeight / 2, -size), new Vector3(size * 2, barrierHeight, barrierThickness)); // Back
        CreateBarrier(new Vector3(size, oceanHeight + barrierHeight / 2, 0), new Vector3(barrierThickness, barrierHeight, size * 2)); // Right
        CreateBarrier(new Vector3(-size, oceanHeight + barrierHeight / 2, 0), new Vector3(barrierThickness, barrierHeight, size * 2)); // Left
        
        if (spawnEdgePrefabs && edgePrefab != null)
        {
            SpawnEdgeObjects();
        }
        
    }
    
    private void SpawnEdgeObjects()
    {
        float size = (barrierDistance + 0.5f) * tileSize;
        float y = oceanHeight;
        
        Vector3 topLeft = new Vector3(-size, y, size);
        Vector3 topRight = new Vector3(size, y, size);
        Vector3 bottomRight = new Vector3(size, y, -size);
        Vector3 bottomLeft = new Vector3(-size, y, -size);

        SpawnLine(topLeft, topRight);     
        SpawnLine(topRight, bottomRight);
        SpawnLine(bottomRight, bottomLeft); 
        SpawnLine(bottomLeft, topLeft); 
    }
    
    void SpawnLine(Vector3 start, Vector3 end)
    {
        for (int i = 0; i <= objectsPerEdge; i++)
        {
            float t = i / (float)objectsPerEdge;
            Vector3 pos = Vector3.Lerp(start, end, t);
            Instantiate(edgePrefab, pos, Quaternion.identity, transform);
        }
    }

    private void CreateBarrier(Vector3 position, Vector3 size)
    {
        GameObject barrier = new GameObject("OceanBarrier");
        barrier.transform.parent = this.transform;
        barrier.transform.position = position;

        BoxCollider collider = barrier.AddComponent<BoxCollider>();
        collider.size = size;
        
        if (barrierLayer != 0)
            barrier.layer = Mathf.RoundToInt(Mathf.Log(barrierLayer.value, 2));
    }

    void Update()
    {
        targetTransform = GameStateManager.Instance.TargetTransform;
        Vector2 currentCoord = GetCoord();
        if (currentCoord != previousCoord)
        {
            previousCoord = currentCoord;
            UpdateTiles();
        }

        UpdateEdgeAnimations(currentCoord);
    }

    Vector2 GetCoord()
    {
        return new Vector2(
            Mathf.Floor(targetTransform.position.x / tileSize),
            Mathf.Floor(targetTransform.position.z / tileSize)
        );
    }

    void UpdateTiles()
    {
        HashSet<Vector2> activeDynamicCoords = new HashSet<Vector2>();
        HashSet<Vector2> activeStaticCoords = new HashSet<Vector2>();

        int outerDistance = viewDistance + staticPaddingDistance;

        for (int x = -outerDistance; x <= outerDistance; x++)
        {
            for (int z = -outerDistance; z <= outerDistance; z++)
            {
                Vector2 coord = previousCoord + new Vector2(x, z);
                float dist = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));

                if (dist <= viewDistance)
                {
                    activeDynamicCoords.Add(coord);
                    if (!dynamicTiles.ContainsKey(coord))
                    {
                        Vector3 pos = CoordToPosition(coord);
                        GameObject tile = Instantiate(oceanTilePrefab, pos, Quaternion.identity, transform);
                        tile.layer = this.gameObject.layer;
                        Waves waves = tile.GetComponent<Waves>();
                        if (waves != null)
                        {
                            waves.player = targetTransform;
                            //TODO chaneg
                            //waves.AnimateEdgesOnly = true;
                        }

                        dynamicTiles.Add(coord, tile);
                    }
                }
                else
                {
                    activeStaticCoords.Add(coord);
                    if (!staticTiles.ContainsKey(coord))
                    {
                        Vector3 pos = CoordToPosition(coord);
                        GameObject tile = Instantiate(staticOceanTilePrefab, pos, Quaternion.identity, transform);
                        tile.layer = this.gameObject.layer;
                        staticTiles.Add(coord, tile);
                    }
                }
            }
        }

        // Remove dynamic tiles
        List<Vector2> toRemoveDynamic = new List<Vector2>();
        foreach (var coord in dynamicTiles.Keys)
        {
            if (!activeDynamicCoords.Contains(coord))
                toRemoveDynamic.Add(coord);
        }
        foreach (var coord in toRemoveDynamic)
        {
            Destroy(dynamicTiles[coord]);
            dynamicTiles.Remove(coord);
        }

        // Remove static tiles
        List<Vector2> toRemoveStatic = new List<Vector2>();
        foreach (var coord in staticTiles.Keys)
        {
            if (!activeStaticCoords.Contains(coord))
                toRemoveStatic.Add(coord);
        }
        foreach (var coord in toRemoveStatic)
        {
            Destroy(staticTiles[coord]);
            staticTiles.Remove(coord);
        }
    }

    void UpdateEdgeAnimations(Vector2 centerCoord)
    {
        foreach (var pair in dynamicTiles)
        {
            var waves = pair.Value.GetComponent<Waves>();
            if (waves == null) continue;

            Vector2 coord = pair.Key;
            bool isCenter = (coord == centerCoord);

            var animationSettings = new WaveAnimationSettings
            {
                animatedEdge = !isCenter,
                edgeSettings = new List<EdgeSettings>()
            };

            foreach (EdgeType edgeType in Enum.GetValues(typeof(EdgeType)))
            {
                bool shouldAnimate = isCenter;

                if (!isCenter)
                {
                    Vector2 neighborOffset = edgeType switch
                    {
                        EdgeType.EdgeTop => new Vector2(-1, 0),
                        EdgeType.EdgeBottom => new Vector2(1, 0),
                        EdgeType.EdgeLeft => new Vector2(0, -1),
                        EdgeType.EdgeRight => new Vector2(0, 1),

                        EdgeType.CornerTopLeft => new Vector2(-1, -1),
                        EdgeType.CornerTopRight => new Vector2(-1, 1),
                        EdgeType.CornerBottomLeft => new Vector2(1, -1),
                        EdgeType.CornerBottomRight => new Vector2(1, 1),

                        _ => Vector2.zero
                    };

                    Vector2 neighborCoord = coord + neighborOffset;
                    shouldAnimate = !staticTiles.ContainsKey(neighborCoord);
                }

                animationSettings.edgeSettings.Add(new EdgeSettings
                {
                    edgeType = edgeType,
                    animatedEdge = shouldAnimate
                });
            }

            waves.SetAnimationSettings(animationSettings);

            if (isCenter)
                currentTile = waves;
        }
    }

    Vector3 CoordToPosition(Vector2 coord)
    {
        return new Vector3(
            (coord.x + 0.5f) * tileSize,
            oceanHeight,
            (coord.y + 0.5f) * tileSize
        );
    }

    public float GetWorldHeight(Vector3 worldPos)
    {
        if (currentTile != null)
            return currentTile.GetWorldHeight(worldPos);

        //Debug.LogWarning("Current tile is not set. Returning default height 0.");
        return 0f;
    }
    

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || currentTile == null) return;
        
        
        if (useMovementBarrier)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // semi-transparent red

            float sizes = (barrierDistance + 0.5f) * tileSize;
            Vector3 centers = new Vector3(0, oceanHeight + barrierHeight / 2, 0);
            Vector3 fullSize = new Vector3(sizes * 2, barrierHeight, sizes * 2);

            Gizmos.DrawWireCube(centers, fullSize);
        }
        
        Gizmos.color = Color.cyan;
        Vector3 center = currentTile.transform.position;
        Vector3 size = new Vector3(tileSize, 1f, tileSize);
        Gizmos.DrawWireCube(center, size);
    }
}
