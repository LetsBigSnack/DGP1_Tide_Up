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
        
        [SerializeField] private bool decay = false;
        [SerializeField] private float decayRate = 0.01f;
        [SerializeField] private float trackRate = 0.3f;
        
        [SerializeField] private bool isHoldingLeft = true;
        [SerializeField] private bool isHoldingRight = true;
        
        private float _playerPosition;
        private float _trashPosition;
        private bool _isActive;
        private float _score;
        
        private Action<float, float> OnMiniGameVertProgress = delegate { };


        public override IEnumerator StartMiniGame(Action<bool> callback)
        {
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlaySFX("Fishing_start_swoosh");

            trashMoveSpeed = defaultSpeed;
            _playerPosition = 0.5f;
            _trashPosition = 0.5f;
            _score = 0f;
            _isActive = true;
            isHoldingLeft = false;
            isHoldingRight = false;
            
            //MiniGameController.OnMoveBar += OnMoveBar;
            MiniGameController.Instance.RegisterMiniGameInteract(OnInteract, OnMoveLeft, OnMoveRight);
            UISubscribe();
            
            float elapsed = 0f;
            float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;

            UIMiniGameManager.Instance.Initialize(_playerPosition, _trashPosition, Player.Instance.gameObject.transform,
                MiniGameType.Fishing, 0.0f, successThreshold); 

            
            float changeDirectionTimer = 0f;
            float directionChangeInterval = UnityEngine.Random.Range(directionIntervalMin, directionIntervalMax); // Randomize when direction might change
   
            int countHelp = 0;
            while (elapsed < duration)
            {
                if(elapsed >= 0.8f && countHelp == 0)
                {
                    SoundManager.Instance.PlaySFX("Fishing_start_plop");
                    countHelp++;
                }
                if (elapsed >= 2.2f && countHelp == 1)
                {
                    SoundManager.Instance.PlaySFX("Fishing_idle");
                    countHelp++;
                }

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

                if (isHoldingRight != isHoldingLeft)
                {
                    if (isHoldingRight)
                    {
                        _playerPosition += moveSpeed * Time.deltaTime;
                        _playerPosition = Mathf.Clamp01(_playerPosition);
                    }
                    else
                    {
                        _playerPosition -= moveSpeed * Time.deltaTime;
                        _playerPosition = Mathf.Clamp01(_playerPosition);
                    }
                }
                

                // Track if within tolerance
                if (Mathf.Abs(_playerPosition - _trashPosition) <= trackingTolerance)
                {
                    _score = Mathf.Min(1f, _score + trackRate * Time.deltaTime);
                }
                else
                {
                    if (decay)
                    {
                        _score = Mathf.Max(0f, _score - decayRate * Time.deltaTime);
                    }
                }
                
               
                OnMiniGameProgress?.Invoke(_playerPosition, _trashPosition);
                OnMiniGameVertProgress?.Invoke(_score, successThreshold);
                elapsed += Time.deltaTime;

                yield return null;
            }

            //MiniGameController.OnMoveBar -= OnMoveBar;
            MiniGameController.Instance.UnregisterMiniGameInteract(OnInteract, OnMoveLeft, OnMoveRight);
            UIMiniGameManager.Instance.Hide();
            UIUnsubscribe();
            _isActive = false;

            Debug.Log($"[FishingMiniGame] Score: {_score:F2}");
        
            callback(_score > successThreshold);

            if (_score > successThreshold)
            {
                SoundManager.Instance.PlaySFX("Success");
            }
            else
            {
                SoundManager.Instance.PlaySFX("Pick_up");
            }

            yield return new WaitForSeconds(1.2f);
            SoundManager.Instance.PlaySFX("Fishing_start_swoosh");
        }

        protected override void OnInteract()
        {
            return;
        }
        
        private void OnMoveLeft(bool holding)
        {
            isHoldingLeft = holding;
            
        }
        
        private void OnMoveRight(bool holding)
        {
            isHoldingRight = holding;
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
