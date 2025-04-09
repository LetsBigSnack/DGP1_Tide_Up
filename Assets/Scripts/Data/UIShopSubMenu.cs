using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public abstract class UIShopSubMenu : MonoBehaviour
    {
        [SerializeField] private ShopType type;
        public ShopType ShopType { get => type; set => type = value; }

        public abstract void OpenMenu();

        public abstract void CloseMenu();
    }

}
