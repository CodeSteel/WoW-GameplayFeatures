using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMO
{
    public class NoticeUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _noticeTitle;
        [SerializeField] private TextMeshProUGUI _noticeDescription;
        [SerializeField] private TextMeshProUGUI _rewardsDescription;
        [SerializeField] private Button _noticeAcceptButton;
        [SerializeField] private TextMeshProUGUI _noticeAcceptButtonText;
        [SerializeField] private Button _noticeCloseButton;

        [CanBeNull] private Action _onAcceptAction;

        private void Start()
        {
            _noticeCloseButton.onClick.AddListener(OnClickCloseButton);
        }

        private void OnClickCloseButton()
        {
            Destroy(gameObject);
        }
        
        public void DisplayNotice(string title, string description, string rewards, Action onAccept, bool isComplete = false)
        {
            _noticeTitle.SetText(title);
            _noticeDescription.SetText(description);
            _rewardsDescription.SetText(rewards);
            _noticeAcceptButton.gameObject.SetActive(onAccept != null);
            _noticeAcceptButton.onClick.AddListener(OnClickAcceptButton);
            _onAcceptAction = onAccept;
            if (isComplete)
                _noticeAcceptButtonText.SetText("Complete");
            else
                _noticeAcceptButtonText.SetText("Accept");
        }

        private void OnClickAcceptButton()
        {
            _onAcceptAction?.Invoke();
            Destroy(gameObject);
        }
    }
}
