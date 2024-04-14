using FishNet.Object;

namespace MMO.Quest
{
    public class QuestStep_TalkToNpc : QuestStep
    {
        public string TargetQuestPointName;
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
            FinishStep();
        }
    }
}