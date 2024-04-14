namespace SteelBox
{
    public abstract class BaseNetworkManager : BaseNetworkBehaviour
    {
        protected override void OnAwake()
        {
            transform.SetParent(null);
        }
    }
}