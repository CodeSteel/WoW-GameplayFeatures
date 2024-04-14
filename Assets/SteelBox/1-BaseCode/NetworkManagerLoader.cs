using System.Collections.Generic;
using UnityEngine;

namespace SteelBox
{
    public class NetworkManagerLoader : BaseNetworkBehaviour
    {
        public List<BaseNetworkManager> _NetworkManagers = new List<BaseNetworkManager>();

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        public override void OnStartServer()
        {
            foreach (BaseNetworkManager networkManager in _NetworkManagers)
            {
                BaseNetworkManager instancedManager = Instantiate(networkManager);
                Spawn(instancedManager.NetworkObject);
                Debug.Log($"ManagerLoader: Loaded {instancedManager.name}");
            }
        }
    }
}