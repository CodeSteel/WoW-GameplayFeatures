using System.Collections.Generic;
using FishNet.Object;
using MMO.InventorySystem;
using SteelBox;

namespace MMO.Quest
{
    public class QuestStep_CollectItems : QuestStep
    {
        public SerializedDictionary<string, int> ItemsToCollect;
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                InventoryHandlerData.S_OnGiveItem += S_OnGiveItem;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                InventoryHandlerData.S_OnGiveItem -= S_OnGiveItem;
            }
        }

        [Server]
        private void S_OnGiveItem(int clientId, string itemId, int amount)
        {
            if (clientId != OwnerId) return;
            if (!ItemsToCollect.ContainsKey(itemId)) return;
            
            ItemsToCollect[itemId] -= amount;
            if (ItemsToCollect[itemId] < 0)
                ItemsToCollect[itemId] = 0;

            foreach (KeyValuePair<string, int> kvp in ItemsToCollect)
                if (kvp.Value > 0) return;
            
            FinishStep();
        }
    }
}
