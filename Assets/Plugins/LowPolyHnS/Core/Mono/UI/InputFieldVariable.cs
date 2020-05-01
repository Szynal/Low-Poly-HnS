using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("LowPolyHnS/UI/Input Field", 10)]
    public class InputFieldVariable : InputField
    {
        [VariableFilter(Variable.DataType.String, Variable.DataType.Number)]
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
                text = (string) current;
                onValueChanged.AddListener(SyncVariable);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SyncVariable(string value)
        {
            variable.Set(value, gameObject);
        }
    }
}