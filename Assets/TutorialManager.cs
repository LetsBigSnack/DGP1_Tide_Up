using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private TutorialState state;

    [SerializeField] private Npc engineer;

    [SerializeField] private List<ItemInteractable> items = new List<ItemInteractable>();

    [SerializeField] private Recycler recycler;

    [SerializeField] private bool W = false;
    [SerializeField] private bool A = false;
    [SerializeField] private bool S = false;
    [SerializeField] private bool D = false;

    [SerializeField] private bool itemsAreEnabled = false;

    [SerializeField] private GameObject uiTutorialParent;

    [SerializeField] private GameObject buttonA;
    [SerializeField] private GameObject buttonW;
    [SerializeField] private GameObject buttonS;
    [SerializeField] private GameObject buttonD;
    [SerializeField] private GameObject buttonE;

    [SerializeField] private GameObject tutorialDoor;

    [SerializeField] private bool tutorialIntroEnded;

    [SerializeField] private Scenes scene;

    [SerializeField] private GameObject tutorialSysTrigger;

    private void OnEnable()
    {
        InventoryManager.OnInventoryChanged += CheckForTutorialItem;
        PlayerController.OnActionPerformed += OnInteractPerformed;
        PlayerController.OnMovePerformed += OnMovePerformed;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChanged -= CheckForTutorialItem;
        PlayerController.OnActionPerformed -= OnInteractPerformed;
        PlayerController.OnMovePerformed -= OnMovePerformed;
    }

    private void Awake()
    {
        if (Instance == null)
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
        if (state == TutorialState.ItemPickUp && engineer.CurrentQuest?.QuestState == QuestState.InProgress && !itemsAreEnabled)
        {
            ToggleItems(true);
        }
    }

    private IEnumerator StartTutorial()
    {
        GameStateManager.Instance.SetGameState(GameStates.Dialogue);
        yield return UIFadeScreenHelper.Instance.EndTransition();
        buttonE.SetActive(true);
        ProceedDialogue();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!engineer.GetComponent<NpcInteractable>().isActiveAndEnabled && GameStateManager.Instance.GetGameState() != GameStates.Dialogue && !AllButtonsDone())
        {

            Vector2 input = context.ReadValue<Vector2>();

            if (input.y > 0 && !W) { W = true; buttonW.SetActive(false); }
            if (input.y < 0 && !S) { S = true; buttonS.SetActive(false); }
            if (input.x < 0 && !A) { A = true; buttonA.SetActive(false); }
            if (input.x > 0 && !D) { D = true; buttonD.SetActive(false); }

            if (state == TutorialState.Intro && AllButtonsDone())
            {
                if (!InventoryManager.Instance.Items.Exists(t => t.ItemData.title == "Tutorial_Item"))
                {
                    state = TutorialState.ItemPickUp;
                    engineer.GetComponent<NpcInteractable>().enabled = true;
                }
            }
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!tutorialIntroEnded)
        {
            ProceedDialogue();
            buttonE.SetActive(false);
            if (engineer.NpcState != NpcStates.Intro)
            {
                tutorialIntroEnded = true;
                buttonA.SetActive(true);
                buttonD.SetActive(true);
                buttonW.SetActive(true);
                buttonS.SetActive(true);
            }
        }
    }

    private bool AllButtonsDone()
    {
        return A && W && S && D;
    }

    private void ProceedDialogue()
    {
        if (engineer.NpcState == NpcStates.Quest)
        {
            NpcDialogueManager.Instance.EndDialogue();
        }
        NpcDialogueManager.Instance.StartDialogue(engineer);
        NpcDialogueManager.Instance.InteractDialogue();
    }

    private void ToggleItems(bool isEnabled)
    {
        if (items == null)
        {
            return;
        }

        foreach (ItemInteractable i in items)
        {
            i.enabled = isEnabled;
        }

        itemsAreEnabled = isEnabled;
    }

    private void CheckForTutorialItem(List<ItemInstance> items)
    {
        if (items.Exists(t => t.ItemData.title == "DriftWood") && items.Exists(t => t.ItemData.title == "Can") && state == TutorialState.ItemPickUp)
        {
            if (!items.Exists(t => t.ItemData.title == "Tutorial_Item"))
            {
                QuestItemInstance questItem = DataUtil.Instance.GetTutorialItem();
                InventoryManager.Instance.AddItem(questItem);
                state = TutorialState.Crafting;
            }
        }

        if (!items.Exists(t => t.ItemData.title == "Tutorial_Item") && state == TutorialState.Crafting && engineer.CurrentQuest.QuestState == QuestState.InProgress)
        {
            recycler.enabled = true;
            if(tutorialSysTrigger != null)
            {
                tutorialSysTrigger.SetActive(true);
                tutorialSysTrigger = null;
            }
            QuestItemInstance questItem = DataUtil.Instance.GetQuestItemByName("Shovel");
            engineer.NpcData.CurrentQuest.QuestItem = questItem;
        }

        if (items.Exists(t => t.ItemData.title == "Shovel"))
        {
            if(engineer.NpcData.CurrentQuest.QuestState == QuestState.Offer && state == TutorialState.Crafting)
            { 
                engineer.NpcData.CurrentQuest.AcceptQuest();
                QuestItemInstance questItem = DataUtil.Instance.GetQuestItemByName("Shovel");
                engineer.NpcData.CurrentQuest.QuestItem = questItem;
            }
            state = TutorialState.End;
        }

        if (!items.Exists(t => t.ItemData.title == "Shovel") && state == TutorialState.End)
        {
            tutorialDoor.SetActive(true);
        }
    }

    public void LeaveTutorial()
    {
        LocationManager.Instance.TravelToScene(scene);
    }
}
