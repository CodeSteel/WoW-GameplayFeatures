using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

namespace MMO.InteractionSystem
{
    public class TestInteractableObject : NetworkBehaviour, IInteraction
    {
        public float InteractionTime => 2;
        public new int ObjectId => NetworkObject.ObjectId;

        public void Interact(int clientId, bool asServer)
        {
            if (asServer) return;
            GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0, 1, 0, 1);
        }
    }
}