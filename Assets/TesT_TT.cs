using System;
using UnityEngine;
using UnityEngine.UI;

public class TesT_TT : MonoBehaviour
{
    private void Awake()
    {
        Button b =GetComponent<Button>();
        b.navigation = new Navigation();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
