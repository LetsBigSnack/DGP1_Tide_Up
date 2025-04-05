using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

[Serializable]
public class TextToSpeechLetterData
{
    [SerializeField]
    public char letter;
    [SerializeField]
    public AudioClip clip;
}

public class TextToSpeechManager : MonoBehaviour
{
    public float timeBetweenLetters;
    public AudioSource audioSource;
    public TMP_InputField inputField;
    public bool isTalking;
    [SerializeField]
    private TextToSpeechLetterData[] letters;

    private Dictionary<char, AudioClip> _letterSoundClips = new Dictionary<char, AudioClip>();

    public void Awake()
    {
        foreach(TextToSpeechLetterData data in letters)
        {
            _letterSoundClips.Add(data.letter, data.clip);
        }
    }

    public void TranslateTextToAudio()
    {
        if (isTalking)
        {
            return;
        }
        StartCoroutine(TranslateToAudio(inputField.text.ToUpper()));
    }

    public void PlayLetter(char letter)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = _letterSoundClips[letter];
        audioSource.pitch = UnityEngine.Random.Range(audioSource.pitch - 0.01f, audioSource.pitch + 0.01f);
        audioSource.Play();
    }

    public IEnumerator TranslateToAudio(string text)
    {
        isTalking = true;
        for(int i = 0; i < text.Length; i++)
        {
            if (!_letterSoundClips.ContainsKey(text[i])){
                yield return new WaitForSeconds(timeBetweenLetters);
            }
            else
            {
                PlayLetter(text[i]);
                yield return new WaitForSeconds(timeBetweenLetters);
            }
        }
        isTalking = false;        
    }
}
