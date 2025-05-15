using UnityEngine;

public class UIPauseButtons : MonoBehaviour
{

    public void OnHoverButton()
    {
        UIPauseMenuManager.Instance.BorderIcon.SetActive(true);
        UIPauseMenuManager.Instance.BorderIcon.transform.position = this.transform.position;
    }

    public void OnLeaveButton()
    {
        UIPauseMenuManager.Instance.BorderIcon.SetActive(false);
    }
}
