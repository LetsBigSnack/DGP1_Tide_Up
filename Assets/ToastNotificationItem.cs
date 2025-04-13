using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastNotificationItem : MonoBehaviour
{
    [Header("ToastSetup")]
    [SerializeField] private ToastType type;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image image;
    [SerializeField] private float timeUntilDestroy;

    private Animator _toastAnim;
    private bool _animationIsPlaying = false;

    private void Awake()
    {
        _toastAnim = GetComponent<Animator>();
        StartCoroutine(EndToast());
    }

    public void SetToast(string titleText ="", string descriptionText = "", Sprite sprite = null)
    {
        if(image != null)
        {
            image.sprite = sprite;
        }

        if(title != null)
        {
            title.text = titleText;
        }
        
        if(description != null)
        {
            description.text = descriptionText;
        }
    }

    public void PlayEndAnimation()
    {
        if (_animationIsPlaying) return;
        _toastAnim.Play("EndToast");
    }

    public void DestroyToast()
    {
        Destroy(gameObject);
    }

    private IEnumerator EndToast()
    {
        yield return new WaitForSeconds(timeUntilDestroy);
        UI_ToastManager.Instance.RemoveFromList(type, gameObject);
        PlayEndAnimation();
    }

}
