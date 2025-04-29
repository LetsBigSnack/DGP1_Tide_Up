using UnityEngine;
using UnityEngine.UI;

public class UIJournalBookMarkItem : MonoBehaviour
{
    [SerializeField] private JournalType type;
    [SerializeField] private Image image;
    [SerializeField] private bool isRight;
    [SerializeField] private UIJournalFiller currentParent;

    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private Sprite questSprite;
    [SerializeField] private Sprite recipeSprite;
    [SerializeField] private Sprite friendsSprite;
    [SerializeField] private Sprite calenderSprite;
    [SerializeField] private Sprite mapSprite;

    private Animator anim;

    public JournalType Type
    {
        get => type;
        set => type = value;
    }

    public Sprite Sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    public bool IsRight
    {
        get => isRight;
        set => isRight = value;
    }

    public UIJournalFiller CurrentParent
    {
        get => currentParent;
        set => currentParent = value;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        Setup();
    }

    public void Update()
    {
        if(UIJournalManager.Instance.GetCurrentState() == type)
        {
            anim.Play("Raised");
        }
    }

    public void OnClick()
    {
        if(UIJournalManager.Instance.GetCurrentState() == type)
        {
            return;
        }

        if (isRight)
        {
            UIBookMarkController.Instance.SwitchPosition(this);
        }
        UIJournalManager.Instance.SwitchState(type);
        currentParent.SwitchPosition();
    }

    private void Setup()
    {
        switch (type)
        {
            case JournalType.Inventory:
                image.sprite = inventorySprite;
                break;
            case JournalType.Calender:
                image.sprite = calenderSprite;
                break;
            case JournalType.Map:
                image.sprite = mapSprite;
                break;
            case JournalType.FriendBook:
                image.sprite = friendsSprite;
                break;
            case JournalType.Quests:
                image.sprite = questSprite;
                break;
            case JournalType.Recipies:
                image.sprite = recipeSprite;
                break;
        }
    }

    public void Remove()
    {
        anim.Play("End");
    }

    public void EndBookMark()
    {
        Destroy(gameObject);
    }


}
