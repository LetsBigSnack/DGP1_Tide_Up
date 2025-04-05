using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class Dialogue
    {
       
        public List<string> dialogueContent;

        [JsonIgnore] 
        private int _currentDialogueState = 0;

        public int CurrentDialogueState
        {
            get => _currentDialogueState;
            set => _currentDialogueState = value;
        }

        private bool _isDialogueFinished = false;

        public bool IsDialogueFinished
        {
            get => _isDialogueFinished;
            set => _isDialogueFinished = value;
        }
    
        
        
        
        
        
        public string GetCurrentDialogue()
        {
            
            string result = dialogueContent[_currentDialogueState];
            NextDialogueContent();
            return result;
        }

        private void NextDialogueContent()
        {
            _currentDialogueState++;
            if (_currentDialogueState >= dialogueContent.Count)
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
