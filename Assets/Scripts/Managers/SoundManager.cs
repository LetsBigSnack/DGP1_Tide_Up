using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundData> AllSfxSounds;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public static SoundManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSfxVolume();
    }

    // Taken from Monkepok, we prob need this later right?
    //private void SetVolumesFromPrefs()
    //{
    //    float musicVolume = PlayerPrefs.GetFloat("music");
    //    float sfxVolume = PlayerPrefs.GetFloat("sfx");
    //    musicSlider.value = musicVolume;
    //    sfxSlider.value = sfxVolume;
    //    myMixer.SetFloat("music", Mathf.Log10(musicVolume) * 20);
    //    myMixer.SetFloat("sfx", Mathf.Log10(sfxVolume) * 20);
    //}

    public void PlaySFX(string name)
    {
        SoundData sound = AllSfxSounds.Find(s => s.name == name);

        if (sound == null)
        {
            Debug.Log("Sound not found: " + name);
            return;
        }
            
        sfxSource.PlayOneShot(sound.soundClip);
        
    }
    public void SetMasterVolume()
    {
        if (masterSlider == null)
        {
            Debug.Log("No master slider assigned!");
            return;
        }

        float volume = masterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Master", volume);
    }

    public void SetMusicVolume()
    {
        if (musicSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }

        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SetSfxVolume()
    {
        if (sfxSlider == null)
        {
            Debug.Log("No music slider");
            return;
        }

        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
    }
}
