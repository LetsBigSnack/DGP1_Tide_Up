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

    [Header("CurrentToastLists")]
    [SerializeField] private List<GameObject> itemToastList;
    [SerializeField] private List<GameObject> environmentToastList;
    [SerializeField] private List<GameObject> importantToastList;
    private Dictionary<ToastType, List<GameObject>> toastLists = new Dictionary<ToastType, List<GameObject>>();
    private Dictionary<ToastType, int> toastCapLists = new Dictionary<ToastType, int>();

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
        toastLists.Add(ToastType.Item, itemToastList);
        toastLists.Add(ToastType.Environment, environmentToastList);
        toastLists.Add(ToastType.Important, importantToastList);

        toastCapLists.Add(ToastType.Item, itemToastStackSize);
        toastCapLists.Add(ToastType.Environment, environmentToastStackSize);
        toastCapLists.Add(ToastType.Important, importantToastStackSize);
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
        if (ListCapReached(toastLists[type], toastCapLists[type]))
        {
            toastLists[type][0].GetComponent<ToastNotificationItem>().PlayEndAnimation();
            toastLists[type].RemoveAt(0);
        }
        toastLists[type].Add(toast);
    }

    public void RemoveFromList(ToastType type, GameObject toast)
    {
        toastLists[type].Remove(toastLists[type].Find(t => t == toast));      
    }

    private bool ListCapReached(List<GameObject> list, int maxCap)
    {
        return list.Count >= maxCap;
    }
}
