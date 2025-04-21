using System.Collections.Generic;
using UnityEngine;

public class AwarenessManager : MonoBehaviour
{
    //TODO: Atm dummy to have some impact on the TideUpBox
    [SerializeField] private int awarenessScore = 1;

    public static AwarenessManager Instance;

    public int AwarenessScore
    {
        get => awarenessScore;
        set => awarenessScore = value;
    }

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
