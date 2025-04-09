using System;

namespace Data
{
    [Serializable]
    public class QuestItemInstance : ItemInstance
    {
        public QuestItemData QuestItemData => (QuestItemData)ItemData;
        
        public QuestItemInstance(QuestItemData itemData) : base(itemData)
        {
            
        }
        
        public string GetUse()
        {
            return QuestItemData.GetUse();
        }
        
    }
}