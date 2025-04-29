using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIBookMarkController : MonoBehaviour
{
    public static UIBookMarkController Instance;

    [SerializeField] private float spawnTime;

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private GameObject fillerPrefab;
    [SerializeField] private Transform rightParent;
    [SerializeField] private Transform leftParent;

    [SerializeField] private List<UIJournalFiller> _fillerParents;

    private List<GameObject> _curBookmarks = new List<GameObject>();
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

        }
        else if (UIJournalManager.Instance.GetCurrentState() != JournalType.Closed &&
                 UIShopManager.Instance.GetCurrentState() == ShopType.Closed &&
                 UIReUpcycleManager.Instance.GetCurrentState() != ReUpcyclerType.Closed)
        {

        }

        foreach (JournalType j in typesToInstantiate)
        {
            Coroutine spawn = StartCoroutine(SpawnBookmarks(j, parent));
            _bookMarkSpawnings.Add(spawn);
            yield return spawn;
        }
    }

    private UIJournalFiller ReturnBookMarkFiller(JournalType type, bool isRight)
    {
        return _fillerParents.Find(x => x.Type == type && x.IsRight == isRight);
    }

    public void SwitchPosition(UIJournalBookMarkItem bookMark)
    {
        StartCoroutine(SwitchPositionRoutine(bookMark));
    }

    private IEnumerator SwitchPositionRoutine(UIJournalBookMarkItem bookMark)
    {
        if (bookMark.IsRight)
        {
               
        }
        else
        {
            
        }
    }

    private bool BookmarkExistsInParent(JournalType type, Transform parent)
    {
        return _curBookmarks.Any(bookmark =>
        {
            UIJournalBookMarkItem item = bookmark.GetComponent<UIJournalBookMarkItem>();
            return item.Type == type && bookmark.transform.parent == parent;
        });
    }

    public IEnumerator DestroyBookMarkByTypeCoroutine(JournalType type, bool createFiller = false)
    {
        GameObject bookMarkToDestroy = _curBookmarks.Find(g => g.GetComponent<UIJournalBookMarkItem>().Type == type);
        if (bookMarkToDestroy == null)
        {
            yield break;
        }

        Transform parent = bookMarkToDestroy.transform.parent;
        int siblingIndex = bookMarkToDestroy.transform.GetSiblingIndex();

        _curBookmarks.Remove(bookMarkToDestroy);
        bookMarkToDestroy.GetComponent<UIJournalBookMarkItem>().Remove();

        if (createFiller && parent == rightParent)
        {
            yield return new WaitForSeconds(spawnTime);

            GameObject filler = Instantiate(fillerPrefab, parent);
            UIJournalFiller fillerComponent = filler.GetComponent<UIJournalFiller>();

            filler.transform.SetSiblingIndex(siblingIndex);

            StartCoroutine(DelayedLayoutRebuild(parent));
        }
    }

    private IEnumerator SpawnBookmarks(JournalType type, Transform parent)
    {
        StartCoroutine(DestroyBookMarkByTypeCoroutine(type, createFiller: parent == leftParent));

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
        StartCoroutine(DelayedLayoutRebuild(parent));
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

    private IEnumerator DelayedLayoutRebuild(Transform parent)
    {
        yield return new WaitForEndOfFrame();

        var layoutGroup = parent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
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

