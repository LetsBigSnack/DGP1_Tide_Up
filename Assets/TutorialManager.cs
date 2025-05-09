using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

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

    public List<ItemInteractable> items = new List<ItemInteractable>();

    public Recycler recycler;

    public bool W = false;
    public bool A = false;
    public bool S = false;
    public bool D = false;

    public bool itemsAreEnabled = false;

    public GameObject uiTutorialParent;

    public GameObject buttonA;
    public GameObject buttonW;
    public GameObject buttonS;
    public GameObject buttonD;
    public GameObject buttonE;

    public GameObject tutorialDoor;

    public bool tutorialIntroEnded;

    public Scenes scene;

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
        recycler.enabled = false;
        engineer.GetComponent<NpcInteractable>().enabled = false;
        ToggleItems(false);

        StartCoroutine(StartTutorial());
    }

    private void Update()
    {
        if(state == TutorialState.ItemPickUp && engineer.CurrentQuest?.QuestState == QuestState.InProgress && !itemsAreEnabled)
        {
            ToggleItems(true);
        }

        if (!engineer.GetComponent<NpcInteractable>().isActiveAndEnabled && GameStateManager.Instance.GetGameState() != GameStates.Dialogue && !AllButtonsDone())
        {
            CheckForInput();
        }

        if (Input.GetKeyDown(KeyCode.E) && !tutorialIntroEnded)
        {
            ProceedDialogue();
            buttonE.SetActive(false);
            if (engineer.NpcState != NpcStates.Intro)
            {
                Debug.Log("notpossiblenexttime");
                tutorialIntroEnded = true;
                buttonA.SetActive(true);
                buttonD.SetActive(true);
                buttonW.SetActive(true);
                buttonS.SetActive(true);
            }
        }
    }

    private IEnumerator StartTutorial()
    {
        GameStateManager.Instance.SetGameState(GameStates.Dialogue);
        yield return UIFadeScreenHelper.Instance.EndTransition();
        buttonE.SetActive(true);
        ProceedDialogue();
    }

    private bool AllButtonsDone()
    {
        return A && W && S && D;
    }

    private void ProceedDialogue()
    {
        if(engineer.NpcState == NpcStates.Quest)
        {
            NpcDialogueManager.Instance.EndDialogue();
        }
        NpcDialogueManager.Instance.StartDialogue(engineer);
        NpcDialogueManager.Instance.InteractDialogue();
    }

    private void ToggleItems(bool isEnabled)
    {
        if(items == null)
        {
            return;
        }

        foreach(ItemInteractable i in items)
        {
            i.enabled = isEnabled;
        }

        itemsAreEnabled = isEnabled;
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            W = true;
            buttonW.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            A = true;
            buttonA.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            S = true;
            buttonS.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            D = true;
            buttonD.SetActive(false);
        }

        if (state == TutorialState.Intro && AllButtonsDone())
        {
            if (!InventoryManager.Instance.Items.Exists(t => t.ItemData.title == "Tutorial_Item"))
            {
                state = TutorialState.ItemPickUp;
                engineer.GetComponent<NpcInteractable>().enabled = true;
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

        if(!items.Exists(t => t.ItemData.title == "Tutorial_Item") && state == TutorialState.Crafting && engineer.CurrentQuest.QuestState == QuestState.InProgress)
        {
            recycler.enabled = true;
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

        if(!items.Exists(t => t.ItemData.title == "Tutorial_Item") && state == TutorialState.End)
        {
            tutorialDoor.SetActive(true);
        }
    }

    public void LeaveTutorial()
    {
        LocationManager.Instance.TravelToScene(scene);
    }
}
