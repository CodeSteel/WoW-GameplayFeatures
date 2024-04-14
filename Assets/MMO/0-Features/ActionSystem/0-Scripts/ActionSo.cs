using UnityEditor;
using UnityEngine;

namespace MMO.ActionSystem
{
    [CreateAssetMenu(menuName = "MMO/ActionSo")]
    public class ActionSo : ScriptableObject
    {
        public string Id;
        public Sprite Icon;
        
        public float Cooldown;
        
        public float MinimumDistance;
        public bool CanTargetSelf;
        
        public int HealTargetAmount;
        public int DamageTargetAmount;
        
        private void OnValidate()
        {
#if UNITY_EDITOR
            Id = this.name;
            EditorUtility.SetDirty(this);
#endif
        }
    }
}