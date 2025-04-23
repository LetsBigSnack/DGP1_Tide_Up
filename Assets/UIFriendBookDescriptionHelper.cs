using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendBookDescriptionHelper : MonoBehaviour
{
    public static UIFriendBookDescriptionHelper Instance;

    [Header("Page Components")]
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI home;
    [SerializeField] private TextMeshProUGUI vibe;
    [SerializeField] private TextMeshProUGUI birthday;
    [SerializeField] private TextMeshProUGUI mbti;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI awareness;
    [SerializeField] private TextMeshProUGUI colour;
    [SerializeField] private TextMeshProUGUI food;
    [SerializeField] private TextMeshProUGUI animal;
    [SerializeField] private TextMeshProUGUI thing;
    [SerializeField] private Slider awarenessSlider;

    [Header("Awareness String Lvl")]
    [SerializeField] private string awarenessState_1;
    [SerializeField] private string awarenessState_2;
    [SerializeField] private string awarenessState_3;

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

    public void Setup(Npc npc)
    {
        this.portrait.sprite = npc.Portrait;
        this.npcName.text = npc.NpcName;
        this.home.text = "Home: " + npc.HomeDetails;
        this.vibe.text = "Vibe: " + npc.Vibe;
        this.birthday.text ="Birthday: " +  npc.Birthday;
        this.mbti.text ="Mbti: " + npc.Mbti;
        this.description.text = npc.Description;
        this.colour.text = npc.FavColour;
        this.food.text = npc.FavFood;
        this.animal.text = npc.FavAnimal;
        this.thing.text = npc.FavThing;

        SetupAwareness(npc.NpcAwareness);
        SetupSlider(npc);
    }

    public void SetupAwareness(NpcAwareness lvl)
    {
        switch (lvl)
        {
            case NpcAwareness.Low:
                awareness.text = awarenessState_1;
                break;
            case NpcAwareness.Medium:
                awareness.text = awarenessState_2;
                break;
            case NpcAwareness.High:
                awareness.text = awarenessState_3;
                break;
        }
    }

    public void SetupSlider(Npc npc)
    {
        awarenessSlider.maxValue = npc.MaxCompletedQuests;
        awarenessSlider.value = npc.CompletedQuests;
    }
}
