using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionTransform : IAction
    {
        public enum RELATIVE
        {
            Local,
            Global
        }

        public enum PARENT
        {
            DontChange,
            ChangeParent,
            ClearParent
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);

        public PARENT changeParent = PARENT.DontChange;
        public TargetGameObject newParent = new TargetGameObject(TargetGameObject.Target.GameObject);

        public bool changePosition = false;
        public RELATIVE positionRelativity = RELATIVE.Global;
        public TargetPosition position = new TargetPosition();

        public bool changeRotation = false;
        public RELATIVE rotationRelativity = RELATIVE.Global;
        public Vector3 rotation = Vector3.zero;

        public bool changeScale = false;
        public Vector3 scale = Vector3.one;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                switch (changeParent)
                {
                    case PARENT.ChangeParent:
                        Transform newParentTransform = newParent.GetTransform(target);
                        if (newParentTransform != null) targetTrans.SetParent(newParentTransform);
                        break;

                    case PARENT.ClearParent:
                        targetTrans.SetParent(null);
                        break;
                }

                if (changePosition)
                {
                    switch (positionRelativity)
                    {
                        case RELATIVE.Local:
                            targetTrans.localPosition = position.GetPosition(target);
                            break;
                        case RELATIVE.Global:
                            targetTrans.position = position.GetPosition(target);
                            break;
                    }
                }

                if (changeRotation)
                {
                    switch (rotationRelativity)
                    {
                        case RELATIVE.Local:
                            targetTrans.localRotation = Quaternion.Euler(rotation);
                            break;
                        case RELATIVE.Global:
                            targetTrans.rotation = Quaternion.Euler(rotation);
                            break;
                    }
                }

                if (changeScale)
                {
                    targetTrans.localScale = scale;
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Transform";
        private const string NODE_TITLE = "Change {0} transform properties";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;

        private SerializedProperty spChangeParent;
        private SerializedProperty spNewParent;

        private SerializedProperty spChangePosition;
        private SerializedProperty spPositionRelativity;
        private SerializedProperty spPosition;

        private SerializedProperty spChangeRotation;
        private SerializedProperty spRotationRelativity;
        private SerializedProperty spRotation;

        private SerializedProperty spChangeScale;
        private SerializedProperty spScale;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, target);
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");

            spChangeParent = serializedObject.FindProperty("changeParent");
            spNewParent = serializedObject.FindProperty("newParent");

            spChangePosition = serializedObject.FindProperty("changePosition");
            spPositionRelativity = serializedObject.FindProperty("positionRelativity");
            spPosition = serializedObject.FindProperty("position");

            spChangeRotation = serializedObject.FindProperty("changeRotation");
            spRotationRelativity = serializedObject.FindProperty("rotationRelativity");
            spRotation = serializedObject.FindProperty("rotation");

            spChangeScale = serializedObject.FindProperty("changeScale");
            spScale = serializedObject.FindProperty("scale");
        }

        protected override void OnDisableEditorChild()
        {
            spTarget = null;
            spChangeParent = null;
            spNewParent = null;
            spChangePosition = null;
            spPositionRelativity = null;
            spPosition = null;
            spChangeRotation = null;
            spRotationRelativity = null;
            spRotation = null;
            spChangeScale = null;
            spScale = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);

            EditorGUILayout.PropertyField(spChangeParent);
            if (spChangeParent.intValue == (int) PARENT.ChangeParent)
            {
                EditorGUILayout.PropertyField(spNewParent);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(spChangePosition);
            if (spChangePosition.boolValue)
            {
                EditorGUILayout.PropertyField(spPositionRelativity);
                EditorGUILayout.PropertyField(spPosition);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(spChangeRotation);
            if (spChangeRotation.boolValue)
            {
                EditorGUILayout.PropertyField(spRotationRelativity);
                EditorGUILayout.PropertyField(spRotation);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(spChangeScale);
            if (spChangeScale.boolValue)
            {
                EditorGUILayout.PropertyField(spScale);
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}