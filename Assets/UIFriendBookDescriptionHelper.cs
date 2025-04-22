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
    [SerializeField] private Image awarenessImage;

    [Header("Awareness Sprites")]
    [SerializeField] private Sprite awarenessLvl_0;
    [SerializeField] private Sprite awarenessLvl_1;
    [SerializeField] private Sprite awarenessLvl_2;
    [SerializeField] private Sprite awarenessLvl_3;

    [Header("Awareness String Lvl")]
    [SerializeField] private string awarenessState_0;
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
    }

    public void SetupAwareness(NpcAwareness lvl)
    {
        switch (lvl)
        {
            case NpcAwareness.Low:
                awarenessImage.sprite = awarenessLvl_1;
                awareness.text = awarenessState_1;
                break;
            case NpcAwareness.Medium:
                awarenessImage.sprite = awarenessLvl_2;
                awareness.text = awarenessState_2;
                break;
            case NpcAwareness.High:
                awarenessImage.sprite = awarenessLvl_3;
                awareness.text = awarenessState_3;
                break;
        }
    }
}
