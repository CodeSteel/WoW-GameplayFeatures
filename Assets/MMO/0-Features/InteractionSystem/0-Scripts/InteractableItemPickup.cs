using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO.InteractionSystem
{
    public class InteractableItemPickup : NetworkBehaviour, IInteraction
    {
        public SerializedDictionary<string, int> ItemsToPickup = new SerializedDictionary<string, int>();
        public int InteractionDelay = 0;

        private bool _hasBeenLooted;

        #region IInteraction
        public float InteractionTime => _hasBeenLooted ? 0 : InteractionDelay;

        public void Interact(int clientId, bool asServer)
        {
            if (!asServer || _hasBeenLooted) return;
            foreach (KeyValuePair<string, int> kvp in ItemsToPickup)
                InventoryHandlerData.S_GiveItem(clientId, kvp.Key, kvp.Value);
            _hasBeenLooted = true;
        }
        #endregion
    }
}
