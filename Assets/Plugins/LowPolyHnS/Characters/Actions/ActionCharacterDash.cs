using LowPolyHnS.Core;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionCharacterDash : IAction
    {
        private static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        public enum Direction
        {
            CharacterMovement3D,
            TowardsTarget,
            TowardsPosition,
            MovementSidescrollXY,
            MovementSidescrollZY
        }

        public TargetCharacter character = new TargetCharacter(TargetCharacter.Target.Player);

        public Direction direction = Direction.CharacterMovement3D;
        public TargetGameObject target = new TargetGameObject();
        public TargetPosition position = new TargetPosition();

        public NumberProperty impulse = new NumberProperty(5f);
        public NumberProperty duration = new NumberProperty(0f);
        public float drag = 10f;

        [Space] public AnimationClip dashClipForward;
        public AnimationClip dashClipBackward;
        public AnimationClip dashClipRight;
        public AnimationClip dashClipLeft;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character characterTarget = character.GetCharacter(target);
            if (characterTarget == null) return true;

            CharacterLocomotion locomotion = characterTarget.characterLocomotion;
            CharacterAnimator animator = characterTarget.GetCharacterAnimator();
            Vector3 moveDirection = Vector3.zero;

            switch (direction)
            {
                case Direction.CharacterMovement3D:
                    moveDirection = locomotion.GetMovementDirection();
                    break;

                case Direction.TowardsTarget:
                    Transform targetTransform = this.target.GetTransform(target);
                    if (targetTransform != null)
                    {
                        moveDirection = targetTransform.position - characterTarget.transform.position;
                        moveDirection.Scale(PLANE);
                    }

                    break;

                case Direction.TowardsPosition:
                    Vector3 targetPosition = position.GetPosition(target);
                    moveDirection = targetPosition - characterTarget.transform.position;
                    moveDirection.Scale(PLANE);
                    break;

                case Direction.MovementSidescrollXY:
                    moveDirection = locomotion.GetMovementDirection();
                    moveDirection.Scale(new Vector3(1, 1, 0));
                    break;

                case Direction.MovementSidescrollZY:
                    moveDirection = locomotion.GetMovementDirection();
                    moveDirection.Scale(new Vector3(0, 1, 1));
                    break;
            }

            Vector3 charDirection = Vector3.Scale(
                characterTarget.transform.TransformDirection(Vector3.forward),
                PLANE
            );

            float angle = Vector3.SignedAngle(moveDirection, charDirection, Vector3.up);
            AnimationClip clip = null;

            if (angle <= 45f && angle >= -45f) clip = dashClipForward;
            else if (angle < 135f && angle > 45f) clip = dashClipLeft;
            else if (angle > -135f && angle < -45f) clip = dashClipRight;
            else clip = dashClipBackward;

            bool isDashing = characterTarget.Dash(
                moveDirection.normalized,
                impulse.GetValue(target),
                duration.GetValue(target),
                drag
            );

            if (isDashing && clip != null && animator != null)
            {
                animator.CrossFadeGesture(clip, 1f, null, 0.05f, 0.5f);
            }

            return true;
        }

#if UNITY_EDITOR
        public static new string NAME = "Character/Character Dash";
        private const string TITLE_NAME = "Character {0} dash {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                TITLE_NAME,
                character,
                direction
            );
        }

        private SerializedProperty spCharacter;
        private SerializedProperty spDirection;
        private SerializedProperty spTarget;
        private SerializedProperty spPosition;

        private SerializedProperty spImpulse;
        private SerializedProperty spDuration;
        private SerializedProperty spDrag;

        private SerializedProperty spDashForward;
        private SerializedProperty spDashBackward;
        private SerializedProperty spDashRight;
        private SerializedProperty spDashLeft;

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spDirection = serializedObject.FindProperty("direction");
            spTarget = serializedObject.FindProperty("target");
            spPosition = serializedObject.FindProperty("position");

            spImpulse = serializedObject.FindProperty("impulse");
            spDuration = serializedObject.FindProperty("duration");
            spDrag = serializedObject.FindProperty("drag");

            spDashForward = serializedObject.FindProperty("dashClipForward");
            spDashBackward = serializedObject.FindProperty("dashClipBackward");
            spDashRight = serializedObject.FindProperty("dashClipRight");
            spDashLeft = serializedObject.FindProperty("dashClipLeft");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spDirection = null;
            spTarget = null;
            spPosition = null;

            spImpulse = null;
            spDuration = null;
            spDrag = null;

            spDashForward = null;
            spDashBackward = null;
            spDashRight = null;
            spDashLeft = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spDirection);
            switch (spDirection.enumValueIndex)
            {
                case (int) Direction.TowardsTarget:
                    EditorGUILayout.PropertyField(spTarget);
                    break;

                case (int) Direction.TowardsPosition:
                    EditorGUILayout.PropertyField(spPosition);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spImpulse);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDuration);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDrag);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spDashForward);
            EditorGUILayout.PropertyField(spDashBackward);
            EditorGUILayout.PropertyField(spDashRight);
            EditorGUILayout.PropertyField(spDashLeft);

            serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}