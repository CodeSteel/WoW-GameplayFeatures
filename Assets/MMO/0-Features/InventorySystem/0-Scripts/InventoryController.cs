using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using SteelBox;
using UnityEngine;

namespace MMO.InventorySystem
{
    public class InventoryController : BaseNetworkBehaviour
    {
        [SerializeField]
        private Inventory Inventory;
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                InventoryHandlerData.S_OnGiveItem += S_OnGiveItem;
                InventoryHandlerData.S_OnHasItem += S_OnHasItem;
                InventoryHandlerData.S_OnTakeItem += S_OnTakeItem;
            }
            InventoryHandlerData.OnCanGiveItem += OnCanGiveItem;
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                InventoryHandlerData.S_OnGiveItem -= S_OnGiveItem;
                InventoryHandlerData.S_OnHasItem -= S_OnHasItem;
                InventoryHandlerData.S_OnTakeItem -= S_OnTakeItem;
            }
            InventoryHandlerData.OnCanGiveItem -= OnCanGiveItem;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            int inventorySize = 0;
            PlayerHandlerData.S_GetInventoryLimit(OwnerId, ref inventorySize);
            Inventory = new Inventory(inventorySize);

            StartCoroutine(S_DelayedSetupInterview(inventorySize));
        }
        
        [Server]
        private IEnumerator S_DelayedSetupInterview(int size)
        {
            yield return new WaitForSeconds(0.1f);
            C_TRpcSetupInventory(Owner, size);
        }
        
        [TargetRpc]
        private void C_TRpcSetupInventory(NetworkConnection _, int size)
        {
            Inventory = new Inventory(size);
            InventoryHandlerData.C_InventoryChanged(Inventory.Slots);
        }
        
        [Server]
        private void S_OnGiveItem(int clientId, string itemId, int amount)
        {
            if (clientId != OwnerId) return;
            Inventory.GiveItem(itemId, amount);
            C_TRpcGiveItem(Owner, itemId, amount);
        }
        
        private void OnCanGiveItem(int clientId, string itemId, int amount, ref bool canGive)
        {
            if (clientId != OwnerId) return;
            canGive = Inventory.CanGiveItem(itemId, amount);
        }

        [Server]
        private void S_OnHasItem(int clientId, string itemId, int amount, ref bool hasItem)
        {
            if (clientId != OwnerId) return;
            hasItem = Inventory.HasItem(itemId, amount);
        }
        
        [Server]
        private void S_OnTakeItem(int clientId, string itemId, int amount)
        {
            if (clientId != OwnerId) return;
            Inventory.TakeItem(itemId, amount);
            C_TRpcTakeItem(Owner, itemId, amount);
        }

        [TargetRpc(ExcludeServer = true)]
        private void C_TRpcGiveItem(NetworkConnection _, string itemId, int amount)
        {
            Inventory.GiveItem(itemId, amount);
            InventoryHandlerData.C_InventoryChanged(Inventory.Slots);
        }
        
        [TargetRpc(ExcludeServer = true)]
        private void C_TRpcTakeItem(NetworkConnection _, string itemId, int amount)
        {
            Inventory.TakeItem(itemId, amount);
            InventoryHandlerData.C_InventoryChanged(Inventory.Slots);
        }
    }
}