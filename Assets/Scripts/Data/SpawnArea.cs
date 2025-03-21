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

}
