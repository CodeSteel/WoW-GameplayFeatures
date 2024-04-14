using FishNet.Object;

namespace SteelBox
{
    public abstract class BaseNetworkBehaviour : NetworkBehaviour
    {
        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            RegisterEvents();
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            UnregisterEvents();
        }

        protected abstract void RegisterEvents();
        protected abstract void UnregisterEvents();
    }
}