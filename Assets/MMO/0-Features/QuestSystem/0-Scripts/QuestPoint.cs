using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using MMO.ActionSystem;
using MMO.InteractionSystem;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO.Quest
{
    public class QuestPoint : BaseNetworkBehaviour, IInteraction
    {
        private enum QuestPointType
        {
            Notify,
            Question
        }
        
        [SerializeField] private GameObject _notifyMarkObject;
        [SerializeField] private GameObject _questionMarkObject;

        [SerializeField]
        private SerializedDictionary<string, QuestPointType> _queuedInteractions = new SerializedDictionary<string, QuestPointType>();
        private Dictionary<string, (string, string)> _queuedInteractionsText = new Dictionary<string, (string, string)>();
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnQuestStateChange += S_OnQuestStateChange;
                QuestHandlerData.S_OnStartInteractionStepForQuestPoint += S_OnStartInteractionStepForQuestPoint;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                QuestHandlerData.S_OnQuestStateChange -= S_OnQuestStateChange;
                QuestHandlerData.S_OnStartInteractionStepForQuestPoint -= S_OnStartInteractionStepForQuestPoint;
            }
        }

        [Server]
        private void S_OnQuestStateChange(int clientId, Quest quest)
        {
            if (!ServerManager.Clients.TryGetValue(clientId, out NetworkConnection conn)) return;
            if ((quest.QuestState == QuestState.CanFinish && quest.QuestSo.QuestCompleterName == gameObject.name) || (quest.QuestState == QuestState.CanStart && quest.QuestSo.QuestGiverName == gameObject.name))
                C_TRpcSetupQuestionPointForQuest(conn, quest.QuestSo.Id);
            else if ((quest.QuestState == QuestState.Finished && quest.QuestSo.QuestCompleterName == gameObject.name))
                C_TRpcRemovePointForQuest(conn, quest.QuestSo.Id);
        }
        
        [TargetRpc]
        private void C_TRpcSetupQuestionPointForQuest(NetworkConnection _, string questId)
        {
            _queuedInteractions.TryAdd(questId, QuestPointType.Question);
            C_UpdatePointState();
        }

        [Server]
        private void S_OnStartInteractionStepForQuestPoint(int clientId, string questId, string questPointName, string title, string description)
        {
            if (questPointName != gameObject.name) return;
            if (!ServerManager.Clients.TryGetValue(clientId, out NetworkConnection conn)) return;
            C_TRpcSetupNotifyPointForQuest(conn, questId, title, description);
        }

        [TargetRpc]
        private void C_TRpcSetupNotifyPointForQuest(NetworkConnection _, string questId, string title, string description)
        {
            _queuedInteractions.TryAdd(questId, QuestPointType.Notify);
            _queuedInteractionsText.TryAdd(questId, (title, description));
            C_UpdatePointState();
        }
        
        [TargetRpc]
        private void C_TRpcRemovePointForQuest(NetworkConnection _, string questId)
        {
            _queuedInteractions.Remove(questId);
            C_UpdatePointState();
        }

        [Client]
        private void C_UpdatePointState()
        {
            if (_queuedInteractions.Count == 0)
            {
                _notifyMarkObject.SetActive(false);
                _questionMarkObject.SetActive(false);
                return;
            }
            
            // prioritize completing quests
            bool useQuestion = false;
            foreach (KeyValuePair<string, QuestPointType> kvp in _queuedInteractions)
            {
                if (kvp.Value == QuestPointType.Question)
                    useQuestion = true;
            }
            
            _notifyMarkObject.SetActive(!useQuestion);
            _questionMarkObject.SetActive(useQuestion);
        }

        #region IInteraction
        public float InteractionTime => 0;
        public void Interact(int clientId, bool asServer)
        {
            if (asServer) return;
            
            ActionHandlerData.C_SetTarget(-1);
            
            // question interactions first

            foreach (KeyValuePair<string, QuestPointType> kvp in _queuedInteractions)
            {
                if (kvp.Value == QuestPointType.Question)
                {
                    Quest quest = null;
                    QuestHandlerData.GetPlayerQuest(clientId, kvp.Key, ref quest);
                    if (quest == null) continue;
                    
                    if (quest.QuestState == QuestState.CanFinish)
                        NoticeHandlerData.C_DisplayCompleteNotice(quest.QuestSo.Title, quest.QuestSo.Description, GetRewardsFromQuest(quest.QuestSo), () =>
                        {
                            S_RpcCompletePlayerQuest(kvp.Key);
                        });
                    else
                        NoticeHandlerData.C_DisplayNotice(quest.QuestSo.Title, quest.QuestSo.Description, GetRewardsFromQuest(quest.QuestSo), () =>
                        {
                            _queuedInteractions.Remove(kvp.Key);
                            C_UpdatePointState();
                            S_RpcStartPlayerQuest(kvp.Key);
                        });
                    return;
                }
            }
            
            //  no question interactions, go ahead with notifies
            
            foreach (KeyValuePair<string, QuestPointType> kvp in _queuedInteractions)
            {
                Quest quest = null;
                QuestHandlerData.GetPlayerQuest(clientId, kvp.Key, ref quest);
                if (quest == null) continue;

                if (!_queuedInteractionsText.TryGetValue(kvp.Key, out (string, string) questText))
                {
                    Debug.LogWarning($"{this} Tried using queued interactions text that doesn't exist! questId={kvp.Key}");
                    return;
                }
                NoticeHandlerData.C_DisplayNotice(questText.Item1, questText.Item2, "", () =>
                {
                    S_RpcPlayerInteractedStep(kvp.Key);
                    _queuedInteractions.Remove(kvp.Key);
                    _queuedInteractionsText.Remove(kvp.Key);
                    C_UpdatePointState();
                });
                
                return;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void S_RpcStartPlayerQuest(string questId, NetworkConnection conn = null)
        {
            QuestHandlerData.S_StartQuest(conn.ClientId, questId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void S_RpcCompletePlayerQuest(string questId, NetworkConnection conn = null)
        {
            QuestHandlerData.S_CompleteQuest(conn.ClientId, questId);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void S_RpcPlayerInteractedStep(string questId, NetworkConnection conn = null)
        {
            QuestHandlerData.S_PlayerInteractedQuestPointStep(conn.ClientId, questId, this);
        }

        private string GetRewardsFromQuest(QuestSo questSo)
        {
            string str = "Rewards:  ";
            str += $"Exp: {questSo.ExperienceReward}, Gold: {questSo.GoldReward}";
            if (questSo.ActionsGivenReward.Count > 0)
            {
                str += ", Actions: ";
                for (int i = 0; i < questSo.ActionsGivenReward.Count; i++)
                {
                    str += questSo.ActionsGivenReward[i].Id;
                    if (questSo.ActionsGivenReward.Count - 1 > i)
                        str += ", ";
                }
            }
            
            if (questSo.InventoryItemsReward.Count > 0)
            {
                str += ", Items: ";
                foreach (KeyValuePair<InventoryItemSo, int> item in questSo.InventoryItemsReward)
                    str += $"{item.Key.Id} ({item.Value})";
            }

            return str;
        }
        #endregion
    }
}