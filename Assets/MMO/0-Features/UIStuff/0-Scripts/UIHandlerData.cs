using SteelBox;

namespace MMO
{
    public class UIHandlerData
    {
        public static event ActionRef<bool> OnGetIsMouseOverUI;
        public static void GetIsMouseOverUI(ref bool isMouseOverUI) => OnGetIsMouseOverUI?.Invoke(ref isMouseOverUI);
    }
}