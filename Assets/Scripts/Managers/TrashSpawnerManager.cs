using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class TrashPerArea
{
    [SerializeField] private SpawnAreaType spawnArea;
    [SerializeField] private GameObject[] items;
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
    
}


public class TrashSpawnerManager : MonoBehaviour
{
    [SerializeField] private SpawnArea[] areas;
    
    [SerializeField] private int maxTrashTotal = 20;
    private List<GameObject> _spawnedTrash = new List<GameObject>();

    private Coroutine _spawnRoutine;

    [SerializeField] private float trashSpawnInterval = 2f;
    [SerializeField] private bool isSpawningTrash = true;

    public static TrashSpawnerManager Instance;


    [SerializeField] private List<TrashPerArea> trashItemPerArea;
    
    
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

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        StartSpawningTrash();
    }

    public void SpawnTrash()
    {
        foreach (SpawnArea area in areas)
        {
            area.SpawnTrashInArea();
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

    public void StartSpawningTrash()
    {
        _spawnRoutine = StartCoroutine(SpawnTrashOverTime());
    }

    public void RemoveNulls()
    {
        _spawnedTrash.RemoveAll(item => item == null);
    }

    public GameObject GetTrashForArea(SpawnAreaType type)
    {
        TrashPerArea area = trashItemPerArea.FirstOrDefault(c => c.SpawnArea == type);

        if (area?.Items == null || area?.Items.Length == 0)
        {
            return null;
        }
        
        GameObject[] items = area.Items;
        
        return items[UnityEngine.Random.Range(0, items.Length)];
        
    }
}
