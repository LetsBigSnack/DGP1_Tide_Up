using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    [SerializeField] private Transform target;

    void LateUpdate()
    {
        this.gameObject.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);    
    }
}
