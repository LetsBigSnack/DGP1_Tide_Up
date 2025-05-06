using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BookMarkLink
{
    [SerializeField] private JournalType type;

    [SerializeField] private UIJournalBookMarkItem bookMarkPosLeft;
    [SerializeField] private UIJournalBookMarkItem bookMarkPosRight;

    public JournalType GetCurrentType()
    {
        return type;
    }

    public UIJournalBookMarkItem GetPosLeft()
    {
        return bookMarkPosLeft;
    }

    public UIJournalBookMarkItem GetPosRight()
    {
        return bookMarkPosRight;
    }
}


public class UIBookMarkController : MonoBehaviour
{
    public static UIBookMarkController Instance;

    [SerializeField] private float spawnTime;

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private GameObject rightParent;
    [SerializeField] private GameObject leftParent;
    [SerializeField] private GameObject reupcylceParent;

    private List<GameObject> _curBookmarks = new List<GameObject>();
    [SerializeField] private List<BookMarkLink> _linkedBookmarks = new List<BookMarkLink>();

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

    private void OnEnable()
    {
        UIJournalManager.OnJournalStateChanged += UpdateBookmarks;
    }

    private void OnDisable()
    {
        UIJournalManager.OnJournalStateChanged -= UpdateBookmarks;
    }

    private void UpdateBookmarks(JournalType type)
    {
        if(type == JournalType.Closed)
        {
            return;
        }

        if(UIReUpcycleManager.Instance.GetCurrentState() != ReUpcyclerType.Closed)
        {
            return;
        }

        BookMarkLink link = ReturnBookMarkLinkByType(type);

        if (!link.GetPosLeft().gameObject.activeInHierarchy)
        {
            link.GetPosLeft().Activate();
        }
        else
        {
            link.GetPosRight().Activate();
        }
    }

    private BookMarkLink ReturnBookMarkLinkByType(JournalType type)
    {
        return _linkedBookmarks.Find(x => x.GetCurrentType() == type);
    }

    public void OpenMenu()
    {
        if(UIReUpcycleManager.Instance.GetCurrentState() != ReUpcyclerType.Closed)
        {
            reupcylceParent.SetActive(true);
            return;
        }
        rightParent.SetActive(true);
        leftParent.SetActive(true);
    }

    public void CloseMenu()
    {
        reupcylceParent.SetActive(false);
        rightParent.SetActive(false);
        leftParent.SetActive(false);
    }
}

