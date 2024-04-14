using System;
using System.Collections.Generic;

namespace MMO.ActionSystem
{
    public static class ActionHandlerData
    {
        public static event Action<int, ActionSo> S_OnGiveAction;
        public static void S_GiveAction(int clientId, ActionSo actionSo) => S_OnGiveAction?.Invoke(clientId, actionSo);
        
        public static event Action<List<ActionSo>> C_OnActionsChanged;
        public static void C_ActionsChanged(List<ActionSo> actionSos) => C_OnActionsChanged?.Invoke(actionSos);
        
        public static event Action<ActionSo, int> C_OnUseAction;
        public static void C_UseAction(ActionSo actionSo, int actionIndex) => C_OnUseAction?.Invoke(actionSo, actionIndex);
        
        public static event Action<int> C_OnSetTarget;
        public static void C_SetTarget(int nobId) => C_OnSetTarget?.Invoke(nobId);
        
        public static event Action<int, ActionSo> S_OnUseAction;
        public static void S_UseAction(int clientId, ActionSo actionSo) => S_OnUseAction?.Invoke(clientId, actionSo);
    }
}