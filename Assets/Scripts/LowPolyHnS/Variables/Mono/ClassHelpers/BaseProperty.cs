using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public abstract class BaseProperty<T>
    {
        public enum OPTION
        {
            Value,
            UseGlobalVariable,
            UseLocalVariable,
            UseListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public OPTION optionIndex = OPTION.Value;
        public T value;

        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected BaseProperty()
        {
            value = default;
            SetupVariables();
        }

        protected BaseProperty(T value)
        {
            this.value = value;
            SetupVariables();
        }

        private void SetupVariables()
        {
            global = global ?? new HelperGlobalVariable();
            local = local ?? new HelperLocalVariable();
            list = list ?? new HelperGetListVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T GetValue()
        {
            Debug.LogWarning(
                "This should not be called but it's here for legacy purposes. " +
                "Please contact us at: marti@catsoft-studios.com. Thanks!"
            );

            return GetValue(null);
        }

        public T GetValue(GameObject invoker)
        {
            switch (optionIndex)
            {
                case OPTION.Value: return value;
                case OPTION.UseGlobalVariable: return (T) global.Get();
                case OPTION.UseLocalVariable: return (T) local.Get(invoker);
                case OPTION.UseListVariable: return (T) list.Get(invoker);
            }

            return default;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            switch (optionIndex)
            {
                case OPTION.Value: return GetValueName();
                case OPTION.UseGlobalVariable: return "(Global Variable)";
                case OPTION.UseLocalVariable: return "(Local Variable)";
                case OPTION.UseListVariable: return "(List Variable)";
            }

            return "unknown";
        }

        protected virtual string GetValueName()
        {
            return value == null
                ? "(none)"
                : value.ToString();
        }
    }
}