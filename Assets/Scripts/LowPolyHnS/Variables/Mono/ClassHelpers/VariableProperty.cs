using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class VariableProperty
    {
        public enum GetVarType
        {
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public GetVarType variableType = GetVarType.GlobalVariable;

        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableProperty()
        {
            SetupVariables();
        }

        public VariableProperty(Variable.VarType variableType)
        {
            SetupVariables();
            switch (variableType)
            {
                case Variable.VarType.GlobalVariable:
                    this.variableType = GetVarType.GlobalVariable;
                    break;

                case Variable.VarType.LocalVariable:
                    this.variableType = GetVarType.LocalVariable;
                    break;

                case Variable.VarType.ListVariable:
                    this.variableType = GetVarType.ListVariable;
                    break;
            }
        }

        public VariableProperty(GetVarType variableType)
        {
            SetupVariables();
            this.variableType = variableType;
        }

        private void SetupVariables()
        {
            global = global ?? new HelperGlobalVariable();
            local = local ?? new HelperLocalVariable();
            list = list ?? new HelperGetListVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(GameObject invoker = null)
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable: return global.Get(invoker);
                case GetVarType.LocalVariable: return local.Get(invoker);
                case GetVarType.ListVariable: return list.Get(invoker);
            }

            return null;
        }

        public void Set(object value, GameObject invoker = null)
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable:
                    global.Set(value, invoker);
                    break;
                case GetVarType.LocalVariable:
                    local.Set(value, invoker);
                    break;
                case GetVarType.ListVariable:
                    list.Set(value, invoker);
                    break;
            }
        }

        public string GetVariableID()
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable: return global.name;
                case GetVarType.LocalVariable: return local.name;
                case GetVarType.ListVariable: return list.select.ToString();
            }

            return "";
        }

        public Variable.VarType GetVariableType()
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable: return Variable.VarType.GlobalVariable;
                case GetVarType.LocalVariable: return Variable.VarType.LocalVariable;
                case GetVarType.ListVariable: return Variable.VarType.ListVariable;
            }

            return Variable.VarType.GlobalVariable;
        }

        #region TEMPORAL_FIX

        private const string UPGRADE_WARN1 = "<b>LowPolyHnS Warning:</b> Unhandled method upgrade. ";
        private const string UPGRADE_WARN2 = "Please, report this message to hello@LowPolyHnS.io";

        // TODO: Remove in upgrade
        public GameObject GetLocalVariableGameObject()
        {
            Debug.LogWarning(UPGRADE_WARN1 + UPGRADE_WARN2);
            return GetLocalVariableGameObject(null);
        }

        // TODO: Remove in upgrade
        public GameObject GetListVariableGameObject()
        {
            Debug.LogWarning(UPGRADE_WARN1 + UPGRADE_WARN2);
            return GetListVariableGameObject(null);
        }

        #endregion

        public GameObject GetLocalVariableGameObject(GameObject invoker)
        {
            return local.GetGameObject(invoker);
        }

        public GameObject GetListVariableGameObject(GameObject invoker)
        {
            return list.GetGameObject(invoker);
        }

        public Variable.DataType GetVariableDataType(GameObject invoker)
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable:
                    return global.GetDataType();

                case GetVarType.LocalVariable:
                    GameObject targetLocal = local.GetGameObject(invoker);
                    return local.GetDataType(targetLocal);

                case GetVarType.ListVariable:
                    GameObject targetList = local.GetGameObject(invoker);
                    return list.GetDataType(targetList);
            }

            return Variable.DataType.Null;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
        {
            string varName = "";
            switch (variableType)
            {
                case GetVarType.GlobalVariable:
                    varName = global.ToString();
                    break;
                case GetVarType.LocalVariable:
                    varName = local.ToString();
                    break;
                case GetVarType.ListVariable:
                    varName = list.ToString();
                    break;
            }

            return string.IsNullOrEmpty(varName) ? "(unknown)" : varName;
        }

        public string ToStringValue(GameObject invoker)
        {
            switch (variableType)
            {
                case GetVarType.GlobalVariable: return global.ToStringValue(invoker);
                case GetVarType.LocalVariable: return local.ToStringValue(invoker);
                case GetVarType.ListVariable: return list.ToStringValue(invoker);
            }

            return "unknown";
        }
    }
}