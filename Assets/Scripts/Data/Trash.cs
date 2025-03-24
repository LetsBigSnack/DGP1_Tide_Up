using System.Collections;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public string name;
    public bool itemQual;
    public TrashMaterial material1;
    public TrashMaterial material2 = null;

    private void OnEnable()
    {
        StartCoroutine(SelfDestruct());
    }


    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
