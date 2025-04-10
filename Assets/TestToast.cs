using UnityEngine;
using System.Collections;

public class TestToast : MonoBehaviour
{
    [SerializeField] private bool spawnToastTestBool = false;
    [SerializeField] private int spawnTime;
    [SerializeField] private int spawnTimeImportant;
    private float timeCount = 3;
    private float timeCountImportant = 10;
    public void Update()
    {
        timeCount += Time.deltaTime;
        timeCountImportant += Time.deltaTime;
        if (spawnToastTestBool)
        {
            if (timeCount >= spawnTime)
            {
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Item, "Test");
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Environment, "Test");
                timeCount = 0;
            }
            if (timeCountImportant >= spawnTimeImportant)
            {
                UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Test", "Decription");
                timeCountImportant = 0;
            }
        }
    }
}
