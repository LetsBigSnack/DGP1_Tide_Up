using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum TutorialState
{
    Intro,
    ItemPickUp,
    Crafting,
    End
}


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TutorialState state;

    public Npc engineer;

    public GameObject waitScreenParent;
    public Image waitScreenBackground;
    public Image waitScreenLogo;
    public GameObject waitScreenContent;
    public Slider slider;

    public List<ItemInteractable> items = new List<ItemInteractable>();

    public Recycler recycler;

    public float fadeOutDuration;
    public float loadingDuration;

    public bool W;
    public bool A;
    public bool S;
    public bool D;


    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += CheckForTutorialItem;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= CheckForTutorialItem;
    }

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
        recycler.isEnabled = false;
        ToggleItems(false);
        StartCoroutine(FadeInWaitBG());
    }

    private void Update()
    {
        if(engineer.NpcState != NpcStates.Intro)
        {
            CheckForInput();
        }
    }

    private void ToggleItems(bool isEnabled)
    {
        foreach(ItemInteractable i in items)
        {
            i.IsInteractable = isEnabled;
        }
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            W = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            A = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            S = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            D = true;
        }

        if(state == TutorialState.Intro && W && A && S && D)
        {
            if (!InventoryManager.Instance.Items.Exists(t => t.ItemData.title == "Tutorial_Item"))
            {
                QuestItemInstance questItem = DataUtil.Instance.GetTutorialItem();
                InventoryManager.Instance.AddItem(questItem);
                ToggleItems(true);
                state = TutorialState.Crafting;
            }
        }
    }

    private void CheckForTutorialItem(List<ItemInstance> items)
    {
        if(items.Exists(t => t.ItemData.title == "DriftWood") && items.Exists(t => t.ItemData.title == "Can") && state == TutorialState.ItemPickUp)
        {
            if (!items.Exists(t => t.ItemData.title == "Tutorial_Item"))
            {
                QuestItemInstance questItem = DataUtil.Instance.GetTutorialItem();
                InventoryManager.Instance.AddItem(questItem);
                state = TutorialState.Crafting;
            }
        }

        if(!items.Exists(t => t.ItemData.title == "Tutorial_Item") && state == TutorialState.Crafting)
        {
            recycler.isEnabled = true;
        }

        if(items.Exists(t => t.ItemData.title == "Shovel") && state == TutorialState.Crafting)
        {
            if(!items.Exists(t=>t.ItemData.title == "Tutorial_Item"))
            {
                QuestItemInstance questItem = DataUtil.Instance.GetTutorialItem();
                InventoryManager.Instance.AddItem(questItem);
                state = TutorialState.End;
            }
        }
    }

    private IEnumerator FadeInWaitBG()
    {
        float elapsed = 0f;
        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            slider.maxValue = loadingDuration;
            slider.value = elapsed;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        waitScreenLogo.gameObject.SetActive(false);

        elapsed = 0f;

        Color colorBg = waitScreenBackground.color;

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
}
