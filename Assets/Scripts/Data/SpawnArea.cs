using System.Collections;
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

    [SerializeField] private float delayAfterExit = 5f;
    private Coroutine reactivationRoutine;

    public List<GameObject> areaSpawnedTrash = new List<GameObject>();

    public int TrashCount => areaSpawnedTrash.Count;
    private HashSet<Transform> playersInside = new HashSet<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        Transform playerRoot = other.transform.root;
        if (!playerRoot.CompareTag("Player"))
            return;

        if (playersInside.Add(playerRoot))
        {
            Debug.Log("Player ENTERED area: " + type);
            shouldSpawn = false;

            if (reactivationRoutine != null)
            {
                StopCoroutine(reactivationRoutine);
                reactivationRoutine = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform playerRoot = other.transform.root;
        if (!playerRoot.CompareTag("Player"))
            return;

        if (playersInside.Remove(playerRoot) && playersInside.Count == 0)
        {
            Debug.Log("Player LEFT area: " + type);

            reactivationRoutine = StartCoroutine(DelayedReactivate());
        }
    }
    private IEnumerator DelayedReactivate()
    {
        yield return new WaitForSeconds(delayAfterExit);
        shouldSpawn = true;
        Debug.Log("Area reactivated for spawning: " + type);
        reactivationRoutine = null;
    }

    public void RemoveTrashAreaNulls()
    {
        areaSpawnedTrash.RemoveAll(item => item == null);
    }
}
