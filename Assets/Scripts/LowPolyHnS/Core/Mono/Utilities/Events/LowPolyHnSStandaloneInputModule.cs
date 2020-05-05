using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class LowPolyHnSStandaloneInputModule : StandaloneInputModule
    {
        public GameObject GameObjectUnderPointer(int pointerId)
        {
            var lastPointer = GetLastPointerEventData(pointerId);
            if (lastPointer != null)
            {
                return lastPointer.pointerCurrentRaycast.gameObject;
            }

            return null;
        }

        public GameObject GameObjectUnderPointer()
        {
            return GameObjectUnderPointer(kMouseLeftId);
        }
    }
}