using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBookMarkController : MonoBehaviour
{
    public static UIBookMarkController Instance;

    [SerializeField] private float spawnTime;

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private GameObject fillerPrefab;
    [SerializeField] private Transform rightParent;
    [SerializeField] private Transform leftParent;

    private List<GameObject> _curBookmarks = new List<GameObject>();

    private Dictionary<JournalType, int> _prefabPosition = new Dictionary<JournalType, int>()
    {
        {JournalType.Inventory, 1 },
        {JournalType.Quests, 2 },
        {JournalType.Recipies, 3 },
        {JournalType.FriendBook, 4 },
        {JournalType.Calender, 5 },
        {JournalType.Map, 6 },
    };

    private List<Coroutine> _bookMarkSpawnings = new List<Coroutine>();

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
        StopAllBookMarkCoroutines();
        StartCoroutine(CreateBookMarksRoutine());
    }

    private void OnDisable()
    {
        StopAllBookMarkCoroutines();
    }

    private IEnumerator CreateBookMarksRoutine()
    {
        List<JournalType> typesToInstantiate = new List<JournalType>();
        Transform parent = null;

        if (UIJournalManager.Instance.GetCurrentState() != JournalType.Closed &&
            UIShopManager.Instance.GetCurrentState() == ShopType.Closed &&
            UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            parent = rightParent;
            typesToInstantiate = _prefabPosition.Keys.ToList();
        }
        else if (UIJournalManager.Instance.GetCurrentState() != JournalType.Closed &&
                 UIShopManager.Instance.GetCurrentState() == ShopType.Closed &&
                 UIReUpcycleManager.Instance.GetCurrentState() != ReUpcyclerType.Closed)
        {
            parent = leftParent;
            typesToInstantiate = _prefabPosition.Keys
                .Where(j => j == JournalType.Inventory || j == JournalType.Recipies)
                .ToList();
        }

        foreach (JournalType j in typesToInstantiate)
        {
            Coroutine spawn = StartCoroutine(SpawnBookmarks(j, parent));
            _bookMarkSpawnings.Add(spawn);
            yield return spawn;
        }
    }

    public void SwitchPosition(UIJournalBookMarkItem bookMark)
    {
        StartCoroutine(SwitchPositionRoutine(bookMark));
    }

    private IEnumerator SwitchPositionRoutine(UIJournalBookMarkItem bookMark)
    {
        List<JournalType> typesToInstantiate = new List<JournalType>();
        Transform targetParent = bookMark.IsRight ? leftParent : rightParent;

        if (bookMark.IsRight)
        {
            for (int i = _prefabPosition[bookMark.Type]; i > 0; i--)
            {
                var match = _prefabPosition.FirstOrDefault(x => x.Value == i);
                if (!match.Equals(default(KeyValuePair<JournalType, int>)))
                {
                    typesToInstantiate.Add(match.Key);
                }
            }

            typesToInstantiate.Reverse();
        }
        else
        {
            for (int i = _prefabPosition[bookMark.Type]; i <= _prefabPosition.Max(x => x.Value); i++)
            {
                var match = _prefabPosition.FirstOrDefault(x => x.Value == i);
                if (!match.Equals(default(KeyValuePair<JournalType, int>)))
                {
                    typesToInstantiate.Add(match.Key);
                }
            }
        }

        foreach (JournalType j in typesToInstantiate)
        {
            if (!BookmarkExistsInParent(j, targetParent))
            {
                Coroutine spawn = StartCoroutine(SpawnBookmarks(j, targetParent));
                yield return spawn;
            }
        }

        ReorderBookmarksInParent(targetParent);
    }

    private bool BookmarkExistsInParent(JournalType type, Transform parent)
    {
        return _curBookmarks.Any(bookmark =>
        {
            UIJournalBookMarkItem item = bookmark.GetComponent<UIJournalBookMarkItem>();
            return item.Type == type && bookmark.transform.parent == parent;
        });
    }

    public void DestroyBookMarkByType(JournalType type, bool createFiller = false)
    {
        GameObject bookMarkToDestroy = _curBookmarks.Find(g => g.GetComponent<UIJournalBookMarkItem>().Type == type);
        if (bookMarkToDestroy == null)
        {
            return;
        }

        Transform parent = bookMarkToDestroy.transform.parent;

        _curBookmarks.Remove(bookMarkToDestroy);
        bookMarkToDestroy.GetComponent<UIJournalBookMarkItem>().Remove();

        if (createFiller && parent == rightParent)
        {
            GameObject filler = Instantiate(fillerPrefab, parent);
            UIJournalFiller fillerComponent = filler.GetComponent<UIJournalFiller>();

            filler.transform.SetSiblingIndex(bookMarkToDestroy.transform.GetSiblingIndex());
        }
    }

    private IEnumerator SpawnBookmarks(JournalType type, Transform parent)
    {
        DestroyBookMarkByType(type, createFiller: parent == leftParent);

        yield return new WaitForSeconds(spawnTime);

        int siblingIndex = -1;

        if (parent == rightParent)
        {
            siblingIndex = FindAndRemoveFiller(type, parent);
        }

        GameObject newBookMark = Instantiate(btnPrefab, parent);
        _curBookmarks.Add(newBookMark);

        UIJournalBookMarkItem item = newBookMark.GetComponent<UIJournalBookMarkItem>();
        item.Type = type;
        item.IsRight = (parent != leftParent);

        if (siblingIndex >= 0)
        {
            newBookMark.transform.SetSiblingIndex(siblingIndex);
        }
    }

    private int FindAndRemoveFiller(JournalType type, Transform parent)
    {
        foreach (Transform child in parent)
        {
            var filler = child.GetComponent<UIJournalFiller>();
            if (filler != null)
            {
                int index = child.GetSiblingIndex();
                Destroy(child.gameObject);
                return index;
            }
        }
        return -1; // No filler found
    }

    private void ReorderBookmarksInParent(Transform parent)
    {
        var bookmarksInParent = parent.GetComponentsInChildren<UIJournalBookMarkItem>(true);

        List<UIJournalBookMarkItem> orderedBookmarks;

        if (parent == rightParent)
        {
            orderedBookmarks = bookmarksInParent
                .OrderBy(bookmark => _prefabPosition[bookmark.Type])
                .ToList();
        }
        else
        {
            orderedBookmarks = bookmarksInParent
                .OrderByDescending(bookmark => _prefabPosition[bookmark.Type])
                .ToList();
        }

        for (int i = 0; i < orderedBookmarks.Count; i++)
        {
            orderedBookmarks[i].transform.SetSiblingIndex(i);
        }
    }

    private void StopAllBookMarkCoroutines()
    {
        if(_bookMarkSpawnings.Count > 0)
        {
            foreach (Coroutine c in _bookMarkSpawnings)
            {
                StopCoroutine(c);
            }
        }
        ClearBookmarks();
    }

    private void ClearBookmarks()
    {
        if(_curBookmarks.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in _curBookmarks)
        {
            Destroy(o);
        }

        _curBookmarks.Clear();
    }
}

