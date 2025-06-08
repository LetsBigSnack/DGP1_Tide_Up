using UnityEngine;
using UnityEngine.UI;

public class UIJournalBookMarkItem : MonoBehaviour
{
    [SerializeField] private JournalType type;
    [SerializeField] private Image image;
    [SerializeField] private bool isRight;
    [SerializeField] private UIJournalBookMarkItem counterPart;

    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private Sprite questSprite;
    [SerializeField] private Sprite recipeSprite;
    [SerializeField] private Sprite friendsSprite;
    [SerializeField] private Sprite calenderSprite;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private Vector3 baseY;
    [SerializeField] private Vector3 raisedY;

    [SerializeField] private UIJournalBookMarkItem prev;
    [SerializeField] private UIJournalBookMarkItem next;

    private Button _button;

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

    public UIJournalBookMarkItem Previous
    {
        get => prev;
    }

    public UIJournalBookMarkItem Next
    {
        get => next;
    }

    private void Start()
    {
        _button = GetComponentInChildren<Button>();
        anim = GetComponent<Animator>();
        Setup();
    }

    private void OnEnable()
    {
        _button = GetComponentInChildren<Button>();
        SetButtonNavigation(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += SetButtonNavigation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= SetButtonNavigation;
    }

    private void SetButtonNavigation(DeviceType type)
    {
        Navigation nav = _button.navigation;

        if (type != DeviceType.Keyboard && type != DeviceType.Mouse)
        {
            _button.interactable = false;
            nav.mode = Navigation.Mode.None;
            _button.navigation = nav;
            return;
        }
        _button.interactable = true;
        nav.mode = Navigation.Mode.Automatic;
        _button.navigation = nav;
    }

    public void OnClick()
    {
        UIJournalManager.Instance.SwitchState(type);
        SoundManager.Instance.PlaySFX("Click");
    }

    public void OnSelected()
    {
        anim.SetBool("Selected", true);
        SoundManager.Instance.PlaySFX("Click");
    }

    public void OnDeselect()
    {
        anim.SetBool("Selected", false);
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

    private void Update()
    {
        RaiseItem();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Activate()
    {
        counterPart.CloseLinkedItems();
        if(type != JournalType.Inventory && !isRight && UIJournalManager.Instance.GetCurrentState() == type)
        {
            counterPart.SetActive(false);
            gameObject.SetActive(true);
        }
        OpenLinkedItems();
    }

    public void RaiseItem()
    {
        if (ShouldBeRaised())
        {
            anim.SetBool("raised", true);
            
            UIBookMarkController.Instance.SetCurrentBookMark(this);
            return;
        }
        anim.SetBool("raised", false);
    }

    private bool ShouldBeRaised()
    {
        return !isRight && UIJournalManager.Instance.GetCurrentState() == type;
    }

    private void CloseLinkedItems()
    { 
        if (isRight && prev != null && prev.gameObject.activeInHierarchy)
        {
            prev.CloseLinkedItems();
            prev.SetActive(false);
        }
        else if(!isRight && next != null && next.gameObject.activeInHierarchy)
        {
            next.CloseLinkedItems();
            next.SetActive(false);
        }
    }

    public void OpenLinkedItems()
    {

        if (isRight && next != null && !next.gameObject.activeInHierarchy)
        {
            next.SetActive(true);
            next.OpenLinkedItems();
        }
        else if (!isRight && prev != null && !prev.gameObject.activeInHierarchy)
        {
            prev.SetActive(true);
            prev.OpenLinkedItems();
        }  
    }
}
