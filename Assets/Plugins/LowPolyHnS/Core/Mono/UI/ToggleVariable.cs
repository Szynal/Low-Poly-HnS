using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("LowPolyHnS/UI/Toggle", 10)]
    public class ToggleVariable : Toggle
    {
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty variable = new VariableProperty();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            EventSystemManager.Instance.Wakeup();
        }

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;

            object current = variable.Get(gameObject);

            if (current != null)
            {
                isOn = (bool) current;
                onValueChanged.AddListener(SyncVariableWithValue);
            }

            switch (variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        SyncValueWithVariable,
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        SyncValueWithVariable,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        SyncValueWithList,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!Application.isPlaying) return;

            switch (variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        SyncValueWithVariable,
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        SyncValueWithVariable,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        SyncValueWithList,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SyncValueWithVariable(string variableName)
        {
            object current = variable.Get(gameObject);
            if (current != null)
            {
                bool variableValue = (bool) current;
                isOn = variableValue;
            }
        }

        private void SyncValueWithList(int index, object prevElem, object newElem)
        {
            SyncValueWithVariable(string.Empty);
        }

        private void SyncVariableWithValue(bool state)
        {
            variable.Set(state, gameObject);
        }
    }
}