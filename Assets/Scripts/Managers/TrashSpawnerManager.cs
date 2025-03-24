using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject trashItem;
    [SerializeField] private SpawnArea[] areas;
    
    [SerializeField] private int maxTrashPerArea = 5;
    //[SerializeField] private int maxTrashTotal = 20;
    private int trashCount;
    private List<GameObject> spawnedTrash = new List<GameObject>();

    private Coroutine spawnRoutine;

    [SerializeField] private float trashSpawnInterval = 2f;
    [SerializeField] private bool isSpawningTrash = true;

    private void Start()
    {
        StartSpawningTrash();
    }

    public void SpawnTrash()
    {
        foreach (SpawnArea area in areas)
        {
            area.RemoveTrashAreaNulls();
            RemoveNulls();
            trashCount = area.TrashCount;

            if (area.shouldSpawn && trashCount < maxTrashPerArea)
            {
                Vector3 randomPosition = area.transform.position + new Vector3(Random.Range(-4f, 4f), 1.5f, Random.Range(-4f, 4f));
                GameObject trash = Instantiate(trashItem, randomPosition, Quaternion.identity);
                area.areaSpawnedTrash.Add(trash);
                spawnedTrash.Add(trash);
            }
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
        spawnRoutine = StartCoroutine(SpawnTrashOverTime());
    }

    public void RemoveNulls()
    {
        spawnedTrash.RemoveAll(item => item == null);
    }
}
