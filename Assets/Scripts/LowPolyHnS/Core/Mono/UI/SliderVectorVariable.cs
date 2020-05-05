using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("LowPolyHnS/UI/Slider Vector", 10)]
    public class SliderVectorVariable : Slider
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        [VariableFilter(Variable.DataType.Vector2, Variable.DataType.Vector3)]
        public VariableProperty variable = new VariableProperty();

        public Axis component = Axis.X;

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
                Variable.DataType dataType = variable.GetVariableDataType(gameObject);
                switch (dataType)
                {
                    case Variable.DataType.Vector2:
                        if (component == Axis.X) value = ((Vector2) current).x;
                        if (component == Axis.Y) value = ((Vector2) current).y;
                        break;

                    case Variable.DataType.Vector3:
                        if (component == Axis.X) value = ((Vector3) current).x;
                        if (component == Axis.Y) value = ((Vector3) current).y;
                        if (component == Axis.Z) value = ((Vector3) current).z;
                        break;
                }

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

        protected void SyncValueWithVariable(string variableName)
        {
            object current = variable.Get(gameObject);
            if (current != null)
            {
                Variable.DataType dataType = variable.GetVariableDataType(gameObject);
                switch (dataType)
                {
                    case Variable.DataType.Vector2:
                        Vector2 newValue2 = (Vector2) current;
                        if (component == Axis.X) value = newValue2.x;
                        if (component == Axis.Y) value = newValue2.y;
                        break;

                    case Variable.DataType.Vector3:
                        Vector3 newValue3 = (Vector3) current;
                        if (component == Axis.X) value = newValue3.x;
                        if (component == Axis.Y) value = newValue3.y;
                        if (component == Axis.Z) value = newValue3.z;
                        break;
                }
            }
        }

        protected void SyncVariableWithValue(float newValue)
        {
            Variable.DataType dataType = variable.GetVariableDataType(gameObject);
            switch (dataType)
            {
                case Variable.DataType.Vector2:
                    Vector2 vec2 = (Vector2) variable.Get(gameObject);
                    if (component == Axis.X) vec2.x = newValue;
                    if (component == Axis.Y) vec2.y = newValue;
                    variable.Set(vec2, gameObject);
                    break;

                case Variable.DataType.Vector3:
                    Vector3 vec3 = (Vector3) variable.Get(gameObject);
                    if (component == Axis.X) vec3.x = newValue;
                    if (component == Axis.Y) vec3.y = newValue;
                    if (component == Axis.Z) vec3.z = newValue;
                    variable.Set(vec3, gameObject);
                    break;
            }
        }
    }
}