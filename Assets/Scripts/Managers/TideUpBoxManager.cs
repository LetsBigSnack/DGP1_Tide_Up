using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class TideUpBoxManager : MonoBehaviour
{
    [SerializeField] private List<TideUpBox> allTideUpBoxes;

    [SerializeField] private float timeToAddItems = 6f;
    [SerializeField] private float resetDayTime = 6f;

    private bool _hasAddedItemsToday = false;
    private bool _waitingForDependencies = false;

    public static TideUpBoxManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TimeManager.OnTimeChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= HandleTimeChanged;
    }

    private void Update()
    {
        Debug.Log("Has added = " + _hasAddedItemsToday);
    }

    private void HandleTimeChanged(float currentTime)
    {
        if (currentTime >= timeToAddItems && !_hasAddedItemsToday)
        {
            AddDailyItems();
            _hasAddedItemsToday = true;
        }

        if (currentTime < resetDayTime && _hasAddedItemsToday)
        {
            _hasAddedItemsToday = false;
        }
    }

    private void AddDailyItems()
    {
#if UNITY_EDITOR
        ConsoleUtil.ClearConsole();
#endif
        if (DataUtil.Instance == null || AwarenessManager.Instance == null)
        {
            if (_waitingForDependencies)
            {
                return;
            }
            StartCoroutine(WaitAndTryAgain());
        }

        foreach (TideUpBox box in allTideUpBoxes)
        {
            int spaceLeft = box.MaxTotalTrash - box.BoxInventory.Count;
            if (spaceLeft <= 0)
            {
                Debug.Log("Tide-Up-Box is full. No materials could be added.");
                return;
            }

            int maxAddableToday = Mathf.Min(spaceLeft, AwarenessManager.Instance.AwarenessScore * box.AwarenessBoxMultiplier);

            for (int i = 0; i < maxAddableToday; i++)
            {
                TrashData randomTrash = DataUtil.Instance.GetRandomTrash();

                box.BoxInventory.Add(randomTrash);
                Debug.Log("Added " + randomTrash + " to the Tide-Up-Box");
            }

            Debug.Log("Tide-Up-Box currently has: " + box.BoxInventory.Count + " items");
        }
    }

    public TideUpBox GetTideUpBox(int boxIndex)
    {
        if (allTideUpBoxes[boxIndex] == null)
        {
            Debug.Log("Item index is not in the box");
            return null;
        }
        return allTideUpBoxes[(int)boxIndex];
    }
    private IEnumerator WaitAndTryAgain()
    {
        _waitingForDependencies = true;
        Debug.Log("Waiting for dependencies...");

        while (DataUtil.Instance == null || AwarenessManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        _waitingForDependencies = false;
        Debug.Log("Dependencies ready. Adding daily items.");
        AddDailyItems();
    }

}
