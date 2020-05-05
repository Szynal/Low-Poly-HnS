using System;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public abstract class IHPMonoBehaviour<TData> : MonoBehaviour where TData : IHPMonoBehaviour<TData>.IData
    {
        [Serializable]
        public abstract class IData
        {
            public Hotspot hotspot;
            public IHPMonoBehaviour<TData> instance;
            public bool enabled = false;

            public void Setup(Hotspot hotspot, GameObject instance)
            {
                this.hotspot = hotspot;
                this.instance = instance.GetComponent<IHPMonoBehaviour<TData>>();
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public TData data;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static TData Create<THotspot>(Hotspot hotspot, TData data) where THotspot : IHPMonoBehaviour<TData>
        {
            GameObject instance = new GameObject(typeof(THotspot).ToString());
            instance.transform.SetParent(hotspot.transform, false);

            THotspot component = instance.AddComponent<THotspot>();
            component.data = data;
            component.ConfigureData(hotspot, instance);
            component.Initialize();
            return component.data;
        }

        private void ConfigureData(Hotspot hotspot, GameObject instance)
        {
            data.hotspot = hotspot;
            data.instance = instance.GetComponent<IHPMonoBehaviour<TData>>();
        }

        // ABSTRACT & VIRTUAL METHODS: ------------------------------------------------------------

        public abstract void Initialize();

        public virtual void HotspotMouseEnter()
        {
        }

        public virtual void HotspotMouseExit()
        {
        }

        public virtual void HotspotMouseOver()
        {
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected bool IsWithinConstrainedRadius()
        {
            if (data.hotspot.trigger == null) return true;
            if (!data.hotspot.trigger.minDistance) return true;
            if (HookPlayer.Instance == null) return false;

            float distance = Vector3.Distance(
                HookPlayer.Instance.transform.position,
                transform.position
            );

            return distance <= data.hotspot.trigger.minDistanceToPlayer;
        }
    }
}