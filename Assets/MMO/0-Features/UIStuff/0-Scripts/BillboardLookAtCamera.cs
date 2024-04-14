using UnityEngine;

namespace MMO
{
    public class BillboardLookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            if (Camera.main != null) transform.LookAt(Camera.main.transform);
        }
    }
}
