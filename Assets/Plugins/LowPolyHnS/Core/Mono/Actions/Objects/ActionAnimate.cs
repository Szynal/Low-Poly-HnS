using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionAnimate : IAction
    {
        public TargetGameObject animator = new TargetGameObject(TargetGameObject.Target.GameObject);

        public string parameterName = "Parameter Name";
        public AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;

        public int parameterInteger = 1;
        public float parameterFloat = 1.0f;
        public bool parameterBool = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Animator anim = animator.GetComponent<Animator>(target);
            if (anim == null) return true;

            if (anim != null)
            {
                switch (parameterType)
                {
                    case AnimatorControllerParameterType.Trigger:
                        anim.SetTrigger(parameterName);
                        break;

                    case AnimatorControllerParameterType.Int:
                        anim.SetInteger(parameterName, parameterInteger);
                        break;

                    case AnimatorControllerParameterType.Float:
                        anim.SetFloat(parameterName, parameterFloat);
                        break;

                    case AnimatorControllerParameterType.Bool:
                        anim.SetBool(parameterName, parameterBool);
                        break;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        private static readonly GUIContent GUICONTENT_ANIMATOR = new GUIContent("Animator");
        private static readonly GUIContent GUICONTENT_PARAM_NAME = new GUIContent("Parameter Name");
        private static readonly GUIContent GUICONTENT_PARAM_TYPE = new GUIContent("Parameter Type");

        private static readonly GUIContent GUICONTENT_PARAM_INT = new GUIContent("Integer");
        private static readonly GUIContent GUICONTENT_PARAM_FLO = new GUIContent("Float");
        private static readonly GUIContent GUICONTENT_PARAM_BOL = new GUIContent("Bool");

        public static new string NAME = "Animation/Animate";
        private const string NODE_TITLE = "Animate {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spAnimator;
        private SerializedProperty spParameterName;
        private SerializedProperty spParameterType;

        private SerializedProperty spParameterInteger;
        private SerializedProperty spParameterFloat;
        private SerializedProperty spParameterBool;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, animator);
        }

        protected override void OnEnableEditorChild()
        {
            spAnimator = serializedObject.FindProperty("animator");
            spParameterName = serializedObject.FindProperty("parameterName");
            spParameterType = serializedObject.FindProperty("parameterType");

            spParameterInteger = serializedObject.FindProperty("parameterInteger");
            spParameterFloat = serializedObject.FindProperty("parameterFloat");
            spParameterBool = serializedObject.FindProperty("parameterBool");
        }

        protected override void OnDisableEditorChild()
        {
            spAnimator = null;
            spParameterName = null;
            spParameterType = null;

            spParameterInteger = null;
            spParameterFloat = null;
            spParameterBool = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spAnimator, GUICONTENT_ANIMATOR);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spParameterName, GUICONTENT_PARAM_NAME);
            EditorGUILayout.PropertyField(spParameterType, GUICONTENT_PARAM_TYPE);
            int paramTypeInt = spParameterType.intValue;
            AnimatorControllerParameterType paramType = (AnimatorControllerParameterType) paramTypeInt;

            EditorGUI.indentLevel++;
            switch (paramType)
            {
                case AnimatorControllerParameterType.Int:
                    EditorGUILayout.PropertyField(spParameterInteger, GUICONTENT_PARAM_INT);
                    break;

                case AnimatorControllerParameterType.Float:
                    EditorGUILayout.PropertyField(spParameterFloat, GUICONTENT_PARAM_FLO);
                    break;

                case AnimatorControllerParameterType.Bool:
                    EditorGUILayout.PropertyField(spParameterBool, GUICONTENT_PARAM_BOL);
                    break;
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}