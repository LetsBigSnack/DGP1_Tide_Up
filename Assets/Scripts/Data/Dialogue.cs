using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public enum DialogueType
    {
        Intro,
        Quest
    }
    
    
    [Serializable]
    public class Dialogue
    {
       
        private List<string> _dialogueContent;
        [JsonIgnore] 
        private int _currentDialogueState = 0;
        [JsonIgnore] 
        private bool _isDialogueFinished = false;
        
        private DialogueType _dialogueType;
        private bool _hasDialogueStarted = false;
        
        
        
        public DialogueType DialogueType
        {
            get => _dialogueType;
            set => _dialogueType = value;
        }

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
            
            string result = _dialogueContent[_currentDialogueState];
            
            if (_currentDialogueState >= _dialogueContent.Count-1)
            {
                _isDialogueFinished = true;
            }
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
