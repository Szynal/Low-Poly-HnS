using System;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class HPProximity : IHPMonoBehaviour<HPProximity.Data>
    {
        [Serializable]
        public class Data : IData
        {
            public GameObject prefab;
            public Vector3 offset;
            private GameObject prefabInstance;

            [Range(0.0f, 20.0f)] public float radius = 1.0f;

            public bool targetPlayer = true;
            public GameObject target;

            public void ChangeState(Transform parent, bool state)
            {
                if (prefab == null) return;
                if (prefabInstance == null)
                {
                    prefabInstance = Instantiate(prefab, parent);
                    prefabInstance.transform.localPosition = offset;
                    prefabInstance.transform.localRotation = Quaternion.identity;
                    prefabInstance.transform.localScale = Vector3.one;
                }

                prefabInstance.SetActive(state);
            }
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void Initialize()
        {
            if (!data.enabled) return;

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = data.radius;
        }

        // TRIGGER METHODS: -----------------------------------------------------------------------

        private void OnTriggerEnter(Collider collider)
        {
            if (!data.enabled || data.prefab == null) return;
            if (!HotspotIndicatorIsTarget(collider)) return;

            data.ChangeState(transform, true);
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!data.enabled || data.prefab == null) return;
            if (!HotspotIndicatorIsTarget(collider)) return;

            data.ChangeState(transform, false);
        }

        private bool HotspotIndicatorIsTarget(Collider collider)
        {
            if (data.targetPlayer &&
                collider.gameObject.GetInstanceID() == HookPlayer.Instance.gameObject.GetInstanceID())
            {
                return true;
            }

            if (!data.targetPlayer &&
                collider.gameObject.GetInstanceID() == data.target.GetInstanceID())
            {
                return true;
            }

            return false;
        }
    }
}