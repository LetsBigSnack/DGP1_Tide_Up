using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;
    public bool isActive = false;

    void LateUpdate()
    {
        if (isActive) { 
        transform.Rotate(rotation * Time.deltaTime, Space.Self);
        }
    }
}
