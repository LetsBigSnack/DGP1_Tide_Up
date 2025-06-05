using System.Collections;
using UnityEngine;

public class UIStartButtons : MonoBehaviour
{
    public void OnHoverButton()
    {
        StartCoroutine(DelayedHover());
    }

    private IEnumerator DelayedHover()
    {
        yield return null; // wait for end of frame UI layout updates

        UIStartSceneManager.Instance.BorderIcon.transform.position = transform.position;
        UIStartSceneManager.Instance.BorderIcon.SetActive(true);
    }

    public void OnLeaveButton()
    {
        UIStartSceneManager.Instance.BorderIcon.SetActive(false);
    }
}
