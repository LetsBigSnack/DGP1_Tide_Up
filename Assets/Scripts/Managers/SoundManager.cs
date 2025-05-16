using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<SoundData> allSfxSounds;
    [SerializeField] private List<SoundData> allAtmosphereSounds;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource atmosphereSource;
    [SerializeField] private float fadeDuration;

    [Header("Volumes")]
    [SerializeField] private float masterVolume = 0.3f;
    [SerializeField] private float musicVolume = 0.001f;
    [SerializeField] private float sfxVolume = 0.7f;
    [SerializeField] private float dialogueVolume = 0.7f;

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
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        SetDialogueVolume(dialogueVolume);
    }

    public void PlaySFX(string name)
    {
        SoundData sound = allSfxSounds.Find(s => s.name == name);

        if (sound == null)
        {
            Debug.Log("SFX Sound not found: " + name);
            return;
        }

        sfxSource.volume = sound.volume;
        sfxSource.PlayOneShot(sound.soundClip);
        
    }

    public void ChangeAtmosphere(string name)
    {
        SoundData sound = allAtmosphereSounds.Find(s => s.name == name);

        if(sound == null)
        {
            Debug.Log("Atmos Sound not found: " + name);
            return;
        }

        StartCoroutine(CrossfadeAtmosphere(sound));
    }
    private IEnumerator CrossfadeAtmosphere(SoundData newSound)
    {
        if (atmosphereSource.isPlaying)
        {
            float startVolume = atmosphereSource.volume;

            for (float time = 0; time < fadeDuration; time += Time.deltaTime)
            {
                atmosphereSource.volume = Mathf.Lerp(startVolume, 0, time / fadeDuration);
                yield return null;
            }

            atmosphereSource.Stop();
            atmosphereSource.volume = startVolume;
        }

        atmosphereSource.clip = newSound.soundClip;
        atmosphereSource.volume = 0;
        atmosphereSource.loop = true;
        atmosphereSource.Play();

        for (float time = 0; time < fadeDuration; time += Time.deltaTime)
        {
            atmosphereSource.volume = Mathf.Lerp(0, newSound.volume, time / fadeDuration);
            yield return null;
        }

        atmosphereSource.volume = newSound.volume;
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
    public void SetDialogueVolume(float volume)
    {
        myMixer.SetFloat("Dialogue", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Dialogue", volume);
    }
}
