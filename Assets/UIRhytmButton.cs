using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



[Serializable]
public class DeviceButton
{
    public DeviceType type;
    public Sprite sprite;
}


[Serializable]
public class NoteRepresentation
{
    public MiniGameButton miniGameButton;
    
    public List<DeviceButton> deviceButtons;
    
}

public class UIRhytmButton : MonoBehaviour
{
    [Header("currentType")]

    [Header("Image")]
    [SerializeField] private Image img;
    [SerializeField] private Image hitImage;
    [SerializeField] private bool hit;
    [SerializeField] private List<NoteRepresentation> noteRepresentation;
    
    private MiniGameButton _miniGameButton;
    
    
    private void OnEnable()
    {
        UpdateRepresentation(InputDeviceHelper.Instance.GetLastDeviceType());
        InputDeviceHelper.OnDeviceChange += UpdateRepresentation;
    }

    private void OnDisable()
    {
        InputDeviceHelper.OnDeviceChange -= UpdateRepresentation;
    }

    public void Setup(MiniGameNote miniGameNote)
    {
        _miniGameButton = miniGameNote.button;
        hitImage.gameObject.SetActive(miniGameNote.isHit);
        UpdateRepresentation(InputDeviceHelper.Instance.GetLastDeviceType());
    }
    
    public void UpdateRepresentation(DeviceType type)
    {
        if(type == DeviceType.Mouse)
        {
            type = DeviceType.Keyboard;
        }
        
        img.sprite = noteRepresentation
            .Where(c => c.miniGameButton == _miniGameButton)
            .Select(s => s.deviceButtons).FirstOrDefault()
            .Where(h => h.type == type)
            .Select(s => s.sprite)
            .FirstOrDefault();
    }
}
