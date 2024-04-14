using System.Collections.Generic;
using SteelBox;
using UnityEngine;
using UnityEngine.UI;

namespace MMO.InventorySystem
{
    public class InventoryBagUIController : BaseMonoBehaviour
    {
        [SerializeField] private InventoryUIItem _inventoryUIItemPrefab;
        [SerializeField] private Image _bagMainContainer;
        
        protected override void RegisterEvents()
        {
            InventoryHandlerData.C_OnDisplayBag += C_OnDisplayBag;
            PlayerHandlerData.C_OnPlayerMoveEvent += C_OnPlayerMoveEvent;
            CameraMovementHandlerData.OnClickGameScreen += C_OnClickGameScreen;
        }

        protected override void UnregisterEvents()
        {
            InventoryHandlerData.C_OnDisplayBag -= C_OnDisplayBag;
            PlayerHandlerData.C_OnPlayerMoveEvent -= C_OnPlayerMoveEvent;
            CameraMovementHandlerData.OnClickGameScreen -= C_OnClickGameScreen;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                C_HideDisplayBag();
        }
        
        private void C_OnDisplayBag(int bagNobId, List<InventoryItemSlot> items)
        {
            C_HideDisplayBag();
            _bagMainContainer.gameObject.SetActive(true);

            float padding = items.Count * 70;
            if (items.Count > 1) padding -= 3.5f * items.Count;
            _bagMainContainer.rectTransform.sizeDelta = new Vector2(_bagMainContainer.rectTransform.sizeDelta.x, padding);

            foreach (InventoryItemSlot slot in items)
            {
                InventoryUIItem uiItem = Instantiate(_inventoryUIItemPrefab, _bagMainContainer.transform);
                uiItem.Setup(bagNobId, slot.ItemId, slot.Amount);
            }
        }

        private void C_OnPlayerMoveEvent()
        {
            if (_bagMainContainer.isActiveAndEnabled)
                C_HideDisplayBag();
        }

        private void C_OnClickGameScreen()
        {
            C_HideDisplayBag();
        }
        
        private void C_HideDisplayBag()
        {
            _bagMainContainer.gameObject.SetActive(false);
            for (int i=0; i<_bagMainContainer.transform.childCount; i++)
                Destroy(_bagMainContainer.transform.GetChild(i).gameObject);
        }
    }
}