using Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIQuestIndicatorRepresentation
{
    [SerializeField] private NpcData data;
    [SerializeField] private QuestState state;
    [SerializeField] GameObject indicatorObject;
 
    public NpcData Data
    {
        get => data;
        set => data = value;
    }

    public QuestState State
    {
        get => state;
        set => state = value;
    }

    public GameObject Indicator
    {
        get => indicatorObject;
        set => indicatorObject = value;
    }

    public UIQuestIndicatorRepresentation(NpcData data, QuestState state, GameObject indicator)
    {
        this.data = data;
        this.state = state;
        this.indicatorObject = indicator;
    }
}

public class UIQuestIndicatorManager : MonoBehaviour
{
    public static UIQuestIndicatorManager Instance;
    [SerializeField] private Transform worldspaceCanvas;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private float padding;

    [SerializeField] private Sprite start;
    [SerializeField] private Sprite ongoing;
    [SerializeField] private Sprite readyToFinish;

    [SerializeField] private List<UIQuestIndicatorRepresentation> representations = new List<UIQuestIndicatorRepresentation>();

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

    private UIQuestIndicatorRepresentation RepresentationExistsByNpcData(NpcData data)
    {
        return representations.Where(x => x.Data == data).FirstOrDefault();
    }

    private bool IndicatorExistsByQuestState(UIQuestIndicatorRepresentation representation, QuestState state)
    {
        return representations.Exists(x => x == representation && x.State == state);
    }

    private GameObject ReturnIndicatorByNpcData(NpcData data)
    {
        return representations.Where(x => x.Data == data).FirstOrDefault().Indicator;
    }

    private Sprite ReturnSpriteByQuestState(QuestState state, bool isReadyToFinish)
    {
        if (isReadyToFinish)
        {
            return readyToFinish;
        }

        switch (state)
        {
            case QuestState.Offer:
                return start;
            case QuestState.InProgress:
                return ongoing;
        }

        return null;
    }

    public void HandleChangeIndicator(NpcData data, GameObject target, bool isReadyToFinish = false)
    {
        if(data == null || data.CurrentQuest == null)
        {
            return;
        }

        UIQuestIndicatorRepresentation representation = RepresentationExistsByNpcData(data);

        if(representation != null && IndicatorExistsByQuestState(representation, data.CurrentQuest.QuestState))
        {
            return;
        }

        if(representation != null && !IndicatorExistsByQuestState(representation, data.CurrentQuest.QuestState))
        {
            GameObject current = ReturnIndicatorByNpcData(data); 
            current.GetComponent<UIQuestIndicatorItem>().Setup(target, ReturnSpriteByQuestState(data.CurrentQuest.QuestState, isReadyToFinish), padding);
            return;
        }

        GameObject indicator = Instantiate(indicatorPrefab, worldspaceCanvas);
        indicator.GetComponent<UIQuestIndicatorItem>().Setup(target, ReturnSpriteByQuestState(data.CurrentQuest.QuestState, isReadyToFinish), padding);
        UIQuestIndicatorRepresentation newRepresentation = new UIQuestIndicatorRepresentation(data, data.CurrentQuest.QuestState, indicator);
        representations.Add(newRepresentation);
    }
}
