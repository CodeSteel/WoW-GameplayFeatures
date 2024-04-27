using System.Collections.Generic;
using FishNet.Object;
using MMO.AnimalSystem;
using SteelBox;

namespace MMO.Quest
{
    public class QuestStep_SlayAnimals : QuestStep
    {
        public SerializedDictionary<string, int> AnimalsToSlay = new SerializedDictionary<string, int>();
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                AnimalHandlerData.S_OnAnimalSlayed += S_OnAnimalSlayed;
            }
        }

        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                AnimalHandlerData.S_OnAnimalSlayed -= S_OnAnimalSlayed;
            }
        }

        [Server]
        private void S_OnAnimalSlayed(int clientId, string animalName)
        {
            if (clientId != OwnerId) return;
            if (!AnimalsToSlay.ContainsKey(animalName)) return;
            AnimalsToSlay[animalName]--;
            if (AnimalsToSlay[animalName] <= 0)
                AnimalsToSlay.Remove(animalName);
            if (AnimalsToSlay.Count == 0) FinishStep();
        }
    }
}