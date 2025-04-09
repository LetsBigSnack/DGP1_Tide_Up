using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public abstract class UIJournalSubMenu : MonoBehaviour
    {
        [SerializeField] private JournalType type;
        public JournalType JournalType { get => type; set => type = value; }

        public abstract void OpenMenu();

        public abstract void CloseMenu();

    }
}
