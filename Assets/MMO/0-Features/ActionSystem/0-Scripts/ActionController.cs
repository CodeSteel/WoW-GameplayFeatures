using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using MMO.AnimalSystem;
using SteelBox;
using UnityEngine;

namespace MMO.ActionSystem
{
    public class ActionController : BaseNetworkBehaviour
    {
        public List<ActionSo> StartingActions = new List<ActionSo>();
        
        private readonly SyncList<ActionObject> _actions = new SyncList<ActionObject>();
        private int _targetNobId;
        private float _lastActionUseTime;
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                ActionHandlerData.S_OnGiveAction += S_OnGiveAction;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                ActionHandlerData.S_OnGiveAction -= S_OnGiveAction;
            }
            
            if (IsClientInitialized)
            {
                ActionHandlerData.C_OnSetTarget -= C_OnSetTarget;
                AnimalHandlerData.C_OnAnimalSlayed -= C_OnAnimalSlayed;
                if (IsOwner)
                    _actions.OnChange -= OnActionsChanged;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            ActionHandlerData.C_OnSetTarget += C_OnSetTarget;
            AnimalHandlerData.C_OnAnimalSlayed += C_OnAnimalSlayed;
            if (IsOwner)
                _actions.OnChange += OnActionsChanged;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            foreach (ActionSo actionSo in StartingActions)
                _actions.Add(new ActionObject(actionSo));
        }

        [Server]
        private void S_OnGiveAction(int clientId, ActionSo actionSo)
        {
            if (clientId != OwnerId) return;
            _actions.Add(new ActionObject(actionSo));
        }

        [Client]
        private void C_OnSetTarget(int nobId)
        {
            _targetNobId = nobId;
        }

        [Client]
        private void C_OnAnimalSlayed(int nobId, string name)
        {
            if (nobId == _targetNobId)
                ActionHandlerData.C_SetTarget(-1);
        }
        
        private void Update()
        {
            if ((IsServerInitialized && !IsClientInitialized) || !IsOwner) return;

            if (Input.GetKeyDown(KeyCode.Escape))
                ActionHandlerData.C_SetTarget(-1);
            
            for (int i = 49; i < 58; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    C_UseAction(i - 49);
                }
            }
        }
        
        [Client]
        private void C_UseAction(int actionIndex)
        {
            if (_actions.Count <= actionIndex) return;

            // delay
            if (Time.time - _lastActionUseTime < 0.5f) return;
            if (Time.time - _actions[actionIndex].LastUseTime < _actions[actionIndex].GetActionSo().Cooldown) return;
            
            // distance
            if (_targetNobId != -1 && _targetNobId.TryGetNetworkObjectFromObjectId(out NetworkObject targetNob) &&
                (Vector3.Distance(transform.position, targetNob.transform.position) >
                 _actions[actionIndex].GetActionSo().MinimumDistance)) return;
            
            // target self
            if (_targetNobId == -1 && !_actions[actionIndex].GetActionSo().CanTargetSelf) return;
            
            _actions[actionIndex].LastUseTime = _lastActionUseTime = Time.time;
            ActionHandlerData.C_UseAction(_actions[actionIndex].GetActionSo(), actionIndex);
            S_RpcUseAction(actionIndex, _targetNobId == -1 ? ObjectId : _targetNobId);
        }

        [ServerRpc]
        private void S_RpcUseAction(int actionIndex, int targetNobId, NetworkConnection conn = null)
        {
            if (_actions.Count <= actionIndex) return;
            _actions[actionIndex].LastUseTime = _lastActionUseTime = Time.time;
            ActionHandlerData.S_UseAction(conn.ClientId, _actions[actionIndex].GetActionSo());
            S_PerformAction(_actions[actionIndex].GetActionSo(), targetNobId);
        }

        [Server]
        private void S_PerformAction(ActionSo actionSo, int targetNobId)
        {
            if (actionSo.DamageTargetAmount > 0)
                AnimalHandlerData.S_Attack(targetNobId, OwnerId, actionSo.DamageTargetAmount);
            else if (actionSo.HealTargetAmount > 0)
                AnimalHandlerData.S_ModifyHealth(targetNobId, actionSo.HealTargetAmount);
        }
        
        private void OnActionsChanged(SyncListOperation op, int index, ActionObject oldItem, ActionObject newItem, bool asServer)
        {
            List<ActionSo> actionSos = new List<ActionSo>();
            foreach (ActionObject action in _actions)
                actionSos.Add(action.GetActionSo());
            ActionHandlerData.C_ActionsChanged(actionSos);
        }
    }
}