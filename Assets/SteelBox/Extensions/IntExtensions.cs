using FishNet;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;

namespace SteelBox
{
    public static partial class IntExtensions
    {
        public static bool TryGetNetworkObjectFromObjectId(this int objectId, out NetworkObject networkObject)
        {
            networkObject = null; 
            if (InstanceFinder.IsServer)
            {
                ServerManager serverManager = InstanceFinder.ServerManager;
                if (serverManager == null || !serverManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = serverManager.Objects.Spawned[objectId];
                return true;
            }
            else if (InstanceFinder.IsClient)
            {
                ClientManager clientManager = InstanceFinder.ClientManager;
                if (clientManager == null || !clientManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = clientManager.Objects.Spawned[objectId];
                return true;
            }

            return false;
        }
    }
}