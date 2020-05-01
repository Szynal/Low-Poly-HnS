namespace LowPolyHnS.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("LowPolyHnS/Hotspot", 0)]
	public class Hotspot : MonoBehaviour 
	{
		public HPCursor.Data cursorData;
		public HPProximity.Data proximityData;

        public Trigger trigger;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Awake()
		{
            this.trigger = GetComponent<Trigger>();

			this.cursorData = HPCursor.Create<HPCursor>(this, this.cursorData);
			this.proximityData = HPProximity.Create<HPProximity>(this, this.proximityData);
		}

		// INTERACTION METHODS: -------------------------------------------------------------------

		private void OnMouseEnter() 
		{ 
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseEnter();
		}

		private void OnMouseExit() 
		{ 
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseExit();
		}

        private void OnMouseOver()
        {
            if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseOver();
        }

        private void OnDestroy()
		{
			if (this.cursorData.enabled) this.cursorData.instance.HotspotMouseExit();
		}

		// GIZMO METHODS: -------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "LowPolyHnS/Hotspot/hotspot", true);
		}
	}
}