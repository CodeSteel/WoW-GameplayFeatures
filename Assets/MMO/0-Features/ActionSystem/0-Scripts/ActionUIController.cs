using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO.ActionSystem
{
    public class ActionUIController : BaseMonoBehaviour
    {
        [SerializeField]
        private List<InventoryUIItem> _uiItems;
        [SerializeField]
        private GameObject _targetObjectNotifier;
        
        protected override void RegisterEvents()
        {
            ActionHandlerData.C_OnActionsChanged += C_OnActionsChanged;
            ActionHandlerData.C_OnUseAction += C_OnUseAction;
            ActionHandlerData.C_OnSetTarget += C_OnSetTarget;
        }

        protected override void UnregisterEvents()
        {
            ActionHandlerData.C_OnActionsChanged -= C_OnActionsChanged;
            ActionHandlerData.C_OnUseAction -= C_OnUseAction;
            ActionHandlerData.C_OnSetTarget -= C_OnSetTarget;
        }

        private void C_OnActionsChanged(List<ActionSo> actionSos)
        {
            for (int i = 0; i < _uiItems.Count; i++)
                _uiItems[i].Clear();
            for (int i = 0; i < actionSos.Count; i++)
            {
                if (i + 1 > _uiItems.Count) return;
                _uiItems[i].Setup(actionSos[i]);
            }
        }

        private void C_OnUseAction(ActionSo actionSo, int actionIndex)
        {
            for (int i = 0; i < _uiItems.Count; i++)
            {
                if (!_uiItems[i].IsHoldingItem) continue;
                if (i == actionIndex) continue;
                _uiItems[i].DelayAction(0.5f);
            }
            _uiItems[actionIndex].DelayAction(Mathf.Max(actionSo.Cooldown, 0.5f));
        }

        private void C_OnSetTarget(int targetNobId)
        {
            _targetObjectNotifier.SetActive(false);
            _targetObjectNotifier.transform.SetParent(null);
            if (targetNobId != -1 && targetNobId.TryGetNetworkObjectFromObjectId(out NetworkObject obj))
            {
                _targetObjectNotifier.transform.SetParent(obj.transform);
                _targetObjectNotifier.transform.localPosition = new Vector3(0, 2.3f, 0);
                _targetObjectNotifier.SetActive(true);
            }
        }
    }
}