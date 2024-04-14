using System;
using UnityEngine;

namespace MMO.InventorySystem
{
    [Serializable]
    public class Inventory
    {
        public InventoryItemSlot[] Slots;

        public Inventory()
        {
        }
        
        public Inventory(int slotLimit)
        {
            Slots = new InventoryItemSlot[slotLimit];
            for (int i = 0; i < slotLimit; i++)
                Slots[i] = new InventoryItemSlot();
        }

        public bool CanGiveItem(string itemId, int amount)
        {
            InventoryItemSo itemSo = null;
            ItemDatabaseHandlerData.GetItemById(itemId, ref itemSo);

            int amountToAdd = amount;
            int itemStackLimit = itemSo.StackLimit;
            int emptySlots = 0;

            // first try to add as many as we can to existing slots with same item
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty)
                {
                    emptySlots++;
                    continue;
                }
                if (Slots[i].ItemId == itemId && Slots[i].Amount < itemSo.StackLimit)
                {
                    amountToAdd -= itemSo.StackLimit - Slots[i].Amount;
                }
            }

            if (amountToAdd == 0) return true;
            
            // how many slots do we need to take up and do we have that many empty slots?
            int amountOfSlotsNeeded = amountToAdd / itemStackLimit;
            if (amountOfSlotsNeeded <= emptySlots) return true;
            
            return false;
        }

        public void GiveItem(string itemId, int amount)
        {
            InventoryItemSo itemSo = null;
            ItemDatabaseHandlerData.GetItemById(itemId, ref itemSo);

            int amountToAdd = amount;
            int itemStackLimit = itemSo.StackLimit;

            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty) continue;
                if (Slots[i].ItemId == itemId && Slots[i].Amount < itemSo.StackLimit)
                {
                    int freeSpace = Mathf.Min(amountToAdd, itemSo.StackLimit - Slots[i].Amount);
                    amountToAdd -= freeSpace;
                    Slots[i].Amount += freeSpace;
                    if (amountToAdd == 0) return;
                }
            }
            
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty)
                {
                    int addAmountLimit = Mathf.Min(amountToAdd, itemStackLimit);
                    amountToAdd -= addAmountLimit;
                    Slots[i].Amount += addAmountLimit;
                    Slots[i].ItemId = itemId;
                    if (amountToAdd == 0) return;
                }
            }
        }

        public bool HasItem(string itemId, int amount = 1)
        {
            int amountToFind = amount;
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty) continue;
                if (Slots[i].ItemId == itemId)
                {
                    amountToFind -= Slots[i].Amount;
                    if (amountToFind == 0)
                        return true;
                }
            }

            return false;
        }

        public void TakeItem(string itemId, int amount = 1)
        {
            int amountToTake = amount;
            for (int i = 0; i < Slots.Length; i++)
            {
                if (Slots[i].IsEmpty) continue;
                if (Slots[i].ItemId == itemId)
                {
                    if (amountToTake <= Slots[i].Amount)
                    {
                        Slots[i].Amount -= amountToTake;
                        amountToTake = 0;
                        if (Slots[i].Amount == 0)
                            Slots[i].Clear();
                    }
                    else
                    {
                        amountToTake -= Slots[i].Amount;
                        Slots[i].Clear();
                    }
                    
                    if (amountToTake == 0) return;
                }
            }
        }
    }
}