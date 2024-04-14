using Cinemachine;
using MMO.InteractionSystem;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public enum CameraLookState
    {
        NoLook,
        FreeLook,
        ConstrainedLook
    }

    public class CameraMovementController : BaseMonoBehaviour
    {
        public CameraLookState CameraLookState = CameraLookState.ConstrainedLook;
        
        [SerializeField]
        private CinemachineFreeLook _cinemachineFreeLook;
        [SerializeField]
        private float _resetCameraXSpeed;
        
        private float _cameraZoomAmount = 25f;
        private bool _isCursorOverUI;
        private bool _cursorVisibilityLocked;

        protected override void RegisterEvents()
        {
            CameraMovementHandlerData.OnGetCameraLookState += OnGetCameraLookState;
            CameraMovementHandlerData.OnAttachCameraToPlayer += OnAttachCameraToPlayer;
        }

        protected override void UnregisterEvents()
        {
            CameraMovementHandlerData.OnGetCameraLookState -= OnGetCameraLookState;
            CameraMovementHandlerData.OnAttachCameraToPlayer -= OnAttachCameraToPlayer;
        }
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }
        
        private void Update()
        {
            CameraLookState prevLookState = CameraLookState; 
            if (Input.GetMouseButtonDown(0))
                CameraLookState = CameraLookState.FreeLook;
            else if (Input.GetMouseButtonUp(0))
                CameraLookState = Input.GetMouseButton(1) ? CameraLookState.ConstrainedLook : CameraLookState.NoLook;
            else if (Input.GetMouseButtonDown(1))
                CameraLookState = CameraLookState.ConstrainedLook;
            else if (Input.GetMouseButtonUp(1))
                CameraLookState = Input.GetMouseButton(0) ? CameraLookState.FreeLook : CameraLookState.NoLook;

            if (Cursor.visible)
                UIHandlerData.GetIsMouseOverUI(ref _isCursorOverUI);
            if (prevLookState != CameraLookState && !_isCursorOverUI)
            {
                InteractionHandlerData.C_GetMouseOverInteraction(ref _isCursorOverUI);
                _cursorVisibilityLocked = _isCursorOverUI;
            }
            Cursor.visible = _cursorVisibilityLocked || (CameraLookState == CameraLookState.NoLook || _isCursorOverUI);

            if (!Cursor.visible)
                CameraMovementHandlerData.ClickGameScreen();
            
            _cinemachineFreeLook.m_BindingMode = CameraLookState == CameraLookState.NoLook && (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                ? CinemachineTransposer.BindingMode.LockToTargetWithWorldUp
                : CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

            if (CameraLookState == CameraLookState.NoLook && 
                (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0))
            {
                _cinemachineFreeLook.m_XAxis.Value = Mathf.Lerp(_cinemachineFreeLook.m_XAxis.Value, 0, _resetCameraXSpeed * Time.deltaTime);
            }

            _cinemachineFreeLook.m_YAxisRecentering.m_enabled = CameraLookState == CameraLookState.NoLook && Mathf.Abs(Input.GetAxis("Vertical")) > 0;
            
            float scrollDelta = -Input.mouseScrollDelta.y * 2f;
            _cameraZoomAmount = Mathf.Clamp(_cameraZoomAmount + scrollDelta, 0f, 80f);
            // top rig
            _cinemachineFreeLook.m_Orbits[0].m_Radius = 0;
            _cinemachineFreeLook.m_Orbits[0].m_Height = _cameraZoomAmount / 1.5f;
            // middle rig
            _cinemachineFreeLook.m_Orbits[1].m_Radius = _cameraZoomAmount / 2f;
            _cinemachineFreeLook.m_Orbits[1].m_Height = _cameraZoomAmount / 2f;
            // bottom rig
            _cinemachineFreeLook.m_Orbits[2].m_Radius = _cameraZoomAmount / 2f;
            _cinemachineFreeLook.m_Orbits[2].m_Height = 0;
        }
        
        private float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
                return CameraLookState != CameraLookState.NoLook && !_isCursorOverUI ? Input.GetAxis("Mouse X") : 0;
            if (axisName == "Mouse Y")
                return CameraLookState != CameraLookState.NoLook && !_isCursorOverUI ? Input.GetAxis("Mouse Y") : 0;
            return 0;
        }

        private void OnGetCameraLookState(ref CameraLookState lookState)
        {
            lookState = CameraLookState;
        }

        private void OnAttachCameraToPlayer(Transform playerTranform)
        {
            _cinemachineFreeLook.Follow = playerTranform;
            _cinemachineFreeLook.LookAt = playerTranform;
        }
    }
}
