using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    
    [Serializable]
    public class ItemInstance
    {
        [SerializeField]
        private ItemData itemData;

        public ItemData ItemData
        {
            get => itemData;
            set => itemData = value;
        }

        public ItemInstance(ItemData itemData)
        {
            this.itemData = itemData;
        }

        public virtual List<TrashMaterialData> GetMaterials()
        {
            return itemData.materials;
        }
        
    }
}