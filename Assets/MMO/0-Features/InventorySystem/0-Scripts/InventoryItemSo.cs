using UnityEditor;
using UnityEngine;

namespace MMO.InventorySystem
{
    [CreateAssetMenu(menuName = "MMO/Inventory Item")]
    public class InventoryItemSo : ScriptableObject
    {
        public string Id;

        public string Name;
        public Sprite Icon;
        public int StackLimit = 1;
        
        private void OnValidate()
        {
            #if UNITY_EDITOR
            Id = this.name;
            EditorUtility.SetDirty(this);
            #endif
        }
    }
}