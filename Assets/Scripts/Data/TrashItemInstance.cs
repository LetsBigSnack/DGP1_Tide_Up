using System;

namespace Data
{
    [Serializable]
    public class TrashItemInstance : ItemInstance
    {
        public TrashData TrashData => (TrashData)ItemData;
        
        public TrashItemInstance(TrashData itemData) : base(itemData)
        {
            
        }
    }
}