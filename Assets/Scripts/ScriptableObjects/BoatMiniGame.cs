using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;



 [Serializable]
public enum MiniGameButton
{
    Button1,
    Button2,
    Button3,
    Button4
}

[Serializable]
public class MiniGameNote
{
    public float timeStamp;
    public MiniGameButton button;
    public bool isHit;
    public float percentage;
}

[Serializable]
public class MiniGameTrack
{
    public List<MiniGameNote> notes;
    public float trackDuration;


    public void ResetTrack()
    {
        foreach (MiniGameNote note in notes)
        {
            note.isHit = false;
            note.percentage = note.timeStamp / trackDuration;
        }
    }
    
    
    
}


namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BoatMiniGame", menuName = "Scriptable Objects/MiniGame/BoatMiniGame", order = 3)]
    public class BoatMiniGame : MiniGame
    {

        [SerializeField] private float wiggleRoom = 0.1f;
        [SerializeField] private List<MiniGameTrack> tracks;
        [SerializeField] private float punishmentPercentage = 0.05f;
        [SerializeField] private float progressPercentage = 0f;
        [SerializeField] private float currentProgress = 0f;
        [SerializeField] private float successThreshold = 0.8f;


        public Action<float, float, float, List<MiniGameNote>> OnBoatMiniGameProgress;
        
        public List<MiniGameTrack> GetTracks() => tracks;

        public float GetStartTime() => _startTime;


        public List<MiniGameNote> hitNotes;
        
        private float _startTime;
        [SerializeField] private bool _fullCombo = true;
        private MiniGameTrack _currentTrack;
 
        
        public override IEnumerator StartMiniGame(Action<bool> callback)
        {
            
            _currentTrack = tracks[UnityEngine.Random.Range(0, tracks.Count)];
            _currentTrack.ResetTrack();
            _fullCombo = true;
            hitNotes = new List<MiniGameNote>();
            _startTime = Time.time;
            currentProgress = 0f;
            
            progressPercentage = 1.0f/_currentTrack.notes.Count;
            
            
            MiniGameController.Instance.RegisterMiniGameBoat(OnButton1, OnButton2, OnButton3, OnButton4);
            UISubscribe();

            float endTime = _startTime + _currentTrack.trackDuration;
            
            UIMiniGameManager.Instance.Initialize(_currentTrack.trackDuration,successThreshold, Player.Instance.gameObject.transform, type=MiniGameType.Boat, buttons:_currentTrack.notes);

            
            while (Time.time < endTime)
            {
                //UI updaten
                OnBoatMiniGameProgress?.Invoke((Time.time-_startTime)/_currentTrack.trackDuration, currentProgress, successThreshold, _currentTrack.notes);
                yield return null;
            }

            MiniGameController.Instance.UnregisterMiniGameBoat(OnButton1, OnButton2, OnButton3, OnButton4);
            UIMiniGameManager.Instance.Hide();
            UIUnsubscribe();

            bool success = currentProgress >= successThreshold;

            if (success)
            {
                SoundManager.Instance.PlaySFX("Success");
            }
            else
            {
                SoundManager.Instance.PlaySFX("Pick_up");
            }

            Debug.Log($"[Boat MiniGame] Result: {(success ? "Success" : "Fail")}");
            callback(success);
        }

        protected override void OnInteract()
        {
            return;
        }


        private void CheckButtonPressed(MiniGameButton button)
        {
            float currentHitTime = Time.time;
            
            MiniGameNote toHit = _currentTrack.notes.FirstOrDefault(n => Math.Abs((n.timeStamp + _startTime) - currentHitTime) < wiggleRoom && !n.isHit && n.button == button);

            if (toHit == null)
            {
                Debug.Log($"[Boat MiniGame] Button {button} not found");
                _fullCombo = false;
                currentProgress -= punishmentPercentage;
                currentProgress = Mathf.Clamp(currentProgress, 0f, 1f);
            }
            else
            { 
                toHit.isHit = true;
                currentProgress += progressPercentage;
            }
            hitNotes.Add(new MiniGameNote { timeStamp = currentHitTime-_startTime, button = button });
        }
        
    
        protected void OnButton1(bool isPressed)
        {
            if (isPressed)
            {
                CheckButtonPressed(MiniGameButton.Button1);
                Debug.Log($"[Boat MiniGame] Button 2 Pressed!");
            }
            
        }
        
        protected void OnButton2(bool isPressed)
        {
            if (isPressed)
            {
                CheckButtonPressed(MiniGameButton.Button2);
                Debug.Log($"[Boat MiniGame] Button 2 Pressed!");
            }
            
        }
        
        protected void OnButton3(bool isPressed)
        {
            if (isPressed)
            {
                CheckButtonPressed(MiniGameButton.Button3);
                Debug.Log($"[Boat MiniGame] Button 3 Pressed!");
            }
        }
        
        protected void OnButton4(bool isPressed)
        {
            if (isPressed)
            {
                CheckButtonPressed(MiniGameButton.Button4);
                Debug.Log($"[Boat MiniGame] Button 3 Pressed!");
            }
        }

        public MiniGameTrack GetCurrentTrack()
        {
            return _currentTrack;
        }
        
        protected override void UISubscribe()
        {
            OnBoatMiniGameProgress += UIMiniGameManager.Instance.OnBoatMiniGameProgress;
        }
        
        protected override void UIUnsubscribe()
        {
            OnBoatMiniGameProgress -= UIMiniGameManager.Instance.OnBoatMiniGameProgress;
        }
    }
}
