using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using SteelBox;
using UnityEngine;

namespace MMO.InteractionSystem
{
    public class InteractionManager : BaseNetworkManager
    {
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private float _maxInteractionDistance;

        private Coroutine _delayedCoroutine;

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
            if (IsClientInitialized)
            {
                InteractionHandlerData.C_OnGetMouseOverInteraction -= C_OnGetMouseOverInteraction;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            InteractionHandlerData.C_OnGetMouseOverInteraction += C_OnGetMouseOverInteraction;
        }

        private void Update()
        {
            if (IsClientInitialized && Input.GetMouseButtonDown(1) &&
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 150f,
                    _interactionLayer) && hit.transform.TryGetComponent(out IInteraction interaction))
            {
                Vector3 localPlayerPosition = Vector3.zero;
                PlayerHandlerData.C_GetLocalPlayerPosition(ref localPlayerPosition);
                if (Vector3.Distance(hit.transform.position, localPlayerPosition) > _maxInteractionDistance) return;

                if (_delayedCoroutine != null)
                    StopCoroutine(_delayedCoroutine);
                _delayedCoroutine = StartCoroutine(C_DelayedInteraction(interaction));
            }
        }

        [Client]
        private IEnumerator C_DelayedInteraction(IInteraction interaction)
        {
            InteractionHandlerData.C_InteractionStarted(interaction);

            float timeLeft = interaction.InteractionTime;
            while (timeLeft > 0)
            {
                timeLeft -= 0.1f;
                yield return new WaitForSeconds(0.1f);
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                {
                    InteractionHandlerData.C_InteractionCanceled();
                    yield break;
                }
            }

            interaction.Interact(LocalConnection.ClientId, false);
            S_RpcInteraction(interaction.ObjectId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void S_RpcInteraction(int objectId, NetworkConnection conn = null)
        {
            if (!objectId.TryGetNetworkObjectFromObjectId(out NetworkObject networkObject))
            {
                Debug.LogWarning($"S_RpcInteraction failed - Couldn't find object id! ObjectId={objectId}");
                return;
            }

            if (!networkObject.TryGetComponent(out IInteraction interaction))
            {
                Debug.LogWarning($"S_RpcInteraction failed - Couldn't find IInteraction! ObjectId={objectId}");
                return;
            }

            interaction.Interact(conn.ClientId, true);
        }

        [Client]
        private void C_OnGetMouseOverInteraction(ref bool isOverInteraction)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 150f,
                    _interactionLayer) && hit.transform.TryGetComponent(out IInteraction interaction))
            {
                Vector3 localPlayerPosition = Vector3.zero;
                PlayerHandlerData.C_GetLocalPlayerPosition(ref localPlayerPosition);
                if (Vector3.Distance(hit.transform.position, localPlayerPosition) > _maxInteractionDistance) return;
                isOverInteraction = true;
            }
        }
    }
}