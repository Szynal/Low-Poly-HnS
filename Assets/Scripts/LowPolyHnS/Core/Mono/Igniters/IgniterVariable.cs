using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterVariable : Igniter
    {
        public VariableProperty variable = new VariableProperty();

#if UNITY_EDITOR
        public new static string NAME = "Variables/On Variable Change";
#endif

        private void Start()
        {
            switch (variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        OnVariableChange,
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        OnVariableChange,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        OnListChange,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        private void OnVariableChange(string variableID)
        {
            ExecuteTrigger(gameObject);
        }

        private void OnListChange(int index, object prevElem, object newElem)
        {
            ExecuteTrigger(gameObject);
        }

        private void OnDestroy()
        {
            switch (variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        OnVariableChange,
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        OnVariableChange,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        OnListChange,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }
    }
}