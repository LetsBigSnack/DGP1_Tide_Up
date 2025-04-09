using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public abstract class UIReUpCyclerSubMenu : MonoBehaviour
    {
        [SerializeField] private ReUpcyclerType type;
        public ReUpcyclerType ReUpCyclerMenuType { get => type; set => type = value; }

        public abstract void OpenMenu();

        public abstract void CloseMenu();

    }
}
