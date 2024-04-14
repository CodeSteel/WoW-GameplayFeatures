using SteelBox;

namespace MMO.Quest
{
    public abstract class QuestStep : BaseNetworkBehaviour
    {
        public string QuestId;
        
        protected void FinishStep()
        {
            QuestHandlerData.S_AdvanceQuest(OwnerId, QuestId);
            Despawn(NetworkObject);
        }
    }
}