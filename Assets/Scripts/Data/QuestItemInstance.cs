using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class QuestItemInstance : ItemInstance
    {
        public QuestItemData QuestItemData => (QuestItemData)ItemData;
        
        private List<TrashMaterialData> _usedTrashMaterials;
        
        public QuestItemInstance(QuestItemData itemData, List<TrashMaterialData> recipeIngredients) : base(itemData)
        {
            _usedTrashMaterials = recipeIngredients;
        }
        
        public string GetUse()
        {
            return QuestItemData.GetUse();
        }

        public override List<TrashMaterialData> GetMaterials()
        {
            return _usedTrashMaterials;
        }
        
    }
}