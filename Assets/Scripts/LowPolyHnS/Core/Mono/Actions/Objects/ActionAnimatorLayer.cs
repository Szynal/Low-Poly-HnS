using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionAnimatorLayer : IAction
    {
        public Animator animator;
        public int layerIndex = 1;
        [Range(0.0f, 1.0f)] public float weight = 0.0f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            animator.SetLayerWeight(layerIndex, weight);
            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Animation/Animator Layer";
        private const string NODE_TITLE = "Change {0}'s layer {1} to weight {2}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spAnimator;
        private SerializedProperty spLayerIndex;
        private SerializedProperty spWeight;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE,
                animator == null ? "nothing" : animator.gameObject.name,
                layerIndex, weight
            );
        }

        protected override void OnEnableEditorChild()
        {
            spAnimator = serializedObject.FindProperty("animator");
            spLayerIndex = serializedObject.FindProperty("layerIndex");
            spWeight = serializedObject.FindProperty("weight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spAnimator);
            EditorGUILayout.PropertyField(spLayerIndex);
            EditorGUILayout.PropertyField(spWeight);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}