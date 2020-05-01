using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTransformRotateTowards : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);

        public NumberProperty duration = new NumberProperty(1.0f);
        public Easing.EaseType easing = Easing.EaseType.QuadInOut;

        private bool forceStop;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            forceStop = false;
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                Vector3 targetRotation = lookAt.GetPosition(target);
                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.LookRotation(targetRotation - targetTrans.position);

                float vDuration = duration.GetValue(target);
                float initTime = Time.time;

                while (Time.time - initTime < vDuration && !forceStop)
                {
                    if (targetTrans == null) break;
                    float t = (Time.time - initTime) / vDuration;
                    float easeValue = Easing.GetEase(easing, 0.0f, 1.0f, t);

                    targetTrans.rotation = Quaternion.LerpUnclamped(
                        rotation1,
                        rotation2,
                        easeValue
                    );

                    yield return null;
                }

                if (!forceStop && targetTrans != null)
                {
                    targetTrans.rotation = rotation2;
                }
            }

            yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Transform Rotate Towards";
        private const string NODE_TITLE = "Rotate towards {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spLookAt;
        private SerializedProperty spDuration;
        private SerializedProperty spEasing;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, target);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spLookAt = serializedObject.FindProperty("lookAt");
            spDuration = serializedObject.FindProperty("duration");
            spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spLookAt = null;
            spDuration = null;
            spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spLookAt);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDuration);
            EditorGUILayout.PropertyField(spEasing);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}