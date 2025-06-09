using UnityEngine;

public class SystemInteractionManager : MonoBehaviour
{
    public static SystemInteractionManager Instance;

    [SerializeField] private SystemInteractable currentInteractable;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SystemInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    public void SetCurrentInteractable(SystemInteractable sys)
    {
        currentInteractable = sys;
    }

    public void CloseSystemInformation()
    {
        if(currentInteractable != null)
        {
            currentInteractable.EndInteraction();
        }
    }
}
