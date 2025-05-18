using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Dialogue
    {
        private List<string> _dialogueContent;
        [JsonIgnore] 
        private int _currentDialogueState = 0;
        [JsonIgnore] 
        private bool _isDialogueFinished = false;
        private bool _hasDialogueStarted = false;
        
        public List<string> DialogueContent
        {
            get => _dialogueContent;
            set => _dialogueContent = value;
        }
        
        public int CurrentDialogueState
        {
            get => _currentDialogueState;
            set => _currentDialogueState = value;
        }
        
        public bool IsDialogueFinished
        {
            get => _isDialogueFinished;
            set => _isDialogueFinished = value;
        }
        
        
        public Dialogue()
        {
            this._currentDialogueState = 0;
            this._isDialogueFinished = false;
        }
        
        
        public Dialogue(Dialogue dialogueContent)
        {
            this._dialogueContent = new List<string>(dialogueContent.DialogueContent);
            this._currentDialogueState = 0;
            this._isDialogueFinished = false;
        }
        
        public string GetCurrentDialogue()
        {

            if (_dialogueContent == null || _dialogueContent.Count == 0)
            {
                return string.Empty;
            }

            if (_currentDialogueState >= _dialogueContent.Count-1)
            {
                _isDialogueFinished = true;
            }

            string result = _dialogueContent[_currentDialogueState];

            return result;
        }

        public void NextDialogueContent()
        {
            if (!_hasDialogueStarted)
            {
                _hasDialogueStarted = true;
                return;
            }
            _currentDialogueState++;
        }

        public void ResetDialogue()
        {
            _hasDialogueStarted = false;
            _isDialogueFinished = false;
            _currentDialogueState = 0;
        }
        
    }
}
