using UnityEngine;
using UnityEngine.UI;

public class UI_SoundManager : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        MasterVolumeChange();
        MusicVolumeChange();
        SFXVolumeChange();
    }

    public void MasterVolumeChange()
    {
        if (masterSlider == null)
        {
            Debug.Log("No master slider assigned!");
            return;
        }

        float volume = masterSlider.value;
        SoundManager.Instance.SetMasterVolume(volume);
    }

    public void MusicVolumeChange()
    {
        if (musicSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }

        float volume = musicSlider.value;
        SoundManager.Instance.SetMusicVolume(volume);
    }

    public void SFXVolumeChange()
    {
        if (sfxSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }

        float volume = sfxSlider.value;
        SoundManager.Instance.SetSfxVolume(volume);
    }
}
