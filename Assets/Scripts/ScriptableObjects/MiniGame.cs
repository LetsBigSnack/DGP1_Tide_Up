using System;
using System.Collections;
using UnityEngine;

namespace Data
{
    public abstract class MiniGame : ScriptableObject
    {
        public MiniGameType type;
        
        public Action<float, float> OnMiniGameProgress = delegate { };
        
        public abstract IEnumerator StartMiniGame(Action<bool> callback);

        protected abstract void OnInteract();

        protected virtual void UISubscribe()
        {
            OnMiniGameProgress += UIMiniGameManager.Instance.UpdateHorSlider;
        }

        protected virtual void UIUnsubscribe()
        {
            OnMiniGameProgress -= UIMiniGameManager.Instance.UpdateHorSlider;
        }
    }
}