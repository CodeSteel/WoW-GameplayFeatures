using MMO.ActionSystem;
using MMO.InventorySystem;
using SteelBox;

namespace MMO
{
    public static class ItemDatabaseHandlerData
    {
        public static event ActionValRef<string, InventoryItemSo> OnGetItemById;

        public static void GetItemById(string id, ref InventoryItemSo itemBase) =>
            OnGetItemById?.Invoke(id, ref itemBase);

        public static event ActionValRef<string, ActionSo> OnGetActionById;
        public static void GetActionById(string id, ref ActionSo actionSo) => OnGetActionById?.Invoke(id, ref actionSo);
    }
}