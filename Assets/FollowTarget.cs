using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    void LateUpdate()
    {
        if(GameStateManager.Instance.GetGameState() != GameStates.PlayingBoat)
        {
            target = FindFirstObjectByType<Player>().gameObject.transform;
        }
        else
        {
            target = FindFirstObjectByType<Boat>().gameObject.transform;
        }
        this.gameObject.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);    
    }
}
