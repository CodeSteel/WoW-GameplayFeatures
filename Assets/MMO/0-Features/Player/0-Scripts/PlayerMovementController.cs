using SteelBox;
using UnityEngine;

namespace MMO
{
    public class PlayerMovementController : BaseNetworkBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _rotateSpeed;

        private Vector3 _lookDirection;
        private Vector3 _velocity;
        private bool _autoRunning;

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
            if (IsOwner)
            {
                 PlayerHandlerData.C_OnGetLocalPlayerPosition -= C_OnGetLocalPlayerPosition;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                CameraMovementHandlerData.AttachCameraToPlayer(transform);
                PlayerHandlerData.C_OnGetLocalPlayerPosition += C_OnGetLocalPlayerPosition;
            }
        }
        
        private void Update()
        {
            if (IsServerInitialized)
            {
                if (_characterController.velocity.magnitude > 0.1f)
                {
                    PlayerHandlerData.S_PlayerMoveEvent(OwnerId);
                }

                if (!IsClientInitialized)
                    return;
            }

            if (!IsOwner) return;

            if (_characterController.velocity.magnitude > 0.1f)
                PlayerHandlerData.C_PlayerMoveEvent();

            if (Input.GetMouseButtonDown(4) || Input.GetMouseButtonDown(5))
                _autoRunning = !_autoRunning;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (_autoRunning)
                v = 1;

            CameraLookState lookState = CameraLookState.FreeLook;
            CameraMovementHandlerData.GetCameraLookState(ref lookState);

            Vector3 movement = Vector3.zero;
            if (lookState == CameraLookState.ConstrainedLook)
            {
                Vector3 forwardDirection =
                    Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
                transform.forward = forwardDirection;
                movement = v * forwardDirection + h * transform.right;
            }
            else
            {
                transform.Rotate(0, h * _rotateSpeed * Time.deltaTime, 0);
                movement = transform.forward * v;
            }

            movement *= _walkSpeed;

            _characterController.Move(movement * Time.deltaTime);
        }

        private void C_OnGetLocalPlayerPosition(ref Vector3 position)
        {
            position = transform.position;
        }
    }
}