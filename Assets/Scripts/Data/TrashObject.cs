using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashObject : MonoBehaviour
{
    public TrashData data;

    private void OnEnable()
    {
        StartCoroutine(SelfDestruct());
    }


    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(25f);
        Destroy(gameObject);
    }
}
