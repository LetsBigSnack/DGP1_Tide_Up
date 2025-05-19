using UnityEngine;

public class ForceXRotation : MonoBehaviour
{
    private void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
