using System.Collections.Generic;
using System.Linq;
using MMO.ActionSystem;
using MMO.InventorySystem;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public class ItemDatabaseManager : BaseMonoBehaviour
    {
        private Dictionary<string, InventoryItemSo> _inventoryItemSos = new Dictionary<string, InventoryItemSo>();
        private Dictionary<string, ActionSo> _actionSos = new Dictionary<string, ActionSo>();

        protected override void RegisterEvents()
        {
            ItemDatabaseHandlerData.OnGetItemById += OnGetItemById;
            ItemDatabaseHandlerData.OnGetActionById += OnGetActionById;
        }

        protected override void UnregisterEvents()
        {
            ItemDatabaseHandlerData.OnGetItemById -= OnGetItemById;
            ItemDatabaseHandlerData.OnGetActionById -= OnGetActionById;
        }

        private void Start()
        {
            InventoryItemSo[] slotSos = Resources.LoadAll<InventoryItemSo>("Items");
            foreach (InventoryItemSo slotSo in slotSos)
                _inventoryItemSos.Add(slotSo.Id, slotSo);

            ActionSo[] actionSos = Resources.LoadAll<ActionSo>("Actions");
            foreach (ActionSo actionSo in actionSos)
                _actionSos.Add(actionSo.Id, actionSo);
        }

        private void OnGetItemById(string id, ref InventoryItemSo itemBase)
        {
            _inventoryItemSos.TryGetValue(id, out itemBase);
        }

        private void OnGetActionById(string id, ref ActionSo actionSo)
        {
            _actionSos.TryGetValue(id, out actionSo);
        }
    }
}