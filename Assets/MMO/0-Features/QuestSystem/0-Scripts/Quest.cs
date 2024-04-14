using System;

namespace MMO.Quest
{
    public enum QuestState
    {
        RequirementsNotMet,
        CanStart,
        InProgress,
        CanFinish,
        Finished
    }

    [Serializable]
    public class Quest
    {
        public QuestSo QuestSo;
        public QuestState QuestState;

        public int Step;
        
        public Quest(QuestSo questSo)
        {
            QuestSo = questSo;
            QuestState = QuestState.RequirementsNotMet;
        }
    }
}
