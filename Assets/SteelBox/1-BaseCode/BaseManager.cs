namespace SteelBox
{
    public abstract class BaseManager : BaseMonoBehaviour
    {
        protected override void OnAwake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this);
        }
    }
}
