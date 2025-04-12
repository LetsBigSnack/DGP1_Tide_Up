using System;

namespace Data
{
    [Serializable]
    public class TrashItemInstance : ItemInstance
    {
        public TrashData TrashData => (TrashData)ItemData;
        
        public TrashItemInstance(TrashData itemData, bool isHighQuality) : base(itemData)
        {
            if (isHighQuality)
            {
                ItemQuality = ItemQuality.High;
            }
        }
    }
}