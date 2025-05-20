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
    Mountain,
    Water
}
public class SpawnArea : MonoBehaviour
{
    [SerializeField] private SpawnAreaType type;
    [SerializeField] private bool shouldSpawn;
    [SerializeField] private float areaSpawnSizeX = 4f;
    [SerializeField] private float areaSpawnSizeY = 4f;
    [SerializeField] private float delayAfterExit = 5f;
    private Coroutine _reactivationRoutine;

    public List<GameObject> areaSpawnedTrash = new List<GameObject>();

    public int TrashCount => areaSpawnedTrash.Count;
    private HashSet<Transform> _playersInside = new HashSet<Transform>();

    public SpawnAreaType Type => type;
    
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
            Vector3 randomOffset = new Vector3(
                Random.Range(-areaSpawnSizeX, areaSpawnSizeX),
                10f,
                Random.Range(-areaSpawnSizeY, areaSpawnSizeY)
            );

            Vector3 rayStart = gameObject.transform.position + randomOffset;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, TrashSpawnerManager.Instance.GroundLayer))
            {
                Vector3 spawnPoint = hit.point;
                
                if (CameraUtil.IsVisibleToCamera(spawnPoint) && CameraUtil.HasLineOfSight(spawnPoint))
                    return;
                
                bool isIntersecting = Physics.CheckSphere(spawnPoint + Vector3.up * 0.1f, TrashSpawnerManager.Instance.CheckRadius, TrashSpawnerManager.Instance.InteractableLayer);
                if (isIntersecting)
                    return;

                float randomRotation = Random.Range(0f, 360f);
                GameObject trashItem = TrashSpawnerManager.Instance.GetTrashForArea(type);
                GameObject trash = Instantiate(trashItem, spawnPoint + Vector3.up * 0.5f, Quaternion.identity, transform);
                trash.transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
                TrashSpawnerManager.Instance.SpawnedTrash.Add(trash);
            }
        }
    }
    
    public void SpawnWaterTrashInArea()
    {
        TrashSpawnerManager.Instance.RemoveNulls();
        
        if (shouldSpawn && TrashSpawnerManager.Instance.SpawnedWaterTrash.Count < TrashSpawnerManager.Instance.MaxWaterTrashTotal)
        {
            Vector3 offset = new Vector3(
                Random.Range(-areaSpawnSizeX, areaSpawnSizeX),
                OceanManager.Instance.oceanHeight,
                Random.Range(-areaSpawnSizeY, areaSpawnSizeY)
            );
            
            Vector3 spawnPoint = gameObject.transform.position + offset;
            spawnPoint.y = OceanManager.Instance.oceanHeight;
            
            bool isIntersecting = Physics.CheckSphere(spawnPoint + Vector3.up * 0.1f, TrashSpawnerManager.Instance.CheckRadius, TrashSpawnerManager.Instance.InteractableLayer);
            if (isIntersecting)
                return;

            float randomRotation = Random.Range(0f, 360f);
            GameObject trashItem = TrashSpawnerManager.Instance.GetTrashForArea(type);
            GameObject trash = Instantiate(trashItem, spawnPoint + Vector3.up * 0.5f, Quaternion.identity, transform);
            trash.transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
            TrashSpawnerManager.Instance.SpawnedWaterTrash.Add(trash);
        }
    }

    private void OnDrawGizmos()
    {

        
        
        Gizmos.color = Color.yellow;

        if (shouldSpawn)
        {
            Gizmos.color = Color.green;
        }
        
#if UNITY_EDITOR
        switch (type)
        {
            case SpawnAreaType.Beach:
                Gizmos.color = Color.yellow;
                break;
            case SpawnAreaType.City:
                Gizmos.color = Color.gray;
                break;
            case SpawnAreaType.Mountain:
                Gizmos.color = new Color(159f/255f, 75f/255f, 1f/255f, 1f);
                break;
            case SpawnAreaType.Forest:
                Gizmos.color = Color.green;
                break;
            case SpawnAreaType.Water:
                Gizmos.color = Color.blue;
                break;
        }
        
#endif

        
        
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSpawnSizeX*2, 1, areaSpawnSizeY*2));
    }
}
