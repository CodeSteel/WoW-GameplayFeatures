using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using MMO.InteractionSystem;
using SteelBox;

namespace MMO.InventorySystem
{
    public class InventoryBag : BaseNetworkBehaviour, IInteraction
    {
        public List<InventoryItemSlot> ItemsToPickup = new List<InventoryItemSlot>();
        public int InteractionDelay = 0;
    
        private readonly SyncList<InventoryItemSlot> _itemsInBag = new SyncList<InventoryItemSlot>();
        
        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
            if (IsClientInitialized)
            {
                InventoryHandlerData.C_OnRetrieveItemFromBag -= C_OnRetrieveItemFromBag;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _itemsInBag.AddRange(ItemsToPickup);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            InventoryHandlerData.C_OnRetrieveItemFromBag += C_OnRetrieveItemFromBag;
        }

        [Client]
        private void C_OnRetrieveItemFromBag(int bagNobId, string item)
        {
            if (bagNobId != ObjectId) return;
            S_RpcRetrieveItemFromBag(item);
        }

        [ServerRpc(RequireOwnership = false)]
        private void S_RpcRetrieveItemFromBag(string item, NetworkConnection conn = null)
        {
            InventoryItemSlot slot = _itemsInBag.FirstOrDefault((x) => x.ItemId == item);
            if (slot == default) return;
            bool canGive = false;
            InventoryHandlerData.CanGiveItem(OwnerId, item, slot.Amount, ref canGive);
            if (canGive)
            {
                _itemsInBag.Remove(slot);
                InventoryHandlerData.S_GiveItem(OwnerId, item, slot.Amount);
            }
        }
        
        #region IInteraction
        public float InteractionTime => _itemsInBag.Count == 0 ? 0 : InteractionDelay;
        public void Interact(int clientId, bool asServer)
        {
            if (asServer || _itemsInBag.Count == 0) return;
            InventoryHandlerData.C_DisplayBag(ObjectId, _itemsInBag.GetCollection(false));
        }
        #endregion
    }
}