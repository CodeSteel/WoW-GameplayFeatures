using System;

namespace MMO.InventorySystem
{
    [Serializable]
    public class InventoryItemSlot
    {
        public string ItemId;
        public int Amount;

        public bool IsEmpty => Amount == 0;
        
        public void Clear()
        {
            Amount = 0;
            ItemId = string.Empty;
        }
    }
}