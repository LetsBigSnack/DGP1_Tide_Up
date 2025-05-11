using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeScreenHelper : MonoBehaviour
{
    [SerializeField] private float loadingDuration;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private Slider slider;
    [SerializeField] private Image waitScreenLogo;
    [SerializeField] private Image waitScreenBackground;
    [SerializeField] private GameObject waitScreenParent;


    public static UIFadeScreenHelper Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Coroutine StartTransition()
    {
        return StartCoroutine(SceneTransitionStart());
    }

    public Coroutine EndTransition()
    {
        return StartCoroutine(SceneTransitionEnd());
    }

    private IEnumerator SceneTransitionEnd()
    {
        Color colorBg = SetBGColor(1f);
        slider.gameObject.SetActive(false);
        waitScreenLogo.gameObject.SetActive(false); 
        waitScreenParent.SetActive(true);

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            colorBg.a = alpha;
            waitScreenBackground.color = colorBg;
            yield return null;
        }

        waitScreenParent.SetActive(false);
    }

    public IEnumerator SceneTransitionStart()
    {
        GameStateManager.Instance.SetGameState(GameStates.SceneTransition);
        Color colorBg = SetBGColor(0f); 

        slider.gameObject.SetActive(false);
        waitScreenLogo.gameObject.SetActive(false);

        waitScreenParent.SetActive(true);

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            colorBg.a = alpha;
            waitScreenBackground.color = colorBg;
            yield return null;
        }

        slider.gameObject.SetActive(true);
        waitScreenLogo.gameObject.SetActive(true);

        elapsed = 0f;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            slider.maxValue = loadingDuration;
            slider.value = elapsed;
            yield return null;
        }
    }

    private Color SetBGColor(float alpha)
    {
        Color colorBg = waitScreenBackground.color;
        colorBg.a = alpha;

        return colorBg;
    }

}
