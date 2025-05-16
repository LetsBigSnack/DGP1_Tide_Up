using System;
using System.Collections;
using Data;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "DiggingMiniGame", menuName = "Scriptable Objects/MiniGame/DiggingMiniGame", order = 2)]
    public class DiggingMiniGame : MiniGame
    {
        [SerializeField] private float miniGameDuration = 3.0f;
        [SerializeField] private int pressThreshold = 30;
        [SerializeField] private GameObject sweatVFXPrefab;

        private int _pressCount;
        private float _startTime;

        public override IEnumerator StartMiniGame(Action<bool> callback)
        {
            SoundManager.Instance.PlaySFX("Dig_start");

            _pressCount = 0;
            _startTime = Time.time;

            MiniGameController.Instance.RegisterMiniGameInteract(OnInteract);
            UISubscribe();

            float endTime = _startTime + miniGameDuration;

            UIMiniGameManager.Instance.Initialize(miniGameDuration, pressThreshold, Player.Instance.transform);

            int countHelp = 0;
            while (Time.time < endTime && !(_pressCount >= pressThreshold))
            {
                float progress = Mathf.Clamp01((float)_pressCount / pressThreshold);
                OnMiniGameProgress?.Invoke(progress, 1.0f); // 1.0f == 100%
                if(Time.time <= endTime - 2.4f && Time.time >= endTime - 2.5f && countHelp == 0)
                {
                    SoundManager.Instance.PlaySFX("Dig_strugle");
                    countHelp++;
                }
                yield return null;
            }

            MiniGameController.Instance.UnregisterMiniGameInteract(OnInteract);
            UIMiniGameManager.Instance.Hide();
            UIUnsubscribe();

            bool success = _pressCount >= pressThreshold;
            Debug.Log($"[DiggingMiniGame] Result: {(success ? "Success" : "Fail")} with {_pressCount} presses.");
            callback(success);
        }

        protected override void OnInteract()
        {
            _pressCount++;

            // Optional: spawn VFX at player location
            if (sweatVFXPrefab != null)
            {
                GameObject vfx = GameObject.Instantiate(sweatVFXPrefab, Player.Instance.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                GameObject.Destroy(vfx, 2f); // Cleanup
            }

            Debug.Log($"[DiggingMiniGame] Pressed! Count: {_pressCount}");
        }
    }
}
