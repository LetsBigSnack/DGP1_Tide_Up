using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TextToSpeechLetterData
{
    [SerializeField]
    public char letter;
    [SerializeField]
    public AudioClip clip;
}

public enum Emotion
{
    Angry,
    Happy
}

public class TextToSpeechManager : MonoBehaviour
{

    public static Dictionary<Emotion, char> EmotionSymbols = new Dictionary<Emotion, char>()
    {
        { Emotion.Angry, '%'},
        { Emotion.Happy, '$' },
    };
    
    
    
    public static TextToSpeechManager Instance;

    [SerializeField]
    private float timeBetweenLetters;
    [SerializeField]
    private float timeAfterWord;
    [SerializeField][Range(0f, 0.01f)]
    private float pitchFlactuation;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private TextToSpeechLetterData[] letters;
    

    public static event Action<char> OnTranslateLetterValueChanged;

    private bool _isTalking;
    private Dictionary<char, AudioClip> _letterSoundClips = new Dictionary<char, AudioClip>();
    private Dictionary<char, Action> _emotionAnimations = new Dictionary<char, Action>();

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(TextToSpeechLetterData data in letters)
        {
            _letterSoundClips.Add(data.letter, data.clip);
        }

        //TODO fill the emotionthings with real animations
        _emotionAnimations.Add(EmotionSymbols[Emotion.Angry], ExpressAngry);
        _emotionAnimations.Add(EmotionSymbols[Emotion.Happy], ExpressHappy);
    }

    public void ExpressAngry()
    {
        Debug.Log("I'm angry!");
    }

    public void ExpressHappy()
    {
        Debug.Log("I'm Happy!");
    }

    public void TranslateTextToAudio(string text)
    {
        if (_isTalking)
        {
            return;
        }
        StartCoroutine(TranslateToAudio(text));
    }

    public void PlayLetter(char letter)
    {
        letter = char.ToUpper(letter);
        
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = _letterSoundClips[letter];
        audioSource.pitch = UnityEngine.Random.Range(audioSource.pitch - pitchFlactuation, audioSource.pitch + pitchFlactuation);
        audioSource.Play();
    }

    public IEnumerator TranslateToAudio(string text)
    {
        _isTalking = true;
        for(int i = 0; i < text.Length; i++)
        {
             if (!_letterSoundClips.ContainsKey(Char.ToUpper(text[i])))
            {
                if (PlayEmotion(text[i]))
                {
                    Debug.Log("I'm animating!");
                }
                else
                {
                    yield return new WaitForSeconds(timeAfterWord);
                }
            }
            else 
            {
                PlayLetter(Char.ToUpper(text[i]));
                yield return new WaitForSeconds(timeBetweenLetters);
            }

            OnTranslateLetterValueChanged(text[i]);
        }
        _isTalking = false;        
    }

    public Dictionary<char, Action> GetEmotionDictionary()
    {
        return _emotionAnimations;
    }

    private bool PlayEmotion(char emotionSymbol)
    {
        if (_emotionAnimations.ContainsKey(emotionSymbol))
        {
            _emotionAnimations[emotionSymbol].Invoke();
            return true;
        }
        return false;
    }
}
