using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReUpCyclerSpriteHelper : MonoBehaviour
{
    [SerializeField] private GameObject selected;
    private Button btn;

    private void OnEnable()
    {
        btn = gameObject.GetComponent<Button>();
    }

    public void OnSelected()
    {
        if (btn == null || selected == null || !btn.interactable || selected.gameObject.activeInHierarchy)
            return;

        selected.SetActive(true);
    }

    public void OffSelected()
    {
        if (btn == null || selected == null || !selected.gameObject.activeInHierarchy)
            return;
        
        selected.SetActive(false);
    }

}
