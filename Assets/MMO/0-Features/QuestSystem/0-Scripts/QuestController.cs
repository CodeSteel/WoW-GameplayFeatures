using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using MMO.ActionSystem;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO.Quest
{
    public class QuestController : BaseNetworkBehaviour
    {
        [SerializeField]
        private SerializedDictionary<string, Quest> _quests = new SerializedDictionary<string, Quest>();

        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnStartQuest += S_OnStartQuest;
                QuestHandlerData.S_OnCompleteQuest += S_OnCompleteQuest;
                QuestHandlerData.S_OnAdvanceQuest += S_OnAdvanceQuest;
                
                PlayerHandlerData.S_OnLevelChanged += S_OnLevelChanged;
            }

            QuestHandlerData.OnGetPlayerQuest += OnGetPlayerQuest;
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnStartQuest -= S_OnStartQuest;
                QuestHandlerData.S_OnCompleteQuest -= S_OnCompleteQuest;
                QuestHandlerData.S_OnAdvanceQuest -= S_OnAdvanceQuest;
                
                PlayerHandlerData.S_OnLevelChanged -= S_OnLevelChanged;
            }
            
            QuestHandlerData.OnGetPlayerQuest -= OnGetPlayerQuest;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            QuestSo[] questSos = Resources.LoadAll<QuestSo>("Quests");
            foreach (QuestSo questSo in questSos)
                _quests.Add(questSo.Id, new Quest(questSo));
            StartCoroutine(DelayedQuestCheck());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner && !IsServerInitialized)
            {
                QuestSo[] questSos = Resources.LoadAll<QuestSo>("Quests");
                foreach (QuestSo questSo in questSos)
                    _quests.Add(questSo.Id, new Quest(questSo));
                StartCoroutine(DelayedQuestCheck());
            }
            
            Destroy(this);
        }

        private IEnumerator DelayedQuestCheck()
        {
            yield return new WaitForSeconds(0.1f);
            CheckForAvailableQuests();
        }
        
        [Server]
        private void S_OnStartQuest(int clientId, string questId)
        {
            if (clientId != OwnerId) return;
            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} StartQuest : QuestId not found, questId={questId}");
                return;
            }
            if (quest.QuestState != QuestState.CanStart)
            {
                Debug.LogWarning($"{this} Tried starting quest that isn't ready to start, state={quest.QuestState}, id={questId}");
                return;
            }

            if (quest.QuestSo.QuestStepsObjects.Count > 0)
            {
                quest.QuestState = QuestState.InProgress;
                S_InstantiateStepObject(questId, 0);
            }
            else
                quest.QuestState = QuestState.CanFinish;
            
            QuestHandlerData.S_QuestStateChange(clientId, quest);
            C_TRpcSetQuestState(Owner, questId, quest.QuestState);
        }

        [Server]
        private void S_OnCompleteQuest(int clientId, string questId)
        {
            if (clientId != OwnerId) return;
        
            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} CompleteQuest : QuestId not found, questId={questId}");
                return;
            }

            if (quest.QuestState != QuestState.CanFinish)
            {
                Debug.LogWarning($"{this} Tried to complete quest that isn't set to CanFinish, questId={questId}, state={quest.QuestState}");
                return;
            }
            
            bool canGiveAllItems = true;
            foreach (KeyValuePair<InventoryItemSo, int> kvp in quest.QuestSo.InventoryItemsReward)
            {
                bool canGive = false;
                InventoryHandlerData.CanGiveItem(clientId, kvp.Key.Id, kvp.Value, ref canGive);
                if (!canGive)
                {
                    canGiveAllItems = false;
                    break;
                }
            }
            if (!canGiveAllItems)
            {
                NoticeHandlerData.S_DisplayNotice(clientId, "Inventory full!", "Failed to add items to complete quest!");
                return;
            }
            
            quest.QuestState = QuestState.Finished;
            PlayerHandlerData.S_GiveExperience(clientId, quest.QuestSo.ExperienceReward);
            PlayerHandlerData.S_GiveGold(clientId, quest.QuestSo.GoldReward);
            foreach (KeyValuePair<InventoryItemSo, int> kvp in quest.QuestSo.InventoryItemsReward)
                InventoryHandlerData.S_GiveItem(clientId, kvp.Key.Id, kvp.Value);
            foreach (ActionSo action in quest.QuestSo.ActionsGivenReward)
                ActionHandlerData.S_GiveAction(clientId, action);
            QuestHandlerData.S_QuestStateChange(clientId, quest);
            C_TRpcSetQuestState(Owner, questId, quest.QuestState);
            CheckForAvailableQuests();
            
        }

        [Server]
        private void S_OnAdvanceQuest(int clientId, string questId)
        {
            if (clientId != OwnerId) return;

            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} InitiateStepObject : QuestId not found, questId={questId}");
                return;
            }

            if (quest.Step + 1 >= quest.QuestSo.QuestStepsObjects.Count)
            {
                quest.QuestState = QuestState.CanFinish;
                C_TRpcSetQuestState(Owner, questId, quest.QuestState);
            }
            else
            {
                quest.Step++;
                C_TRpcSetQuestStep(Owner, questId, quest.Step);
                S_InstantiateStepObject(questId, quest.Step);
            }
            QuestHandlerData.S_QuestStateChange(clientId, quest);
        }

        private void OnGetPlayerQuest(int clientId, string questId, ref Quest quest)
        {
            if (clientId != OwnerId) return;
            _quests.TryGetValue(questId, out quest);
        }
        
        #region Events
        private void S_OnLevelChanged(int clientId, int newLevel)
        {
            if (clientId != OwnerId) return;
            CheckForAvailableQuests();
        }
        #endregion
        
        #region Helpers
        private void CheckForAvailableQuests()
        {
            int level = 0;
            PlayerHandlerData.GetLevel(OwnerId, ref level);
            
            foreach (KeyValuePair<string, Quest> kvp in _quests)
            {
                if (kvp.Value.QuestState == QuestState.RequirementsNotMet)
                {
                    // check if we meet requirements
                    if (level < kvp.Value.QuestSo.LevelRequirement) continue;
                    bool hasAllCompleted = true;
                    for (int i = 0; i < kvp.Value.QuestSo.CompletedQuestsRequirement.Count; i++)
                    {
                        if (!_quests.TryGetValue(kvp.Value.QuestSo.CompletedQuestsRequirement[i].Id, out Quest quest) || quest.QuestState != QuestState.Finished)
                            hasAllCompleted = false;
                    }
                    if (!hasAllCompleted) continue;

                    kvp.Value.QuestState = QuestState.CanStart;
                    
                    if (IsServerInitialized)
                        QuestHandlerData.S_QuestStateChange(OwnerId, kvp.Value);
                }
            }
        }
        
        [Server]
        private void S_InstantiateStepObject(string questId, int step)
        {
            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} InitiateStepObject : QuestId not found, questId={questId}");
                return;
            }

            if (step >= quest.QuestSo.QuestStepsObjects.Count)
            {
                Debug.LogWarning($"{this} Tried initiating step object that doesn't exist! stepCount={step}, questId={questId}");
                return;
            }
            
            // create next step
            QuestStep stepNetworkPrefab = quest.QuestSo.QuestStepsObjects[step];
            if (stepNetworkPrefab == null)
            {
                Debug.LogWarning($"{this} Tried instantiating step objects with null object! stepCount={step}, questId={questId}");
                return;
            }
            QuestStep stepNetworkObject = Instantiate(stepNetworkPrefab);
            stepNetworkObject.QuestId = questId;
            Spawn(stepNetworkObject.NetworkObject, Owner);
        }

        [TargetRpc]
        private void C_TRpcSetQuestState(NetworkConnection _, string questId, QuestState state)
        {
            Debug.Log($"C_SetQuest > id={questId}, state={state}");
            
            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} InitiateStepObject : QuestId not found, questId={questId}");
                return;
            }

            quest.QuestState = state;
            QuestHandlerData.C_QuestStateChange(quest);
        }
        
        [TargetRpc]
        private void C_TRpcSetQuestStep(NetworkConnection _, string questId, int step)
        {
            Debug.Log($"C_SetQuest > id={questId}, step={step}");
            
            if (!_quests.TryGetValue(questId, out Quest quest))
            {
                Debug.LogWarning($"{this} InitiateStepObject : QuestId not found, questId={questId}");
                return;
            }

            quest.Step = step;
        }
        #endregion
    }
}