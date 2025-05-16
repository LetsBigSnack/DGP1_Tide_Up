using UnityEngine;

public class FireFly : MonoBehaviour
{
    [SerializeField] private GameObject filter;

    private void OnEnable()
    {
        TimeManager.OnTimeChanged += CheckIfActive;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= CheckIfActive;
    }

    private void CheckIfActive(float time)
    {
        if(time < 18 && time > 6){
            filter.SetActive(false);
        }
        else
        {
            filter.SetActive(true);
        }
    }
}
