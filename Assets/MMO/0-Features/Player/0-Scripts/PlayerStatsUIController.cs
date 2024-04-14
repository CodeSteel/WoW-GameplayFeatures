using SteelBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMO
{
    public class PlayerStatsUIController : BaseMonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _levelText;
        [SerializeField]
        private Image _experienceBarImage;
        
        protected override void RegisterEvents()
        {
            PlayerHandlerData.C_OnExperienceChangedEvent += C_OnExperienceChangedEvent;
        }

        protected override void UnregisterEvents()
        {
            PlayerHandlerData.C_OnExperienceChangedEvent -= C_OnExperienceChangedEvent;
        }

        private void C_OnExperienceChangedEvent(int newExperience, int newLevel, int expNeededNextLevel)
        {
            Debug.Log($"ExpChangeEvent {newExperience}/{expNeededNextLevel}");
            _levelText.SetText(newLevel.ToString());
            _experienceBarImage.fillAmount = newExperience > 0 ? (float)newExperience / expNeededNextLevel : 0;
        }
    }
}