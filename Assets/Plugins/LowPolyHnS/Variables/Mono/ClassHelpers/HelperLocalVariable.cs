using System;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class HelperLocalVariable : BaseHelperVariable
    {
        public enum Target
        {
            Player,
            Invoker,
            GameObject,
            GameObjectPath
        }

        public string name = "";
        public Target targetType = Target.GameObject;
        public GameObject targetObject;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override object Get(GameObject invoker = null)
        {
            return VariablesManager.GetLocal(
                GetGameObject(invoker),
                name,
                true
            );
        }

        public override void Set(object value, GameObject invoker = null)
        {
            VariablesManager.SetLocal(
                GetGameObject(invoker),
                name,
                value,
                true
            );
        }

        public GameObject GetGameObject(GameObject invoker)
        {
            switch (targetType)
            {
                case Target.Player:
                    if (HookPlayer.Instance == null) return null;
                    return HookPlayer.Instance.gameObject;

                case Target.Invoker: return invoker;
                case Target.GameObject: return targetObject;
                case Target.GameObjectPath: return targetObject;
            }

            return null;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            return name;
        }

        public override string ToStringValue(GameObject invoker = null)
        {
            object value = VariablesManager.GetLocal(
                GetGameObject(invoker),
                name,
                true
            );

            return value != null ? value.ToString() : "null";
        }

        public override Variable.DataType GetDataType(GameObject invoker = null)
        {
            return VariablesManager.GetLocalType(invoker, name, true);
        }
    }
}