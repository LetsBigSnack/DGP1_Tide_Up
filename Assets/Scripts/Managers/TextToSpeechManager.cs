using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Data;

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
    Happy,
    Thinking,
    Surprised,
    Sad
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

    private AnimationController _anim = null;

    public static Dictionary<Emotion, char> emotionSymbols = new Dictionary<Emotion, char>()
    {
        { Emotion.Angry, '%'},
        { Emotion.Happy, '$'},
        { Emotion.Thinking, '+'},
        { Emotion.Surprised, '#'},
        { Emotion.Sad, '*'},
    };

    public static event Action<char> OnTranslateLetterValueChanged;

    private bool _isTalking;

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

    public void TranslateTextToAudio(string text, AnimationController anim)
    {
        if (_isTalking)
        {
            return;
        }

        _anim = anim; 
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

    public bool CharIsEmotion(char c)
    {
        return emotionSymbols.ContainsValue(c);
    }

    private bool PlayEmotion(char emotionSymbol)
    {
        if (emotionSymbols.ContainsValue(emotionSymbol))
        {
            var type = emotionSymbols.FirstOrDefault(x => x.Value == emotionSymbol).Key;
            _anim.PlayOnShotEmotion(type);
            return true;
        }
        return false;
    }
}
