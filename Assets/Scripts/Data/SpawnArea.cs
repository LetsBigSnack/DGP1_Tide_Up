using System.Collections.Generic;
using UnityEngine;

public enum SpawnAreaType
{
    Beach,
    City,
    Forest,
    Mountain
}
public class SpawnArea : MonoBehaviour
{
    public SpawnAreaType type;
    public bool shouldSpawn;

    public List<GameObject> areaSpawnedTrash = new List<GameObject>();

    public int TrashCount => areaSpawnedTrash.Count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            shouldSpawn = false;
        if (other.CompareTag("Trash"))
            Debug.Log("New trash in " + type + ", total = " + TrashCount);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            shouldSpawn = true;
        if (other.CompareTag("Trash"))
            Debug.Log("Trash left " + type + ", new total = " + TrashCount);
    }

    public void RemoveTrashAreaNulls()
    {
        areaSpawnedTrash.RemoveAll(item => item == null);
    }
}
