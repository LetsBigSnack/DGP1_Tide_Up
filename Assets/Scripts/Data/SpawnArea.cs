using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SpawnAreaType
{
    Beach,
    City,
    Forest,
    Mountain
}
public class SpawnArea : MonoBehaviour
{
    [SerializeField] private SpawnAreaType type;
    [SerializeField] private bool shouldSpawn;
    [SerializeField] private float areaSpawnSize = 4f;

    [SerializeField] private float delayAfterExit = 5f;
    private Coroutine _reactivationRoutine;

    public List<GameObject> areaSpawnedTrash = new List<GameObject>();

    public int TrashCount => areaSpawnedTrash.Count;
    private HashSet<Transform> _playersInside = new HashSet<Transform>();


    private void OnTriggerEnter(Collider other)
    {
        Transform playerRoot = other.transform.root;
        if (!playerRoot.CompareTag("Player"))
            return;

        if (_playersInside.Add(playerRoot))
        {
            Debug.Log("Player ENTERED area: " + type);
            shouldSpawn = false;

            if (_reactivationRoutine != null)
            {
                StopCoroutine(_reactivationRoutine);
                _reactivationRoutine = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform playerRoot = other.transform.root;
        if (!playerRoot.CompareTag("Player"))
            return;

        if (_playersInside.Remove(playerRoot) && _playersInside.Count == 0)
        {
            Debug.Log("Player LEFT area: " + type);

            _reactivationRoutine = StartCoroutine(DelayedReactivate());
        }
    }
    private IEnumerator DelayedReactivate()
    {
        yield return new WaitForSeconds(delayAfterExit);
        shouldSpawn = true;
        Debug.Log("Area reactivated for spawning: " + type);
        _reactivationRoutine = null;
    }

    public void SpawnTrashInArea()
    {
        TrashSpawnerManager.Instance.RemoveNulls();

        if (shouldSpawn && TrashSpawnerManager.Instance.SpawnedTrash.Count < TrashSpawnerManager.Instance.MaxTrashTotal)
        {
            //TODO: rework height 
            Vector3 randomPosition = gameObject.transform.position + new Vector3(Random.Range(-areaSpawnSize, areaSpawnSize), gameObject.transform.position.y+0.55f, Random.Range(-areaSpawnSize, areaSpawnSize));

            if (CameraUtil.IsVisibleToCamera(randomPosition) && CameraUtil.HasLineOfSight(randomPosition))
                return;

            GameObject trashItem = TrashSpawnerManager.Instance.GetTrashForArea(type);
            GameObject trash = Instantiate(trashItem, randomPosition, Quaternion.identity, transform);
            TrashSpawnerManager.Instance.SpawnedTrash.Add(trash);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (shouldSpawn)
        {
            Gizmos.color = Color.green;
        }
        
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSpawnSize*2, 1, areaSpawnSize*2));
    }
}
