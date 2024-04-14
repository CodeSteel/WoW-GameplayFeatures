using System.Collections.Generic;
using SteelBox;
using UnityEngine;

namespace MMO.InventorySystem
{
    public class InventoryUIController : BaseMonoBehaviour
    {
        [SerializeField]
        private GameObject _inventoryContainerObject;
        [SerializeField]
        private List<InventoryUIItem> _uiItems;

        protected override void RegisterEvents()
        {
            InventoryHandlerData.C_OnInventoryChanged += C_OnInventoryChanged;
        }

        protected override void UnregisterEvents()
        {
            InventoryHandlerData.C_OnInventoryChanged -= C_OnInventoryChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                _inventoryContainerObject.SetActive(!_inventoryContainerObject.activeSelf);
            if (Input.GetKeyDown(KeyCode.Escape))
                _inventoryContainerObject.SetActive(false);
        }

        private void C_OnInventoryChanged(InventoryItemSlot[] slots)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                    _uiItems[i].Clear();
                else
                    _uiItems[i].Setup(slots[i].ItemId, slots[i].Amount);
            }
        }
    }
}