using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum ItemQuality
    {
        Quest,
        High,
        Normal,
    }

    [Serializable]
    public class ItemInstance
    {
        [SerializeField]
        private ItemData itemData;
        [SerializeField]
        private ItemQuality itemQuality = ItemQuality.Normal;

        public ItemData ItemData
        {
            get => itemData;
            set => itemData = value;
        }

        public ItemQuality ItemQuality
        {
            get => itemQuality;
            set => itemQuality = value;
        }

        public ItemInstance(ItemData itemData)
        {
            this.itemData = itemData;
        }

        public virtual List<TrashMaterialData> GetMaterials()
        {
            return itemData.Materials;
        }
        
        
        
    }
}