using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/Hotspot", 0)]
    public class Hotspot : MonoBehaviour
    {
        public HPCursor.Data cursorData;
        public HPProximity.Data proximityData;

        public Trigger trigger;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void Awake()
        {
            trigger = GetComponent<Trigger>();

            cursorData = HPCursor.Create<HPCursor>(this, cursorData);
            proximityData = HPProximity.Create<HPProximity>(this, proximityData);
        }

        // INTERACTION METHODS: -------------------------------------------------------------------

        private void OnMouseEnter()
        {
            if (cursorData.enabled) cursorData.instance.HotspotMouseEnter();
        }

        private void OnMouseExit()
        {
            if (cursorData.enabled) cursorData.instance.HotspotMouseExit();
        }

        private void OnMouseOver()
        {
            if (cursorData.enabled) cursorData.instance.HotspotMouseOver();
        }

        private void OnDestroy()
        {
            if (cursorData.enabled) cursorData.instance.HotspotMouseExit();
        }

        // GIZMO METHODS: -------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "Hotspot/hotspot", true);
        }
    }
}