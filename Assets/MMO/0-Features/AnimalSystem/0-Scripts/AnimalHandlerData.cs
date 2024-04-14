using System;

namespace MMO.AnimalSystem
{
    public static class AnimalHandlerData
    {
        public static event Action<int, int> S_OnModifyHealth;
        public static void S_ModifyHealth(int nobId, int amount) => S_OnModifyHealth?.Invoke(nobId, amount);
        
        public static event Action<int, int, int> S_OnAttack;
        public static void S_Attack(int nobId, int byClientId, int amount) => S_OnAttack?.Invoke(nobId, byClientId, amount);
        
        public static event Action<int, string> S_OnAnimalSlayed;
        public static void S_AnimalSlayed(int clientId, string animalName) => S_OnAnimalSlayed?.Invoke(clientId, animalName);
        
        public static event Action<int, string> C_OnAnimalSlayed;
        public static void C_AnimalSlayed(int objectId, string animalName) => C_OnAnimalSlayed?.Invoke(objectId, animalName);
    }
}