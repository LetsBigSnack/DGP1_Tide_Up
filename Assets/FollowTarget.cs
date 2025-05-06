using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    void LateUpdate()
    {
        if(target == null)
        {
            target = FindFirstObjectByType<Player>().gameObject.transform;
        }
        this.gameObject.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);    
    }
}
