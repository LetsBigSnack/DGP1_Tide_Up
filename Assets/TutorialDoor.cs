using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            TutorialManager.Instance.LeaveTutorial();
        }
    }
}
