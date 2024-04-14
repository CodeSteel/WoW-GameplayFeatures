using System;
using SteelBox;

namespace MMO.Quest
{
    public class QuestHandlerData
    {
        public static event Action<int, string> S_OnStartQuest;
        public static void S_StartQuest(int clientId, string questId) => S_OnStartQuest?.Invoke(clientId, questId);
        
        public static event Action<int, string> S_OnCompleteQuest;
        public static void S_CompleteQuest(int clientId, string questId) => S_OnCompleteQuest?.Invoke(clientId, questId);
        
        public static event Action<int, string> S_OnAdvanceQuest;
        public static void S_AdvanceQuest(int clientId, string questId) => S_OnAdvanceQuest?.Invoke(clientId, questId);
        
        public static event Action<int, Quest> S_OnQuestStateChange;
        public static void S_QuestStateChange(int clientId, Quest quest) => S_OnQuestStateChange?.Invoke(clientId, quest);
        
        public static event Action<int, string, QuestPoint> S_OnPlayerInteractedQuestPointStep;
        public static void S_PlayerInteractedQuestPointStep(int clientId, string questId, QuestPoint questPoint) => S_OnPlayerInteractedQuestPointStep?.Invoke(clientId, questId, questPoint);
        
        public static event Action<int, string, string, string, string> S_OnStartInteractionStepForQuestPoint;
        public static void S_StartInteractionStepForQuestPoint(int clientId, string questId, string questPointName, string title, string description) => S_OnStartInteractionStepForQuestPoint?.Invoke(clientId, questId, questPointName, title, description);
        
        public static event ActionValValRef<int, string, Quest> OnGetPlayerQuest;
        public static void GetPlayerQuest(int clientId, string questId, ref Quest quest) => OnGetPlayerQuest?.Invoke(clientId, questId, ref quest);
        
        public static event Action<Quest> C_OnQuestStateChange;
        public static void C_QuestStateChange(Quest quest) => C_OnQuestStateChange?.Invoke(quest);
    }
}
