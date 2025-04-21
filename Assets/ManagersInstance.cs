using System;
using UnityEngine;

public class ManagersInstance : MonoBehaviour
{
    public static ManagersInstance Instance;

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
