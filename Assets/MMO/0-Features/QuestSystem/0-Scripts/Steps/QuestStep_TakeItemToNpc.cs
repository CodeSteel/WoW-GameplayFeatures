using System.Collections.Generic;
using FishNet.Object;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO.Quest
{
    public class QuestStep_TakeItemToNpc : QuestStep
    {
        public string TargetQuestPointName;
        public SerializedDictionary<string, int> ItemsToTake = new SerializedDictionary<string, int>();
        public string NotifyTitle;
        public string NotifyDescription;

        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnPlayerInteractedQuestPointStep += S_OnPlayerInteractedQuestPointStep;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnPlayerInteractedQuestPointStep -= S_OnPlayerInteractedQuestPointStep;
            }
        }

        public override void OnStartServer()
        {
            QuestHandlerData.S_StartInteractionStepForQuestPoint(OwnerId, QuestId, TargetQuestPointName, NotifyTitle, NotifyDescription);
        }

        [Server]
        private void S_OnPlayerInteractedQuestPointStep(int clientId, string questId, QuestPoint questPoint)
        {
            if (clientId != OwnerId) return;
            if (questPoint.gameObject.name != TargetQuestPointName) return;
            if (questId != QuestId) return;
            
            bool hasItem = false;
            foreach (KeyValuePair<string, int> kvp in ItemsToTake)
            {
                InventoryHandlerData.S_HasItem(clientId, kvp.Key, kvp.Value, ref hasItem);
                if (!hasItem)
                {
                    Debug.LogWarning($"PlayerInteractPointStep cant take item to npc, doesnt have the items, {kvp.Key}, {kvp.Value}");
                    return;
                }
            }
            
            foreach (KeyValuePair<string, int> kvp in ItemsToTake)
                InventoryHandlerData.S_TakeItem(clientId, kvp.Key, kvp.Value);
            FinishStep();
        }
    }
}