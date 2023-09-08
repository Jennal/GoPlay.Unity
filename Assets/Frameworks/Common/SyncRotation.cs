using UnityEngine;

namespace GoPlay.UI.Common
{
    [ExecuteInEditMode]
    public class SyncRotation : MonoBehaviour
    {
        public Transform Target;

        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            if (!Target) return;

            transform.localRotation = Target.localRotation;
        }
    }
}