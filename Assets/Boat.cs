using System;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public static Boat Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
