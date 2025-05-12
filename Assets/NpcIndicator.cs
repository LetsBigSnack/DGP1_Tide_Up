using Helpers.Util;
using UnityEngine;

public class NpcIndicator : MonoBehaviour
{
    [SerializeField] private GameObject questionIndicator;
    [SerializeField] private GameObject exclamationIndicator;
    [SerializeField] private GameObject readyToDeliverIndicator;
    [SerializeField] private GameObject awarenessPrefab;
    [SerializeField] private Transform worldCanvas;

    private Npc npc;

    private void Start()
    {
        npc = GetComponent<Npc>();
        worldCanvas = UIInstance.Instance.WorldSpace;
    }

    private void OnEnable()
    {
        Npc.OnCompletedQuests += InstantiateAwarenessSlider;
    }

    private void OnDisable()
    {
        Npc.OnCompletedQuests -= InstantiateAwarenessSlider;
    }

    private void FixedUpdate()
    {
        NpcInteractable npcIndicator = npc.gameObject.GetComponent<NpcInteractable>();

        if (npcIndicator == null || !npcIndicator.isActiveAndEnabled || GameStateManager.Instance.GetGameState() == GameStates.Dialogue)
        {
            questionIndicator.SetActive(false);
            exclamationIndicator.SetActive(false);
            readyToDeliverIndicator.SetActive(false);
            return;
        }

        if (npc.NpcState == NpcStates.Intro)
        {
            exclamationIndicator.SetActive(true);
            return;
        }

        if (npc.CurrentQuest?.QuestState == QuestState.Offer &&
            npc.CompletedQuests < npc.MaxCompletedQuests &&
            GameStateManager.Instance.GetGameState() != GameStates.Dialogue)
        {
            exclamationIndicator.SetActive(true);
            questionIndicator.SetActive(false);
            readyToDeliverIndicator.SetActive(false);
            return;
        }

        if (npc.CurrentQuest?.QuestState == QuestState.InProgress &&
            npc.CompletedQuests < npc.MaxCompletedQuests &&
            GameStateManager.Instance.GetGameState() != GameStates.Dialogue)
        {
            if (InventoryManager.Instance.HasItem(npc.CurrentQuest.QuestItem))
            {
                readyToDeliverIndicator.SetActive(true);
                questionIndicator.SetActive(false);
                exclamationIndicator.SetActive(false);
            }
            else
            {
                questionIndicator.SetActive(true);
                readyToDeliverIndicator.SetActive(false);
                exclamationIndicator.SetActive(false);
            }
            return;
        }

        if (npc.NpcState == NpcStates.Finished)
        {
            questionIndicator.SetActive(false);
            exclamationIndicator.SetActive(false);
            readyToDeliverIndicator.SetActive(false);
            return;
        }

        questionIndicator.SetActive(false);
        exclamationIndicator.SetActive(false);
        readyToDeliverIndicator.SetActive(false);
    }

    private void InstantiateAwarenessSlider(int completedQuests, Npc npc)
    {
        if (this.npc != npc)
        {
            return;
        }

        Vector3 topPosition = ColliderUtil.GetTopPosition(npc.gameObject);

        GameObject newAwarenessPrefab = Instantiate(awarenessPrefab, worldCanvas);
        newAwarenessPrefab.GetComponent<UIAwarenessSliderItem>().Setup(completedQuests, npc.MaxCompletedQuests);
        newAwarenessPrefab.transform.position = topPosition + Vector3.up * (2);
    }
}