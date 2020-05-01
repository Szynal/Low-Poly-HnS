using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("LowPolyHnS/UI/Text (Variable)", 20)]
    public class TextVariable : Text
    {
        public string format = "Variable: {0}";
        public VariableProperty variable = new VariableProperty();

        private bool exitingApplication;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                UpdateText();
                SubscribeOnChange();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                UpdateText();
                SubscribeOnChange();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying && !exitingApplication)
            {
                UpdateText();
                UnsubscribeOnChange();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDisable();
            if (Application.isPlaying && !exitingApplication)
            {
                UpdateText();
                UnsubscribeOnChange();
            }
        }

        private void OnApplicationQuit()
        {
            exitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnUpdateVariable(string variableID)
        {
            if (string.IsNullOrEmpty(variableID)) return;
            if (variable.GetVariableID() != variableID) return;

            UpdateText();
        }

        private void OnUpdateList(int index, object prevElem, object newElem)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            string value = format;
            if (format.Contains("{0}"))
            {
                value = string.Format(
                    format,
                    variable.Get(gameObject)
                );
            }

            text = value;
        }

        private void SubscribeOnChange()
        {
            switch (variable.GetVariableType())
            {
                case Variable.VarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        OnUpdateVariable,
                        variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        OnUpdateVariable,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        OnUpdateList,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        private void UnsubscribeOnChange()
        {
            switch (variable.GetVariableType())
            {
                case Variable.VarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        OnUpdateVariable,
                        variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        OnUpdateVariable,
                        variable.GetLocalVariableGameObject(gameObject),
                        variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        OnUpdateList,
                        variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }
    }
}