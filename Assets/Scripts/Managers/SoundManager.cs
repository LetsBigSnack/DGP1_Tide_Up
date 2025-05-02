using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundData> AllSfxSounds;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioSource sfxSource;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Setup();
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

    //TODO: Setup should change to function above
    private void Setup()
    {
        Debug.Log("Setup for sounds done");
        SetMasterVolume(0.3f);
        SetMusicVolume(0.001f);
        SetSfxVolume(0.7f);
    }

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
    public void SetMasterVolume(float volume)
    {
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log("Volume for music = "+ volume);
        //Debug.Log(volume);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SetSfxVolume(float volume)
    {
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFX", volume);
    }
}
