using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class GameInputModule : StandaloneInputModule
    {
        public GameObject GameObjectUnderPointer(int pointerId)
        {
            PointerEventData lastPointer = GetLastPointerEventData(pointerId);
            return lastPointer?.pointerCurrentRaycast.gameObject;
        }

        public GameObject GameObjectUnderPointer()
        {
            return GameObjectUnderPointer(kMouseLeftId);
        }
    }
}