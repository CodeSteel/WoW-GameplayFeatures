using System.Collections;
using SteelBox;
using UnityEngine;
using UnityEngine.UI;

namespace MMO.InteractionSystem
{
    public class InteractionUIController : BaseMonoBehaviour
    {
        [SerializeField] private GameObject _interactionCanvas;
        [SerializeField] private GameObject _interactionCanceledCanvas;
        [SerializeField] private TMPro.TextMeshProUGUI _timerText;
        [SerializeField] private Image _timerBar;

        private Coroutine _interactionCoroutine;
        
        protected override void RegisterEvents()
        {
            InteractionHandlerData.C_OnInteractionStarted += C_OnInteractionStarted;
            InteractionHandlerData.C_OnInteractionCanceled += C_OnInteractionCanceled;
        }

        protected override void UnregisterEvents()
        {
            InteractionHandlerData.C_OnInteractionStarted -= C_OnInteractionStarted;
            InteractionHandlerData.C_OnInteractionCanceled -= C_OnInteractionCanceled;
        }

        private void C_OnInteractionStarted(IInteraction interaction)
        {
            StopInteraction();
            _interactionCoroutine = StartCoroutine(ShowInteraction(interaction));
        }

        private void C_OnInteractionCanceled()
        {
            StopInteraction();
            _interactionCoroutine = StartCoroutine(ShowInteractionCanceled());
        }

        private IEnumerator ShowInteraction(IInteraction interaction)
        {
            float secondsLeft = interaction.InteractionTime;
            
            _timerBar.fillAmount = 0;
            _timerText.SetText(secondsLeft.ToString("0"));
            
            _interactionCanvas.SetActive(true);
            while (secondsLeft > 0)
            {
                yield return new WaitForSeconds(0.1f);
                secondsLeft -= 0.1f;
                _timerText.SetText(secondsLeft.ToString("0"));
                _timerBar.fillAmount = interaction.InteractionTime / secondsLeft;
            }
            
            _interactionCanvas.SetActive(false);
        }

        private IEnumerator ShowInteractionCanceled()
        {
            _interactionCanceledCanvas.SetActive(true);
            yield return new WaitForSeconds(0.4f);
            _interactionCanceledCanvas.SetActive(false);
        }

        private void StopInteraction()
        {
            if (_interactionCoroutine != null)
                StopCoroutine(_interactionCoroutine);
            _interactionCanvas.SetActive(false);
            _interactionCanceledCanvas.SetActive(false);
        }
    }
}
