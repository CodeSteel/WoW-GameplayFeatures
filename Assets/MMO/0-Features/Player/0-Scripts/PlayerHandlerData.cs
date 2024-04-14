using System;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public static class PlayerHandlerData
    {
        public static event ActionRef<Vector3> C_OnGetLocalPlayerPosition;

        public static void C_GetLocalPlayerPosition(ref Vector3 position) =>
            C_OnGetLocalPlayerPosition?.Invoke(ref position);

        public static event ActionValRef<int, int> OnGetLevel;
        public static void GetLevel(int clientId, ref int level) => OnGetLevel?.Invoke(clientId, ref level);

        public static event ActionValRef<int, int> OnGetGold;
        public static void GetGold(int clientId, ref int gold) => OnGetGold?.Invoke(clientId, ref gold);

        public static event Action<int, int> S_OnGiveExperience;

        public static void S_GiveExperience(int clientId, int experience) =>
            S_OnGiveExperience?.Invoke(clientId, experience);

        public static event Action<int, int> S_OnGiveGold;
        public static void S_GiveGold(int clientId, int gold) => S_OnGiveGold?.Invoke(clientId, gold);

        public static event Action<int, int> S_OnLevelChanged;

        public static void S_LevelChangedEvent(int clientId, int newLevel) =>
            S_OnLevelChanged?.Invoke(clientId, newLevel);

        public static event ActionValRef<int, int> S_OnGetInventoryLimit;

        public static void S_GetInventoryLimit(int clientId, ref int inventoryLimit) =>
            S_OnGetInventoryLimit?.Invoke(clientId, ref inventoryLimit);

        public static event Action C_OnPlayerMoveEvent;
        public static void C_PlayerMoveEvent() => C_OnPlayerMoveEvent?.Invoke();
        
        public static event Action<int, int, int> C_OnExperienceChangedEvent;
        public static void C_ExperienceChangedEvent(int newExperience, int newLevel, int expNeededNextLevel) => C_OnExperienceChangedEvent?.Invoke(newExperience, newLevel, expNeededNextLevel);

        public static event Action<int> S_OnPlayerMoveEvent;
        public static void S_PlayerMoveEvent(int clientId) => S_OnPlayerMoveEvent?.Invoke(clientId);

        public static int LocalPlayerClientId;
    }
}