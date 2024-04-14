using System;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public static class CameraMovementHandlerData
    {
        public static event ActionRef<CameraLookState> OnGetCameraLookState;
        public static void GetCameraLookState(ref CameraLookState lookState) =>
            OnGetCameraLookState?.Invoke(ref lookState);

        public static event Action<Transform> OnAttachCameraToPlayer;
        public static void AttachCameraToPlayer(Transform playerTransform) =>
            OnAttachCameraToPlayer?.Invoke(playerTransform);

        public static event Action OnClickGameScreen;
        public static void ClickGameScreen() => OnClickGameScreen?.Invoke();
    }
}