using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMO.AnimalSystem
{
    public class AnimalUIController : MonoBehaviour
    {
        [SerializeField] private GameObject _healthBarObject;
        [SerializeField] private Image _healthBarImage;
        [SerializeField] private TextMeshProUGUI _healthAmountText;

        private AnimalController _animal;

        private void Awake()
        {
            _animal = GetComponent<AnimalController>();
            _animal.OnHealthChangeEvent += OnHealthChangeEvent;
        }

        private void OnHealthChangeEvent()
        {
            _healthBarObject.SetActive(_animal.Sync_HealthValue.Value > 0);
            _healthBarImage.fillAmount = _animal.Sync_HealthValue.Value / (float)_animal.MaxHealth;
            _healthAmountText.text = _animal.Sync_HealthValue.Value.ToString();
        }
    }
}