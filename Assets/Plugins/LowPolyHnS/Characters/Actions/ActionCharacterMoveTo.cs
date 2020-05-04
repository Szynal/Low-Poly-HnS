using System.Collections;
using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterMoveTo : IAction
    {
        public enum MOVE_TO
        {
            Position,
            Transform,
            Marker,
            Variable
        }

        public TargetCharacter target = new TargetCharacter();

        public MOVE_TO moveTo = MOVE_TO.Position;
        public bool waitUntilArrives = true;

        public Vector3 position;
        public new Transform transform;
        public NavigationMarker marker;

        [VariableFilter(Variable.DataType.GameObject, Variable.DataType.Vector3)]
        public VariableProperty variable = new VariableProperty(Variable.VarType.LocalVariable);

        public bool cancelable = false;
        public float cancelDelay = 1.0f;

        [Range(0.0f, 5.0f)] [Tooltip("Threshold distance from the target that is considered as reached")]
        public float stopThreshold = 0.0f;

        private Character character;
        private bool forceStop;

        private bool wasControllable;
        private bool isCharacterMoving;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (waitUntilArrives) return false;
            character = this.target.GetCharacter(target);

            Vector3 cPosition = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            float cStopThresh = 0f;

            GetTarget(character, target, ref cPosition, ref cRotation, ref cStopThresh);
            cStopThresh = Mathf.Max(cStopThresh, stopThreshold);

            character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh);
            return true;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            forceStop = false;
            character = this.target.GetCharacter(target);

            Vector3 cPosition = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            float cStopThresh = stopThreshold;

            GetTarget(character, target, ref cPosition, ref cRotation, ref cStopThresh);
            cStopThresh = Mathf.Max(cStopThresh, stopThreshold);

            isCharacterMoving = true;
            wasControllable = character.characterLocomotion.isControllable;
            character.characterLocomotion.SetIsControllable(false);

            character.characterLocomotion.SetTarget(cPosition, cRotation, cStopThresh, CharacterArrivedCallback);

            bool canceled = false;
            float initTime = Time.time;

            while (isCharacterMoving && !canceled && !forceStop)
            {
                if (cancelable && Time.time - initTime >= cancelDelay)
                {
                    canceled = Input.anyKey;
                }

                yield return null;
            }

            character.characterLocomotion.SetIsControllable(wasControllable);

            if (canceled) yield return int.MaxValue;
            else yield return 0;
        }

        public override void Stop()
        {
            forceStop = true;
            if (character == null) return;

            character.characterLocomotion.SetIsControllable(wasControllable);
            character.characterLocomotion.Stop();
        }

        public void CharacterArrivedCallback()
        {
            isCharacterMoving = false;
        }

        private void GetTarget(Character targetCharacter, GameObject invoker,
            ref Vector3 cPosition, ref ILocomotionSystem.TargetRotation cRotation, ref float cStopThresh)
        {
            cStopThresh = 0.0f;
            switch (moveTo)
            {
                case MOVE_TO.Position:
                    cPosition = position;
                    break;
                case MOVE_TO.Transform:
                    cPosition = transform.position;
                    break;
                case MOVE_TO.Marker:
                    cPosition = marker.transform.position;
                    cRotation = new ILocomotionSystem.TargetRotation(true, marker.transform.forward);
                    cStopThresh = marker.stopThreshold;
                    break;

                case MOVE_TO.Variable:
                    object valueVariable = variable.Get(invoker);
                    switch (variable.GetVariableDataType(invoker))
                    {
                        case Variable.DataType.GameObject:
                            GameObject variableGo = valueVariable as GameObject;
                            if (variableGo == null)
                            {
                                if (targetCharacter != null) cPosition = targetCharacter.transform.position;
                                return;
                            }

                            NavigationMarker varMarker = variableGo.GetComponent<NavigationMarker>();
                            if (varMarker != null)
                            {
                                cPosition = varMarker.transform.position;
                                cRotation = new ILocomotionSystem.TargetRotation(true, varMarker.transform.forward);
                                cStopThresh = varMarker.stopThreshold;
                            }
                            else cPosition = variableGo.transform.position;

                            break;

                        case Variable.DataType.Vector3:
                            cPosition = (Vector3) valueVariable;
                            break;
                    }

                    break;
            }
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Move Character";
        private const string NODE_TITLE = "Move {0} to {1} {2}";

        private static readonly GUIContent GC_CANCEL = new GUIContent("Cancelable");

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spMoveTo;
        private SerializedProperty spWaitUntilArrives;
        private SerializedProperty spPosition;
        private SerializedProperty spTransform;
        private SerializedProperty spMarker;
        private SerializedProperty spVariable;

        private SerializedProperty spStopThreshold;
        private SerializedProperty spCancelable;
        private SerializedProperty spCancelDelay;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string value = "none";
            switch (moveTo)
            {
                case MOVE_TO.Position:
                    value = string.Format("({0},{1},{2})", position.x, position.y, position.z);
                    break;

                case MOVE_TO.Transform:
                    value = transform == null ? "nothing" : transform.gameObject.name;
                    break;
                case MOVE_TO.Marker:
                    value = marker == null ? "nothing" : marker.gameObject.name;
                    break;

                case MOVE_TO.Variable:
                    value = variable.GetVariableID();
                    break;
            }

            return string.Format(
                NODE_TITLE,
                target,
                moveTo,
                value
            );
        }

        protected override void OnEnableEditorChild()
        {
            spTarget = serializedObject.FindProperty("target");
            spMoveTo = serializedObject.FindProperty("moveTo");
            spWaitUntilArrives = serializedObject.FindProperty("waitUntilArrives");
            spPosition = serializedObject.FindProperty("position");
            spTransform = serializedObject.FindProperty("transform");
            spMarker = serializedObject.FindProperty("marker");
            spVariable = serializedObject.FindProperty("variable");
            spStopThreshold = serializedObject.FindProperty("stopThreshold");
            spCancelable = serializedObject.FindProperty("cancelable");
            spCancelDelay = serializedObject.FindProperty("cancelDelay");
        }

        protected override void OnDisableEditorChild()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spMoveTo);

            switch ((MOVE_TO) spMoveTo.intValue)
            {
                case MOVE_TO.Position:
                    EditorGUILayout.PropertyField(spPosition);
                    break;
                case MOVE_TO.Transform:
                    EditorGUILayout.PropertyField(spTransform);
                    break;
                case MOVE_TO.Marker:
                    EditorGUILayout.PropertyField(spMarker);
                    break;
                case MOVE_TO.Variable:
                    EditorGUILayout.PropertyField(spVariable);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spStopThreshold);
            EditorGUILayout.PropertyField(spWaitUntilArrives);
            if (spWaitUntilArrives.boolValue)
            {
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
                Rect rectLabel = new Rect(
                    rect.x,
                    rect.y,
                    EditorGUIUtility.labelWidth,
                    rect.height
                );
                Rect rectCancenable = new Rect(
                    rectLabel.x + rectLabel.width,
                    rectLabel.y,
                    20f,
                    rectLabel.height
                );
                Rect rectDelay = new Rect(
                    rectCancenable.x + rectCancenable.width,
                    rectCancenable.y,
                    rect.width - (rectLabel.width + rectCancenable.width),
                    rectCancenable.height
                );

                EditorGUI.LabelField(rectLabel, GC_CANCEL);
                EditorGUI.PropertyField(rectCancenable, spCancelable, GUIContent.none);

                EditorGUI.BeginDisabledGroup(!spCancelable.boolValue);
                EditorGUI.PropertyField(rectDelay, spCancelDelay, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}