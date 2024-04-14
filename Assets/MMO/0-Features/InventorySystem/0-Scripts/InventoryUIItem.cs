using System.Collections;
using MMO.ActionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMO.InventorySystem
{
    public class InventoryUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemAmount;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Button _button;
        [SerializeField] private Image _delayImage;

        private int _bagNobId = -1;
        private int _holdingAmount;
        private string _holdingItemId;
        private Coroutine _delayCoroutine;
        private float _lastDelay;
        private float _lastDelayTime;

        public bool IsHoldingItem => _iconImage.gameObject.activeSelf;
        
        public void Setup(int bagNobId, string itemId, int amount)
        {
            _bagNobId = bagNobId;
            _holdingItemId = itemId;
            _holdingAmount = amount;

            Setup(itemId, amount);
        }
        
        public void Setup(ActionSo actionSo)
        {
            _iconImage.gameObject.SetActive(true);
            _iconImage.sprite = actionSo.Icon;
        }
        
        public void Setup(string itemId, int amount)
        {
            InventoryItemSo itemSo = null;
            ItemDatabaseHandlerData.GetItemById(itemId, ref itemSo);
            if (itemSo == null)
            {
                Destroy(gameObject);
                Debug.Log($"{this} Couldn't find item by id {itemId}");
                return;
            }

            _iconImage.gameObject.SetActive(true);
            _iconImage.sprite = itemSo.Icon;
            _itemName.gameObject.SetActive(true);
            _itemName.text = itemSo.Name;
            _itemAmount.gameObject.SetActive(true);
            _itemAmount.text = amount.ToString();
            _button.enabled = true;
            _button.onClick.AddListener(OnClickItem);
        }
        
        public void Clear()
        {
            _button.enabled = false;
            _itemAmount.gameObject.SetActive(false);
            _itemName.gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(false);
        }
        
        private void OnClickItem()
        {
            if (_bagNobId == -1) return;
            
            bool canGive = false;
            InventoryHandlerData.CanGiveItem(PlayerHandlerData.LocalPlayerClientId, _holdingItemId, _holdingAmount, ref canGive);
            if (!canGive) return;
            
            InventoryHandlerData.C_RetrieveItemFromBag(_bagNobId, _holdingItemId);
            Destroy(gameObject);
        }

        public void DelayAction(float delayTime)
        {
            if (_delayCoroutine != null)
            {
                float timeSinceCoroutine = Time.time - _lastDelayTime;
                float timeLeftForDelay = _lastDelay - timeSinceCoroutine;
                if (timeLeftForDelay > delayTime) return;
                StopCoroutine(_delayCoroutine);
            }
            
            _delayCoroutine = StartCoroutine(DelayActionCoroutine(delayTime));
            _lastDelay = delayTime;
            _lastDelayTime = Time.time;
        }

        private IEnumerator DelayActionCoroutine(float delayTime)
        {
            float lerpSpeed = 1f / delayTime;
            
            float lerp = 1f;
            while (lerp > 0)
            {
                lerp -= Time.deltaTime * lerpSpeed;
                _delayImage.fillAmount = lerp;
                yield return null;
            }
        }
    }
}