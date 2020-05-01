using System.Collections;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTransformRotate : IAction
    {
        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        [Space] public Space space = Space.Self;
        public Vector3 rotation = new Vector3(90f, 0f, 0f);

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
                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.identity;
                switch (space)
                {
                    case Space.Self:
                        rotation2 = targetTrans.rotation *
                                    Quaternion.Euler(rotation);
                        break;

                    case Space.World:
                        rotation2 = Quaternion.Euler(rotation);
                        break;
                }

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

        public static new string NAME = "Object/Transform Rotate";
        private const string NODE_TITLE = "Rotate to {0} in {1} Space";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spSpace;
        private SerializedProperty spRotation;
        private SerializedProperty spDuration;
        private SerializedProperty spEasing;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, rotation, space);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spSpace = serializedObject.FindProperty("space");
            spRotation = serializedObject.FindProperty("rotation");
            spDuration = serializedObject.FindProperty("duration");
            spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spSpace = null;
            spRotation = null;
            spDuration = null;
            spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spSpace);
            EditorGUILayout.PropertyField(spRotation);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDuration);
            EditorGUILayout.PropertyField(spEasing);

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}