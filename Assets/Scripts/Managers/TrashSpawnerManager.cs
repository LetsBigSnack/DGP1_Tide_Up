using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[Serializable]
public class TrashPerArea
{
    [SerializeField] private SpawnAreaType spawnArea;
    [SerializeField] private GameObject[] items;
    [SerializeField] private GameObject[] hills;
    [SerializeField] private float hillPercentage;
    public SpawnAreaType SpawnArea
    {
        get => spawnArea;
        set => spawnArea = value;
    }

    public GameObject[] Items
    {
        get => items;
        set => items = value;
    }
    
    public GameObject[] Hills
    {
        get => hills;
        set => hills = value;
    }
    
    public float HillPercentage
    {
        get => hillPercentage;
        set => hillPercentage = value;
    }
    
}


public class TrashSpawnerManager : MonoBehaviour
{
    [SerializeField] private SpawnArea[] areas;
    
    [SerializeField] private int maxTrashTotal = 20;
    private List<GameObject> _spawnedTrash = new List<GameObject>();

    private Coroutine _spawnRoutine;
    private Coroutine _waterSpawnRoutine;

    [SerializeField] private float trashSpawnInterval = 2f;
    [SerializeField] private bool isSpawningTrash = true;
    [SerializeField] private bool isSpawningWaterTrash = true;
    
    public static TrashSpawnerManager Instance;


    [SerializeField] private List<TrashPerArea> trashItemPerArea;
    
    
    [SerializeField] private int waterMaxTrashTotal = 100;
    private List<GameObject> _waterSpawnedTrash = new List<GameObject>();
    [SerializeField] private float waterTrashSpawnInterval = 2f;
    [SerializeField] private SpawnArea[] waterAreas;
    [SerializeField] private List<AllowedAreas> allowedAreas;
    
    
    public List<GameObject> SpawnedTrash
    {
        get { return _spawnedTrash; }
        set { _spawnedTrash = value; }
    }
    public int MaxTrashTotal
    {
        get { return maxTrashTotal; }
        set { maxTrashTotal = value; }
    }
    
    public int MaxWaterTrashTotal
    {
        get { return waterMaxTrashTotal; }
        set { waterMaxTrashTotal = value; }
    }

    public List<GameObject> SpawnedWaterTrash
    {
        get { return _waterSpawnedTrash; }
        set { _waterSpawnedTrash = value; }
    }

    public LayerMask InteractableLayer;
    public LayerMask GroundLayer;
    public LayerMask WaterLayer;
    public float CheckRadius = 1f;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartSpawningTrash();
        StartSpawningWaterTrash();
    }

    public void SpawnTrash()
    {
        AllowedAreas tempAllowedAreas =
            allowedAreas.Find(c => c.islandState == EnvironmentManager.Instance.GetStateOfIsland(10));
        
        foreach (SpawnArea area in areas)
        {
            if (tempAllowedAreas.areas.Contains(area.Type))
            {
                area.SpawnTrashInArea();
            }
        }
    }
    
    public void WaterSpawnTrash()
    {
        foreach (SpawnArea area in waterAreas)
        {
            area.SpawnWaterTrashInArea();
        }
    }

    IEnumerator SpawnTrashOverTime()
    {
        while (isSpawningTrash)
        {
            SpawnTrash();
            yield return new WaitForSeconds(trashSpawnInterval);
        }
    }
    
    IEnumerator SpawnWaterTrashOverTime()
    {
        while (isSpawningWaterTrash)
        {
            WaterSpawnTrash();
            yield return new WaitForSeconds(trashSpawnInterval);
        }
    }

    public void StartSpawningTrash()
    {
        _spawnRoutine = StartCoroutine(SpawnTrashOverTime());
    }
    
    public void StartSpawningWaterTrash()
    {
        _waterSpawnRoutine = StartCoroutine(SpawnWaterTrashOverTime());
    }

    public void RemoveNulls()
    {
        _spawnedTrash.RemoveAll(item => item == null);
        _waterSpawnedTrash.RemoveAll(item => item == null);
    }

    public GameObject GetTrashForArea(SpawnAreaType type)
    {
        TrashPerArea area = trashItemPerArea.FirstOrDefault(c => c.SpawnArea == type);
        
        if (area?.Items == null || area?.Items.Length == 0)
        {
            return null;
        }
        
        GameObject[] items;
        if (area.Hills != null && area.Hills?.Length > 0 && area.HillPercentage > Random.Range(0, 1.0f))
        {
            items = area.Hills;
        }
        else
        {
            items = area.Items;
        }
        return items[UnityEngine.Random.Range(0, items.Length)];
    }
}

[Serializable]
internal class AllowedAreas
{
    public EnvironmentState islandState;
    public List<SpawnAreaType> areas = new List<SpawnAreaType>();
}
