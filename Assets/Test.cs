using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    [SerializeField] private bool spawnToastTestBool = false;
    [SerializeField] private int spawnTime;
    private float timeCount = 3;
    public void Update()
    {
        timeCount += Time.deltaTime;
        if (spawnToastTestBool)
        {
            if (timeCount >= spawnTime)
            {
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Item, "Test");
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Test");
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Test", "Decription");
                timeCount = 0;
            }
        }
    }
}
