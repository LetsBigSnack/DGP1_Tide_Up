using System;
using System.Collections;
using Data;
using Unity.VisualScripting;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "FishingMiniGame", menuName = "Scriptable Objects/MiniGame/FishingMiniGame", order = 3)]
    public class FishingMiniGame : MiniGame
    {
        [SerializeField] private float duration = 5f;
        [SerializeField] private float moveSpeed = 0.4f;           // Player bar speed// Trash speed
        [SerializeField] private float trackingTolerance = 0.05f;  // Distance to count as tracking
        [SerializeField] private float successThreshold = 0.7f;
        
        
        [SerializeField] private float trashMoveSpeed = 0.4f;  
        [SerializeField] private float trashMoveRangeMin = -0.3f;
        [SerializeField] private float trashMoveRangeMax = 0.3f;
        [SerializeField] private float trashSpeedMin = 0.1f;
        [SerializeField] private float trashSpeedMax = 0.7f;
        [SerializeField] private float directionIntervalMin = 0.5f;
        [SerializeField] private float directionIntervalMax = 1.5f;
        [SerializeField] private float directionChangeChance = 0.6f;
        [SerializeField] private float defaultSpeed = 0.4f;
        
        
        private float _playerPosition;
        private float _trashPosition;
        private float _trackingTime;
        private bool _isActive;
        
        private Action<float, float> OnMiniGameVertProgress = delegate { };


        public override IEnumerator StartMiniGame(Action<bool> callback)
        {
            trashMoveSpeed = defaultSpeed;
            _playerPosition = 0.5f;
            _trashPosition = 0.5f;
            _trackingTime = 0f;
            _isActive = true;

            MiniGameController.OnMoveBar += OnMoveBar;
            UISubscribe();
            
            float elapsed = 0f;
            float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;

            UIMiniGameManager.Instance.Initialize(_playerPosition, _trashPosition, Player.Instance.gameObject.transform,
                MiniGameType.Fishing, 0.0f, successThreshold); 

            
            float changeDirectionTimer = 0f;
            float directionChangeInterval = UnityEngine.Random.Range(directionIntervalMin, directionIntervalMax); // Randomize when direction might change
            float score = 0;
            
            while (elapsed < duration)
            {
                changeDirectionTimer += Time.deltaTime;
                if (changeDirectionTimer >= directionChangeInterval)
                {
                    if (UnityEngine.Random.value < directionChangeChance) 
                    {
                        direction *= -1f;
                    }

                    // Slight random variation in speed
                    trashMoveSpeed = Mathf.Clamp(trashMoveSpeed + UnityEngine.Random.Range(trashMoveRangeMin, trashMoveRangeMax), trashSpeedMin, trashSpeedMax);

                    // Reset timer
                    changeDirectionTimer = 0f;
                    directionChangeInterval = UnityEngine.Random.Range(directionIntervalMin, directionIntervalMax); // Randomize when direction might change

                }

                // Move trash
                _trashPosition += direction * trashMoveSpeed * Time.deltaTime;

                if (_trashPosition > 1f || _trashPosition < 0f)
                {
                    direction *= -1f;
                    _trashPosition = Mathf.Clamp(_trashPosition, 0f, 1f);
                }

                // Track if within tolerance
                if (Mathf.Abs(_playerPosition - _trashPosition) <= trackingTolerance)
                {
                    _trackingTime += Time.deltaTime;
                }
                
                score = Mathf.Clamp01(_trackingTime / duration);
                // Emit current positions
                OnMiniGameProgress?.Invoke(_playerPosition, _trashPosition);
                OnMiniGameVertProgress?.Invoke(score, successThreshold);
                elapsed += Time.deltaTime;
                yield return null;
            }

            MiniGameController.OnMoveBar -= OnMoveBar;
            UIMiniGameManager.Instance.Hide();
            UIUnsubscribe();
            _isActive = false;
            
            Debug.Log($"[FishingMiniGame] Score: {score:F2}");
        
            callback(score > successThreshold);
        }

        protected override void OnInteract()
        {
            throw new NotImplementedException();
        }

        private void OnMoveBar(float input)
        {
            if (!_isActive) return;

            _playerPosition += input * moveSpeed * Time.deltaTime;
            _playerPosition = Mathf.Clamp01(_playerPosition);
        }
        
        
        protected override void UISubscribe()
        {
            OnMiniGameProgress += UIMiniGameManager.Instance.UpdateHorSlider;
            OnMiniGameVertProgress += UIMiniGameManager.Instance.UpdateVerSlider;
        }

        protected override void UIUnsubscribe()
        {
            OnMiniGameProgress -= UIMiniGameManager.Instance.UpdateHorSlider;
            OnMiniGameVertProgress -= UIMiniGameManager.Instance.UpdateVerSlider;
        }
    }
}
