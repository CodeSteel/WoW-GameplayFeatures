using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public class PlayerStatsController : BaseNetworkBehaviour
    {
        private int Sync_Level = 1;
        private int Sync_Gold = 0;
        private int Sync_Experience = 0;

        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                PlayerHandlerData.S_OnGiveExperience += S_OnGiveExperience;
                PlayerHandlerData.S_OnGiveGold += S_OnGiveGold;
                PlayerHandlerData.S_OnGetInventoryLimit += S_OnGetInventoryLimit;
            }

            PlayerHandlerData.OnGetLevel += OnGetLevel;
            PlayerHandlerData.OnGetGold += OnGetGold;
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                PlayerHandlerData.S_OnGiveExperience -= S_OnGiveExperience;
                PlayerHandlerData.S_OnGiveGold -= S_OnGiveGold;
                PlayerHandlerData.S_OnGetInventoryLimit -= S_OnGetInventoryLimit;
            }

            PlayerHandlerData.OnGetLevel -= OnGetLevel;
            PlayerHandlerData.OnGetGold -= OnGetGold;
        }

        public override void OnStartClient()
        {
            if (IsOwner)
            {
                PlayerHandlerData.LocalPlayerClientId = OwnerId;
                PlayerHandlerData.C_ExperienceChangedEvent(Sync_Experience, Sync_Level, GetRequiredExperienceForNextLevel());
            }
        }

        private int GetRequiredExperienceForNextLevel()
        {
            return Sync_Level * 100;
        }

        [Server]
        private void S_OnGiveExperience(int clientId, int experience)
        {
            if (clientId != OwnerId) return;
            int experienceRequiredForNextLevel = GetRequiredExperienceForNextLevel();
            if (Sync_Experience + experience >= experienceRequiredForNextLevel)
            {
                Sync_Experience = 0;
                Sync_Level++;
                PlayerHandlerData.S_LevelChangedEvent(OwnerId, Sync_Level);
            }
            else
            {
                Sync_Experience += experience;
            }
            C_TRpcUpdateLevel(Owner, Sync_Level, Sync_Experience);
        }

        [TargetRpc]
        private void C_TRpcUpdateLevel(NetworkConnection _, int level, int experience)
        {
            Sync_Level = level;
            Sync_Experience = experience;
            PlayerHandlerData.C_ExperienceChangedEvent(experience, level, GetRequiredExperienceForNextLevel());
        }
        
        [Server]
        private void S_OnGiveGold(int clientId, int gold)
        {
            if (clientId != OwnerId) return;
            Sync_Gold += gold;
            C_TRpcUpdateGold(Owner, gold);
        }
        
        [TargetRpc]
        private void C_TRpcUpdateGold(NetworkConnection _, int gold)
        {
            Sync_Gold = gold;
        }

        [Server]
        private void S_OnGetInventoryLimit(int clientId, ref int inventoryLimit)
        {
            if (clientId != OwnerId) return;
            inventoryLimit = 16;
        }

        private void OnGetLevel(int clientId, ref int level)
        {
            if (clientId != OwnerId) return;
            level = Sync_Level;
        }

        private void OnGetGold(int clientId, ref int gold)
        {
            if (clientId != OwnerId) return;
            gold = Sync_Gold;
        }
    }
}