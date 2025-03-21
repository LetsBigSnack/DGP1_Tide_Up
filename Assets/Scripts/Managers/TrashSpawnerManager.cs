using UnityEngine;

public class TrashSpawnerManager : MonoBehaviour
{
    public GameObject trashItem;
    public SpawnArea[] areas;


    public void SpawnTrash()
    {
        foreach (SpawnArea area in areas)
        {
            if(area.shouldSpawn)
            {
                Vector3 randomPosition = area.transform.position + new Vector3(Random.Range(-4f, 4f), 0.1f, Random.Range(-4f, 4f));
                Instantiate(trashItem, randomPosition, Quaternion.identity);
            }
        }
    }

}
