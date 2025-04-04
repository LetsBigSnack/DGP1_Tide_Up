using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public enum ToastType
{
    Item,
    Environment,
    Important
}

public class UI_ToastManager : MonoBehaviour
{
    public static UI_ToastManager Instance;

    [Header("Prefabs")]
    [SerializeField] private GameObject itemToastPrefab;
    [SerializeField] private GameObject environmentToastPrefab;
    [SerializeField] private GameObject importantToastPrefab;

    [Header("ToastParents")]
    [SerializeField] private Transform itemToastParent;
    [SerializeField] private Transform environmentToastParent;
    [SerializeField] private Transform importantToastParent;

    [Header("MaxToastStack")]
    [SerializeField] private int itemToastStackSize;
    [SerializeField] private int environmentToastStackSize;
    [SerializeField] private int importantToastStackSize;
    private Dictionary<ToastType, int> _toastCapLists = new Dictionary<ToastType, int>();

    [Header("CurrentToastLists")]
    [SerializeField] private List<GameObject> itemToastList;
    [SerializeField] private List<GameObject> environmentToastList;
    [SerializeField] private List<GameObject> importantToastList;
    private Dictionary<ToastType, List<GameObject>> _toastLists = new Dictionary<ToastType, List<GameObject>>();

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
        FillLists();
    }

    private void FillLists()
    {
        _toastLists.Add(ToastType.Item, itemToastList);
        _toastLists.Add(ToastType.Environment, environmentToastList);
        _toastLists.Add(ToastType.Important, importantToastList);

        _toastCapLists.Add(ToastType.Item, itemToastStackSize);
        _toastCapLists.Add(ToastType.Environment, environmentToastStackSize);
        _toastCapLists.Add(ToastType.Important, importantToastStackSize);
    }

    public void SpawnToastMessage(ToastType type, string title = "", string description="", Sprite sprite = null)
    {
        GameObject newToast = null;

        switch (type)
        {
            case ToastType.Item:
                newToast = CreateToast(itemToastPrefab, itemToastParent);
                newToast.GetComponent<ToastNotificationItem>().SetToast(sprite, title);
                break;
            case ToastType.Environment:
                newToast = CreateToast(environmentToastPrefab, environmentToastParent);
                newToast.GetComponent<ToastNotificationItem>().SetToast(titleText:title);
                break;
            case ToastType.Important:
                newToast = CreateToast(importantToastPrefab, importantToastParent);
                newToast.GetComponent<ToastNotificationItem>().SetToast(titleText:title, descriptionText:description);
                break;
        }
        AddToList(type, newToast);
    }

    private GameObject CreateToast(GameObject prefab, Transform parent)
    {
        GameObject newToast = Instantiate(prefab, parent);
        return newToast;
    }

    private void AddToList(ToastType type, GameObject toast)
    {
        if (toast == null)
        {
            return;
        }
        if (ListCapReached(_toastLists[type], _toastCapLists[type]))
        {
            _toastLists[type][0].GetComponent<ToastNotificationItem>().PlayEndAnimation();
            _toastLists[type].RemoveAt(0);
        }
        _toastLists[type].Add(toast);
    }

    public void RemoveFromList(ToastType type, GameObject toast)
    {
        _toastLists[type].Remove(_toastLists[type].Find(t => t == toast));      
    }

    private bool ListCapReached(List<GameObject> list, int maxCap)
    {
        return list.Count >= maxCap;
    }
}
