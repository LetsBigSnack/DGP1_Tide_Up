using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "MiniGame", menuName = "Scriptable Objects/MiniGame/PickUpMiniGame", order = 1)]
    public class PickUpMiniGame : MiniGame
    {
        [SerializeField] private float miniGameDuration = 2.0f;
        [SerializeField] private float miniGameGoalTime = 1.75f;
        [SerializeField] private float miniGameMargin = 0.1f;
        [SerializeField] private bool missPlaysFulltime = true;
        
        
        private bool _isPressed = false;
        private bool _isSuccess = false;
        private float _startTime;
        
        public override IEnumerator StartMiniGame(Action<bool> callback)
        {
            _isPressed = false;
            _isSuccess = false;
            
            _startTime = Time.time;
            Debug.Log($"[MiniGame] PRESS E NOW! Target Time: {_startTime}");
            
            MiniGameController.Instance.RegisterMiniGameInteract(OnInteract);
            UISubscribe();
            
            float endTime = _startTime + miniGameDuration;
            float goalPercentage = miniGameGoalTime / miniGameDuration;
            
            UIMiniGameManager.Instance.Initialize(miniGameDuration, goalPercentage, Player.Instance.gameObject.transform);
            
            while (Time.time < endTime)
            {

                if (_isSuccess || (!missPlaysFulltime && _isPressed))
                {
                    break;
                }
                
                
                if (!_isPressed)
                {
                    OnMiniGameProgress?.Invoke((Time.time-_startTime)/miniGameDuration, goalPercentage);
                }
                
                Debug.Log($"[MiniGame] Current Time: {Time.time:F2} | Time Left: {(endTime - Time.time):F2}s");
                yield return null;
            }
            
            
            MiniGameController.Instance.UnregisterMiniGameInteract(OnInteract);
            UIMiniGameManager.Instance.Hide();
            UIUnsubscribe();
            
            if (!_isPressed)
                Debug.Log("[MiniGame] Failed: No input received.");
            
            callback(_isSuccess);
        }

        protected override void OnInteract()
        {
            if (_isPressed) return;

            _isPressed = true;
            float pressTime = Time.time;
            float goalTime = _startTime + miniGameGoalTime;
            float diff = Mathf.Abs(pressTime - goalTime);
            _isSuccess = diff <= miniGameMargin;
            
            Debug.Log($"[MiniGame] Pressed at {pressTime} — Target: {goalTime}, Diff: {diff}, Success: {_isSuccess}");
        }
        
    }
}