using Helpers.Util;
using UnityEngine;

public class NpcIndicator : MonoBehaviour
{
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

        if (npcIndicator == null || !npcIndicator.isActiveAndEnabled || GameStateManager.Instance.GetGameState() == GameStates.Dialogue || npc.NpcData.NpcState == NpcStates.Finished)
        {
            return;
        }
    }

    private void InstantiateAwarenessSlider(int completedQuests, Npc npc)
    {
        if(this.npc != npc)
        {
            return;
        }

        Vector3 topPosition = ColliderUtil.GetTopPosition(npc.gameObject);

        GameObject newAwarenessPrefab = Instantiate(awarenessPrefab, worldCanvas);
        newAwarenessPrefab.GetComponent<UIAwarenessSliderItem>().Setup(completedQuests, npc.MaxCompletedQuests);
        newAwarenessPrefab.transform.position = topPosition + Vector3.up * (2);
    }
}
