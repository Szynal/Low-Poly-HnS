using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionEnableComponent : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.GameObject);
        [Space] public string componentName = "";

        public BoolProperty enable = new BoolProperty(true);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (string.IsNullOrEmpty(componentName)) return true;

            GameObject targetGo = this.target.GetGameObject(target);
            Component component = targetGo.GetComponent(componentName);

            if (component != null)
            {
                bool value = enable.GetValue(target);
                if (component == null) return true;

                if (component is Renderer)
                {
                    (component as Renderer).enabled = value;
                }
                else if (component is Collider)
                {
                    (component as Collider).enabled = value;
                }
                else if (component is Animation)
                {
                    (component as Animation).enabled = value;
                }
                else if (component is Animator)
                {
                    (component as Animator).enabled = value;
                }
                else if (component is AudioSource)
                {
                    (component as AudioSource).enabled = value;
                }
                else if (component is MonoBehaviour)
                {
                    (component as MonoBehaviour).enabled = value;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Enable Component";
        private const string NODE_TITLE = "{0} component {1}";
        private const string SELECT_TEXT = "Select Target Component";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spComponentName;
        private SerializedProperty spEnable;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                enable,
                componentName
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spComponentName = serializedObject.FindProperty("componentName");
            spEnable = serializedObject.FindProperty("enable");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spComponentName = null;
            spEnable = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            EditorGUILayout.PropertyField(spComponentName);
            spComponentName.stringValue = spComponentName.stringValue.Replace(
                " ", string.Empty
            );

            EditorGUILayout.PropertyField(spEnable);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}