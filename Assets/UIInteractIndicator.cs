using Helpers.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    
    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public void SetUpIndicator(UIInteractionRepresentation interactRepresentation, GameObject target)
    {
        Vector3 topPosition = ColliderUtil.GetTopPosition(target);
        
        Show();
        text.text = interactRepresentation.Text;
        image.sprite = interactRepresentation.Sprite;
        transform.position = topPosition + Vector3.up * (interactRepresentation.Padding);
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
    
}
