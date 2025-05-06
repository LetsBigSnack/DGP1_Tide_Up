using UnityEngine;
using System.Collections.Generic;

public class OceanFloorManager : MonoBehaviour
{
    public Transform _targetTransform;
    public GameObject oceanFloorTilePrefab;
    public int viewDistance = 3; 
    public float tileSize = 50f;
    [SerializeField] private float oceanDepth = -10f;

    private Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();
    private Vector2 previousPlayerCoord;

    void Start()
    {
        _targetTransform = GameStateManager.Instance.TargetTransform;

        previousPlayerCoord = PlayerCoord();
        UpdateTiles();
    }

    void Update()
    {
        _targetTransform = GameStateManager.Instance.TargetTransform;
        Vector2 currentPlayerCoord = PlayerCoord();

        if (currentPlayerCoord != previousPlayerCoord)
        {
            previousPlayerCoord = currentPlayerCoord;
            UpdateTiles();
        }
    }

    Vector2 PlayerCoord()
    {
        return new Vector2(
            Mathf.Floor(_targetTransform.position.x / tileSize),
            Mathf.Floor(_targetTransform.position.z / tileSize));
    }

    void UpdateTiles()
    {
        HashSet<Vector2> currentCoords = new HashSet<Vector2>();

        // Instantiate missing tiles around player
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 coord = previousPlayerCoord + new Vector2(x, z);
                currentCoords.Add(coord);

                if (!tiles.ContainsKey(coord))
                {
                    Vector3 pos = CoordToPosition(coord);
                    GameObject tile = Instantiate(oceanFloorTilePrefab, pos, Quaternion.identity, transform);
                    tiles.Add(coord, tile);
                }
            }
        }

        // Destroy tiles out of range
        List<Vector2> coordsToRemove = new List<Vector2>();

        foreach (var existingCoord in tiles.Keys)
        {
            if (!currentCoords.Contains(existingCoord))
                coordsToRemove.Add(existingCoord);
        }

        foreach (var coord in coordsToRemove)
        {
            Destroy(tiles[coord]);
            tiles.Remove(coord);
        }
    }

    Vector3 CoordToPosition(Vector2 coord)
    {
        return new Vector3(coord.x * tileSize, oceanDepth, coord.y * tileSize);
    }
}
