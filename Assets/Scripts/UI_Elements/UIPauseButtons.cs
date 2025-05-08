using UnityEngine;

public class UIPauseButtons : MonoBehaviour
{

    public void OnHoverButton()
    {
        Debug.Log("Pointer entered");
        UIPauseMenuManager.Instance.BorderIcon.SetActive(true);
        UIPauseMenuManager.Instance.BorderIcon.transform.position = this.transform.position;
    }

    public void OnLeaveButton()
    {
        Debug.Log("Pointer left");
        UIPauseMenuManager.Instance.BorderIcon.SetActive(false);
    }
}
