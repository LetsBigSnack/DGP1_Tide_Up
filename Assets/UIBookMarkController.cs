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

    public void Open()
    {
        if(_curBookmarks.Count > 0)
        {
            return;
        }
        StopAllBookMarkCoroutines();
        StartCoroutine(CreateBookMarksRoutine());
    }

    public void Close()
    {
        StopAllBookMarkCoroutines();
    }

    private IEnumerator CreateBookMarksRoutine()
    {
        List<JournalType> typesToInstantiate = new List<JournalType>();

        bool isRight = true;

        if (UIJournalManager.Instance.GetCurrentState() != JournalType.Closed &&
            UIShopManager.Instance.GetCurrentState() == ShopType.Closed &&
            UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            typesToInstantiate.Add(JournalType.Inventory);
            typesToInstantiate.Add(JournalType.Recipies);
            typesToInstantiate.Add(JournalType.Quests);
            typesToInstantiate.Add(JournalType.FriendBook);
            typesToInstantiate.Add(JournalType.Calender);
            typesToInstantiate.Add(JournalType.Map);
        }

        foreach (JournalType j in typesToInstantiate)
        {
            if(!BookmarkExistsInParent(j, ReturnBookMarkFiller(j, isRight)))
            {
                if(j == JournalType.Inventory) {
                    isRight = false;
                }
                else
                {
                    isRight = true;
                }

                Coroutine spawn = StartCoroutine(SpawnBookmarks(j, ReturnBookMarkFiller(j, isRight), isRight));
                _bookMarkSpawnings.Add(spawn);
                yield return spawn;
            }
        }
    }

    private Transform ReturnBookMarkFiller(JournalType type, bool isRight)
    {
        return _fillerParents.Find(x => x.Type == type && x.IsRight == isRight).gameObject.transform;
    }

    public void SwitchPosition(UIJournalBookMarkItem bookMark)
    {
        StartCoroutine(SwitchPositionRoutine(bookMark));
    }
    
    private IEnumerator SwitchPositionRoutine(UIJournalBookMarkItem bookMark)
    {
        DestroyBookMarkByTypeCoroutine(bookMark.Type, bookMark.IsRight);
        yield return new WaitForSeconds(spawnTime);
        if (!BookmarkExistsInParent(bookMark.Type, ReturnBookMarkFiller(bookMark.Type, !bookMark.IsRight)))
        {
            Coroutine spawn = StartCoroutine(SpawnBookmarks(bookMark.Type, ReturnBookMarkFiller(bookMark.Type, !bookMark.IsRight), !bookMark.IsRight));
            _bookMarkSpawnings.Add(spawn);
            yield return spawn;
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

    public IEnumerator DestroyBookMarkByTypeCoroutine(JournalType type, bool isRight)
    {
        GameObject bookMarkToDestroy = _curBookmarks.Find(g => g.GetComponent<UIJournalBookMarkItem>().Type == type 
        && g.GetComponent<UIJournalBookMarkItem>().IsRight == isRight);

        UIJournalFiller filler = ReturnFillerByTypeAndBool(type, isRight);
        filler.CurrentItem = null;

        if (bookMarkToDestroy == null)
        {
            yield break;
        }
        _curBookmarks.Remove(bookMarkToDestroy);
        bookMarkToDestroy.GetComponent<UIJournalBookMarkItem>().Remove();
    }

    private IEnumerator SpawnBookmarks(JournalType type, Transform parent, bool isRight)
    {
        StartCoroutine(DestroyBookMarkByTypeCoroutine(type, !isRight));

        yield return new WaitForSeconds(spawnTime);

        GameObject newBookMark = Instantiate(btnPrefab, parent);
        _curBookmarks.Add(newBookMark);

        UIJournalFiller filler = ReturnFillerByTypeAndBool(type, isRight);
        UIJournalBookMarkItem item = newBookMark.GetComponent<UIJournalBookMarkItem>();
        item.Type = type;
        item.IsRight = isRight;
        item.CurrentParent = filler;

        filler.CurrentItem = item;
    }

    private UIJournalFiller ReturnFillerByTypeAndBool(JournalType type, bool isRight)
    {
        return _fillerParents.Find(x => x.Type == type && x.IsRight == isRight);
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

        foreach(UIJournalFiller i in _fillerParents)
        {
            Destroy(i.CurrentItem?.gameObject);
            i.CurrentItem = null;
        }

        _curBookmarks.Clear();
    }
}

