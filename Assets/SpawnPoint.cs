using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] public Scenes linkedScene;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
