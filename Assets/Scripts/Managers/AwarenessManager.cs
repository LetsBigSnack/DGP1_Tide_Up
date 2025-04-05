using System.Collections.Generic;
using UnityEngine;

public class AwarenessManager : MonoBehaviour
{
    //TODO: Atm dummy to have some impact on the TideUpBox
    [SerializeField] private int awarenessScore = 1;
    [SerializeField] private int awarenessBoxMultiplier = 3;

    public static AwarenessManager Instance;

    public int AwarenessScore
    {
        get => awarenessScore;
        set => awarenessScore = value;
    }
    public int AwarenessBoxMultiplier
    {
        get => awarenessBoxMultiplier;
        set => awarenessBoxMultiplier = value;
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
