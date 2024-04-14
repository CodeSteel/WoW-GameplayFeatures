using System;
using FishNet.Connection;
using FishNet.Object;
using SteelBox;
using UnityEngine;

namespace MMO
{
    public class NoticeManager : BaseNetworkManager
    {
        [SerializeField] private Transform _noticesUIParent;
        [SerializeField] private NoticeUIController _noticeUIControllerPrefab;
        
        protected override void RegisterEvents()
        {
            if (IsServerInitialized)
            {
                NoticeHandlerData.S_OnDisplayNotice += S_OnDisplayNotice;
            }
        }
        
        protected override void UnregisterEvents()
        {
            if (IsServerInitialized)
            {
                NoticeHandlerData.S_OnDisplayNotice -= S_OnDisplayNotice;
            }

            if (IsClientInitialized)
            {
                NoticeHandlerData.C_OnDisplayNotice -= C_OnDisplayNotice;
                NoticeHandlerData.C_OnDisplayCompleteNotice -= C_OnDisplayCompleteNotice;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            NoticeHandlerData.C_OnDisplayNotice += C_OnDisplayNotice;
            NoticeHandlerData.C_OnDisplayCompleteNotice += C_OnDisplayCompleteNotice;
        }

        [Server]
        private void S_OnDisplayNotice(int clientId, string title, string description)
        {
            if (!ServerManager.Clients.TryGetValue(clientId, out NetworkConnection conn)) return;
            C_TRpcDisplayNotice(conn, title, description);
        }

        [Client]
        private void C_OnDisplayNotice(string title, string description, string rewards, Action onAccept)
        {
            NoticeUIController controller = Instantiate(_noticeUIControllerPrefab, _noticesUIParent);
            controller.DisplayNotice(title, description, rewards, onAccept);
        }

        [Client]
        private void C_OnDisplayCompleteNotice(string title, string description, string rewards, Action onComplete)
        {
            NoticeUIController controller = Instantiate(_noticeUIControllerPrefab, _noticesUIParent);
            controller.DisplayNotice(title, description, rewards, onComplete, true);
        }
        
        [TargetRpc]
        private void C_TRpcDisplayNotice(NetworkConnection conn, string title, string description)
        {
            NoticeHandlerData.C_DisplayNotice(title, description, "");
        }
    }
}
