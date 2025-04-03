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

    [Header("TESTSETUP")]
    [SerializeField] private bool spawnToastTestBool = true;
    [SerializeField] private float spawnTime;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(TestSpawn());
    }

    public void SpawnToastMessage(ToastType type, string title = "", string description="", Sprite sprite = null)
    {
        GameObject newToast = null;

        switch (type)
        {
            case ToastType.Item:
                newToast = CreateToast(itemToastPrefab, itemToastParent);
                newToast.GetComponent<ToastNotificationItem>().Sprite = sprite;
                newToast.GetComponent<ToastNotificationItem>().Title = title;
                break;
            case ToastType.Environment:
                newToast = CreateToast(environmentToastPrefab, environmentToastParent);
                newToast.GetComponent<ToastNotificationItem>().Title = title;
                break;
            case ToastType.Important:
                newToast = CreateToast(importantToastPrefab, importantToastParent);
                newToast.GetComponent<ToastNotificationItem>().Title = title;
                newToast.GetComponent<ToastNotificationItem>().Description = description;
                break;
        }

        AddToList(type, newToast);
    }

    private GameObject CreateToast(GameObject prefab, Transform parent)
    {
        GameObject newToast = Instantiate(prefab);
        newToast.transform.SetParent(parent);
        return newToast;
    }

    private void AddToList(ToastType type, GameObject toast)
    {
        if (!toast) return;

        switch (type)
        {
            case ToastType.Item:
                if(ListCapReached(itemToastList, itemToastStackSize))
                {
                    itemToastList[0].GetComponent<ToastNotificationItem>().PlayEndAnimation();
                    itemToastList.RemoveAt(0);
                }
                itemToastList.Add(toast);
                break;
            case ToastType.Environment:
                if (ListCapReached(environmentToastList, environmentToastStackSize))
                {
                    environmentToastList[0].GetComponent<ToastNotificationItem>().PlayEndAnimation();
                    environmentToastList.RemoveAt(0);
                }
                environmentToastList.Add(toast);
                break;
            case ToastType.Important:
                if (ListCapReached(importantToastList, importantToastStackSize))
                {
                    importantToastList[0].GetComponent<ToastNotificationItem>().PlayEndAnimation();
                    importantToastList.RemoveAt(0);
                }
                importantToastList.Add(toast);
                break;
        }
    }

    public void RemoveFromList(ToastType type, GameObject toast)
    {
        switch (type)
        {
            case ToastType.Item:
                itemToastList.Remove(itemToastList.Find(t => t == toast));
                break;
            case ToastType.Environment:
                environmentToastList.Remove(environmentToastList.Find(t => t == toast));
                break;
            case ToastType.Important:
                importantToastList.Remove(importantToastList.Find(t => t == toast));
                break;
        }
    }

    private bool ListCapReached(List<GameObject> list, int maxCap)
    {
        return list.Count >= maxCap;
    }

    private IEnumerator TestSpawn()
    {
        while (spawnToastTestBool)
        {
            SpawnToastMessage(ToastType.Item, "Test", "Decription");
            SpawnToastMessage(ToastType.Environment, "Test", "Decription");
            SpawnToastMessage(ToastType.Important, "Test", "Decription");
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
