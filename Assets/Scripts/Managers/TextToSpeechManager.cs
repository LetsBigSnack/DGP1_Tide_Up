using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    private List<TextToSpeechLetterData> letters;

    public static Dictionary<Emotion, char> EmotionSymbols = new Dictionary<Emotion, char>()
    {
        { Emotion.Angry, '%'},
        { Emotion.Happy, '$'},
    };

    public static event Action<char> OnTranslateLetterValueChanged;

    private bool _isTalking;
    private Dictionary<char, Action> _emotionAnimations = new Dictionary<char, Action>();

    private Coroutine _talkCoroutine;

    public bool IsTalking
    {
        get { return _isTalking; }
        set { _isTalking = value; }
    }

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

        //TODO fill the emotionthings with real animations
        _emotionAnimations.Add(EmotionSymbols[Emotion.Angry], ExpressAngry);
        _emotionAnimations.Add(EmotionSymbols[Emotion.Happy], ExpressHappy);
    }

    //todo expand on expressions
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

        _isTalking = true;
        _talkCoroutine = StartCoroutine(TranslateToAudio(text));
    }

    public void PlayLetter(char letter)
    {
        letter = char.ToUpper(letter);
        
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        TextToSpeechLetterData letterData = letters.Where(l => l.letter == letter).FirstOrDefault();
        audioSource.clip = letterData.clip;
        audioSource.pitch = UnityEngine.Random.Range(audioSource.pitch - pitchFlactuation, audioSource.pitch + pitchFlactuation);
        audioSource.Play();
    }

    public IEnumerator TranslateToAudio(string text)
    {
        _isTalking = true;
        for (int i = 0; i < text.Length; i++)
        {
            if(!_isTalking)
            {
                yield break;
            }
            if (!LetterExists(text[i]))
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
                PlayLetter(text[i]);
                yield return new WaitForSeconds(timeBetweenLetters);
            }

            OnTranslateLetterValueChanged?.Invoke(text[i]);
        }
        _isTalking = false;        
    }

    public void StopTalking()
    {
        if(_talkCoroutine == null)
        {
            return;
        }
        StopCoroutine(_talkCoroutine);
        _talkCoroutine = null;
    }

    private bool LetterExists(char letter)
    {
        letter = Char.ToUpper(letter);
        return letters.Exists(l => l.letter == letter);
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
