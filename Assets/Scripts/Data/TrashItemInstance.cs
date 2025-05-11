using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class TrashItemInstance : ItemInstance
    {
        public TrashData TrashData => (TrashData)ItemData;
        
        private List<TrashMaterialData> _materials = new List<TrashMaterialData>();
        
        public TrashItemInstance(TrashData itemData, bool isHighQuality) : base(itemData)
        {
            if (isHighQuality)
            {
                ItemQuality = ItemQuality.High;
                UpgradeMaterials();
            }
        }
        
        private void UpgradeMaterials()
        {
            _materials = new List<TrashMaterialData>(ItemData.Materials);
            
            if (ItemData.Materials.Count > 0)
            {
                var randomMaterial = ItemData.Materials[UnityEngine.Random.Range(0, ItemData.Materials.Count)];
                _materials.Add(randomMaterial);
            }
        }
        
        public override List<TrashMaterialData> GetMaterials()
        {
            if (ItemQuality == ItemQuality.High)
            {
                return _materials;
            }
            else
            {
                return ItemData.Materials;
            }
        }
    }
}