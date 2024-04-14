using System;
using System.Collections.Generic;
using MMO.ActionSystem;
using MMO.InventorySystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MMO.Quest
{
    [Serializable]
    [CreateAssetMenu(menuName = "MMO/QuestSO")]
    public class QuestSo : ScriptableObject
    {
        public string Id;
        
        public string Title;
        public string Description;
        
        public int LevelRequirement;
        public List<QuestSo> CompletedQuestsRequirement;
        
        public int ExperienceReward;
        public int GoldReward;
        public SerializedDictionary<InventoryItemSo, int> InventoryItemsReward = new SerializedDictionary<InventoryItemSo, int>();
        public List<ActionSo> ActionsGivenReward = new List<ActionSo>();

        public List<QuestStep> QuestStepsObjects = new List<QuestStep>();

        public string QuestGiverName;
        public string QuestCompleterName;
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            Id = this.name;
            EditorUtility.SetDirty(this);
#endif
        }
    }
}