using UnityEngine;

public class UIStartButtons : MonoBehaviour
{
    public void OnHoverButton()
    {
        UIStartSceneManager.Instance.BorderIcon.SetActive(true);
        UIStartSceneManager.Instance.BorderIcon.transform.position = this.transform.position;
    }

    public void OnLeaveButton()
    {
        UIStartSceneManager.Instance.BorderIcon.SetActive(false);
    }
}
