using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public static Player Instance;
    
    [SerializeField] private float moveSpeed = 5f;

    public float MoveSpeed 
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }
    
}
