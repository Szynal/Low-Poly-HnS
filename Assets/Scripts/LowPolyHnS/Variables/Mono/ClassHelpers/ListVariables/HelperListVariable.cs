using System;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class HelperListVariable
    {
        public enum Target
        {
            Player,
            Camera,
            Invoker,
            GameObject
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target targetType = Target.GameObject;
        public GameObject targetObject;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public ListVariables GetListVariables(GameObject invoker)
        {
            switch (targetType)
            {
                case Target.Player:
                    if (HookPlayer.Instance == null) return null;
                    return HookPlayer.Instance.Get<ListVariables>();

                case Target.Camera:
                    if (HookCamera.Instance == null) return null;
                    return HookCamera.Instance.Get<ListVariables>();

                case Target.Invoker:
                    return invoker == null
                        ? null
                        : invoker.GetComponent<ListVariables>();

                case Target.GameObject:
                    return targetObject == null
                        ? null
                        : targetObject.GetComponent<ListVariables>();
            }

            return null;
        }

        public Variable.DataType GetDataType(GameObject invoker)
        {
            ListVariables list = GetListVariables(invoker);
            return list != null ? list.type : Variable.DataType.Null;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string ToString()
        {
            if (targetType == Target.GameObject)
            {
                return targetObject != null
                    ? targetObject.name
                    : "(null)";
            }

            return targetType.ToString();
        }
    }
}