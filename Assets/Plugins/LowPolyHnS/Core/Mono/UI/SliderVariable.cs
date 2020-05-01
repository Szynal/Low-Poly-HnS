using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/UI/Slider", 10)]
    public class SliderVariable : Slider
    {
        [VariableFilter(Variable.DataType.Number)]
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
                value = (float) current;
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

        private void SyncValueWithList(int index, object prevElem, object newElem)
        {
            SyncValueWithVariable(string.Empty);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void SyncValueWithVariable(string variableName)
        {
            object current = variable.Get(gameObject);
            if (current != null)
            {
                float newValue = (float) current;
                if (!Mathf.Approximately(newValue, value))
                {
                    value = newValue;
                }
            }
        }

        protected virtual void SyncVariableWithValue(float newValue)
        {
            variable.Set(newValue, gameObject);
        }
    }
}