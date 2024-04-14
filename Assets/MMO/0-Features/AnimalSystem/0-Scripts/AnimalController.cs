using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using MMO.ActionSystem;
using MMO.InteractionSystem;
using SteelBox;
using UnityEngine;

namespace MMO.AnimalSystem
{
    public class AnimalController : BaseNetworkBehaviour, IInteraction
    {
        public int MaxHealth = 100;
        
        public readonly SyncVar<int> Sync_HealthValue = new SyncVar<int>();

        private bool _notifiedDeath;
        private int _lastAttackedBy = -1;

        public Action OnHealthChangeEvent;
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                AnimalHandlerData.S_OnModifyHealth += S_OnModifyHealth;
                AnimalHandlerData.S_OnAttack += S_OnAttack;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                AnimalHandlerData.S_OnModifyHealth -= S_OnModifyHealth;
                AnimalHandlerData.S_OnAttack -= S_OnAttack;
            }
            
            if (IsClientInitialized)
            {
                Sync_HealthValue.OnChange -= OnHealthChange;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Sync_HealthValue.Value = MaxHealth;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Sync_HealthValue.OnChange += OnHealthChange;
        }

        [Server]
        private void S_OnModifyHealth(int nobId, int amount)
        {
            if (nobId != ObjectId) return;
            if (_notifiedDeath) return;
            
            Sync_HealthValue.Value += amount;
            if (Sync_HealthValue.Value > MaxHealth)
                Sync_HealthValue.Value = MaxHealth;
            if (Sync_HealthValue.Value <= 0)
            {
                Sync_HealthValue.Value = 0;
                if (!_notifiedDeath)
                {
                    _notifiedDeath = true;
                    AnimalHandlerData.S_AnimalSlayed(_lastAttackedBy, gameObject.name);
                    PlayerHandlerData.S_GiveExperience(_lastAttackedBy, (int)(MaxHealth * 0.3f));
                }
            }
        }

        [Server]
        private void S_OnAttack(int nobId, int byClientId, int amount)
        {
            if (nobId != ObjectId) return;
            if (_notifiedDeath) return;
            _lastAttackedBy = byClientId;
            S_OnModifyHealth(nobId, -amount);
        }

        private void OnHealthChange(int prev, int next, bool asServer)
        {
            if (next <= 0)
            {
                AnimalHandlerData.C_AnimalSlayed(ObjectId, gameObject.name);
            }
            OnHealthChangeEvent?.Invoke();
        }
        
        #region IInteraction
        public float InteractionTime => 0;
        public void Interact(int clientId, bool asServer)
        {
            if (asServer || IsOwner) return;
            if (Sync_HealthValue.Value <= 0) return;
            ActionHandlerData.C_SetTarget(ObjectId);
        }
        #endregion
    }
}