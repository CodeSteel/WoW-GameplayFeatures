using UnityEngine;
using UnityEngine.EventSystems;

namespace MMO
{
    public class DraggableSubWindow : MonoBehaviour, IDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rect;

        private void Start()
        {
            _rect = transform.parent.GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }
}