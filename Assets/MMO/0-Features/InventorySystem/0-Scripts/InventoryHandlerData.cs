using System;
using System.Collections.Generic;
using SteelBox;

namespace MMO.InventorySystem
{
    public static class InventoryHandlerData
    {
        public static event Action<int, string, int> S_OnGiveItem;
        public static void S_GiveItem(int clientId, string itemId, int amount) => S_OnGiveItem?.Invoke(clientId, itemId, amount);

        public static event ActionValValValRef<int, string, int, bool> OnCanGiveItem;
        public static void CanGiveItem(int clientId, string itemId, int amount, ref bool canGive) => OnCanGiveItem?.Invoke(clientId, itemId, amount, ref canGive);
        
        public static event Action<int, string, int> S_OnTakeItem;
        public static void S_TakeItem(int clientId, string itemId, int amount) => S_OnTakeItem?.Invoke(clientId, itemId, amount);
        
        public static event ActionValValValRef<int, string, int, bool> S_OnHasItem;
        public static void S_HasItem(int clientId, string itemId, int amount, ref bool hasItem) => S_OnHasItem?.Invoke(clientId, itemId, amount, ref hasItem);
        
        public static event Action<InventoryItemSlot[]> C_OnInventoryChanged;
        public static void C_InventoryChanged(InventoryItemSlot[] items) => C_OnInventoryChanged?.Invoke(items);
        
        public static event Action<int, List<InventoryItemSlot>> C_OnDisplayBag;
        public static void C_DisplayBag(int bagNobId, List<InventoryItemSlot> items) => C_OnDisplayBag?.Invoke(bagNobId, items);
        
        public static event Action<int, string> C_OnRetrieveItemFromBag;
        public static void C_RetrieveItemFromBag(int bagNobId, string itemId) => C_OnRetrieveItemFromBag?.Invoke(bagNobId, itemId);
    }
}