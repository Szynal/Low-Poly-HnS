using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTransformMove : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        public TargetPosition moveTo = new TargetPosition();

        public bool rotate = true;
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);
        public Space space = Space.World;

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
                Vector3 position1 = targetTrans.position;
                Vector3 position2 = moveTo.GetPosition(target);

                if (moveTo.target == TargetPosition.Target.Position &&
                    space == Space.Self)
                {
                    position2 = targetTrans.TransformPoint(position2);
                }

                Vector3 targetRotation = lookAt.target == TargetPosition.Target.Invoker
                    ? position2
                    : lookAt.GetPosition(target);

                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.LookRotation(targetRotation - targetTrans.position);

                float vDuration = duration.GetValue(target);
                float initTime = Time.time;

                while (Time.time - initTime < vDuration && !forceStop)
                {
                    if (targetTrans == null) break;
                    float t = (Time.time - initTime) / vDuration;
                    float easeValue = Easing.GetEase(easing, 0.0f, 1.0f, t);

                    targetTrans.position = Vector3.LerpUnclamped(
                        position1,
                        position2,
                        easeValue
                    );

                    if (rotate)
                    {
                        targetTrans.rotation = Quaternion.LerpUnclamped(
                            rotation1,
                            rotation2,
                            easeValue
                        );
                    }

                    yield return null;
                }

                if (!forceStop && targetTrans != null)
                {
                    targetTrans.position = position2;
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

        public static new string NAME = "Object/Transform Move";
        private const string NODE_TITLE = "Move {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spMoveTo;
        private SerializedProperty spSpace;
        private SerializedProperty spRotate;
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
            spMoveTo = serializedObject.FindProperty("moveTo");
            spSpace = serializedObject.FindProperty("space");
            spRotate = serializedObject.FindProperty("rotate");
            spLookAt = serializedObject.FindProperty("lookAt");
            spDuration = serializedObject.FindProperty("duration");
            spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spMoveTo = null;
            spSpace = null;
            spRotate = null;
            spLookAt = null;
            spDuration = null;
            spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spMoveTo);

            int targetIndex = spMoveTo.FindPropertyRelative("target").enumValueIndex;
            if (targetIndex == (int) TargetPosition.Target.Position)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spSpace);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spRotate);
            EditorGUI.BeginDisabledGroup(!spRotate.boolValue);
            EditorGUILayout.PropertyField(spLookAt);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDuration);
            EditorGUILayout.PropertyField(spEasing);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}