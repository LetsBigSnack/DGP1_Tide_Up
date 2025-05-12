using UnityEngine;

public class UIEmotionIndicatorManager : MonoBehaviour
{
    public static UIEmotionIndicatorManager Instance;
    [SerializeField] private Transform worldspaceCanvas;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private float padding;

    [SerializeField] private Sprite angry;
    [SerializeField] private Sprite happy;
    [SerializeField] private Sprite sad;
    [SerializeField] private Sprite thinking;
    [SerializeField] private Sprite scared;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        worldspaceCanvas = UIInstance.Instance.WorldSpace;
    }

    private Sprite ReturnSpriteByEmotionType(Emotion type)
    {
        Sprite sprite = null;

        switch (type)
        {
            case Emotion.Angry:
                sprite = angry;
                break;
            case Emotion.Happy:
                sprite = happy;
                break;
            case Emotion.Sad:
                sprite = sad;
                break;
            case Emotion.Thinking:
                sprite = thinking;
                break;
            case Emotion.Surprised:
                sprite = scared;
                break;
        }
        return sprite;
    }

    public void HandleChangeIndicator(GameObject target, Emotion type)
    {
        if(target == null)
        {
            return;
        }
        Debug.Log("created the emotion");
        GameObject indicator = Instantiate(indicatorPrefab, worldspaceCanvas);
        indicator.GetComponent<UIEmotionIndicatorItem>().Setup(target, ReturnSpriteByEmotionType(type), padding);
    }
}
