using System.Collections.Generic;
using SteelBox;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MMO
{
    public class UIManager : BaseMonoBehaviour
    {
        private int _uiLayer;

        private void Start()
        {
            _uiLayer = LayerMask.NameToLayer("UI");
        }

        protected override void RegisterEvents()
        {
            UIHandlerData.OnGetIsMouseOverUI += OnGetIsMouseOverUI;
        }

        protected override void UnregisterEvents()
        {
            UIHandlerData.OnGetIsMouseOverUI -= OnGetIsMouseOverUI;
        }

        private void OnGetIsMouseOverUI(ref bool isMouseOverUI)
        {
            isMouseOverUI = IsPointerOverUIElement(GetEventSystemRaycastResults());
        }

        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == _uiLayer)
                    return true;
            }

            return false;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}