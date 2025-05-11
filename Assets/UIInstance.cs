using System;
using UnityEngine;

public class UIInstance : MonoBehaviour
{
    public static UIInstance Instance;
    [SerializeField] private Transform worldSpace;

    public Transform WorldSpace
    {
        get => worldSpace;
    }

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
}
