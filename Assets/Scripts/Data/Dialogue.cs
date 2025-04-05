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
            NextDialogueContent();
            return result;
        }

        private void NextDialogueContent()
        {
            _currentDialogueState++;
            if (_currentDialogueState >= _dialogueContent.Count)
            {
                _isDialogueFinished = true;
            }
        }

        public void ResetDialogue()
        {
            _currentDialogueState = 0;
        }
        
    }
}
