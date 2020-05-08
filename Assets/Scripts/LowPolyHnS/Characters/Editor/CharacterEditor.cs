using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    [CustomEditor(typeof(Character), true)]
    public class CharacterEditor : Editor
    {
        public class Section
        {
            private const string KEY_STATE = "character-section-{0}";

            public GUIContent name;
            public AnimBool state;

            public Section(string name, Texture2D icon, UnityAction repaint)
            {
                this.name = new GUIContent(string.Format(" {0}", name), icon);
                state = new AnimBool(GetState());
                state.speed = ANIM_BOOL_SPEED;
                state.valueChanged.AddListener(repaint);
            }

            public void PaintSection()
            {
                GUIStyle buttonStyle = state.target
                    ? CoreGUIStyles.GetToggleButtonNormalOn()
                    : CoreGUIStyles.GetToggleButtonNormalOff();

                if (GUILayout.Button(name, buttonStyle))
                {
                    state.target = !state.target;
                    string key = string.Format(KEY_STATE, name.text.GetHashCode());
                    EditorPrefs.SetBool(key, state.target);
                }
            }

            private bool GetState()
            {
                string key = string.Format(KEY_STATE, name.text.GetHashCode());
                return EditorPrefs.GetBool(key, true);
            }
        }

        // CONSTANTS: --------------------------------------------------------------------------------------------------

        public const string CHARACTER_ICONS_PATH = "Assets/Content/Icons/";

        private const float ANIM_BOOL_SPEED = 3f;
        private const string SECTION_CHAR_PARAMS1 = "Basic Parameters";
        private const string SECTION_CHAR_PARAMS2 = "Advanced Parameters";

        private const string PROP_CHAR_LOCOMOTION = "characterLocomotion";
        private const string PROP_MOVSYS = "movementSystem";
        private const string PROP_ISCONT = "isControllable";
        private const string PROP_CANRUN = "canRun";
        private const string PROP_RUNSPD = "runSpeed";
        private const string PROP_CANJMP = "canJump";
        private const string PROP_JMPFRC = "jumpForce";
        private const string PROP_JMPNUM = "jumpTimes";
        private const string PROP_JMPTIM = "timeBetweenJumps";
        private const string PROP_ANGSPD = "angularSpeed";
        private const string PROP_GRAVTY = "gravity";
        private const string PROP_FALLSP = "maxFallSpeed";
        private const string PROP_PUSHFC = "pushForce";
        private const string PROP_SAVEOB = "save";

        private const string PROP_FACEDR = "faceDirection";
        private const string PROP_FACEDT = "faceDirectionTarget";
        private const string PROP_NAVMES = "canUseNavigationMesh";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        protected Character character;
        protected Section sectionProperties1;
        protected Section sectionProperties2;

        protected SerializedProperty spIsControllable;
        protected SerializedProperty spRunSpeed;
        protected SerializedProperty spAngularSpeed;
        protected SerializedProperty spCanRun;
        protected SerializedProperty spCanJump;
        protected SerializedProperty spJumpForce;
        protected SerializedProperty spJumpCount;
        protected SerializedProperty spTimeJumps;
        protected SerializedProperty spGravity;
        protected SerializedProperty spMaxFallSpeed;
        protected SerializedProperty spPushForce;
        protected SerializedProperty spSave;

        protected SerializedProperty spFaceDirection;
        protected SerializedProperty spFaceDirectionTarget;
        protected SerializedProperty spUseNavmesh;

        // INITIALIZERS: -----------------------------------------------------------------------------------------------

        protected void OnEnable()
        {
            SerializedProperty spCharLocomotion = serializedObject.FindProperty(PROP_CHAR_LOCOMOTION);
            character = (Character) target;

            string iconProperties1Path = Path.Combine(CHARACTER_ICONS_PATH, "CharacterBasicParameters.png");
            Texture2D iconProperties1 = AssetDatabase.LoadAssetAtPath<Texture2D>(iconProperties1Path);
            sectionProperties1 = new Section(SECTION_CHAR_PARAMS1, iconProperties1, Repaint);

            string iconPropertiesPath2 = Path.Combine(CHARACTER_ICONS_PATH, "CharacterAdvancedParameters.png");
            Texture2D iconProperties2 = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPropertiesPath2);
            sectionProperties2 = new Section(SECTION_CHAR_PARAMS2, iconProperties2, Repaint);

            spIsControllable = spCharLocomotion.FindPropertyRelative(PROP_ISCONT);
            spRunSpeed = spCharLocomotion.FindPropertyRelative(PROP_RUNSPD);
            spAngularSpeed = spCharLocomotion.FindPropertyRelative(PROP_ANGSPD);
            spGravity = spCharLocomotion.FindPropertyRelative(PROP_GRAVTY);
            spMaxFallSpeed = spCharLocomotion.FindPropertyRelative(PROP_FALLSP);
            spCanRun = spCharLocomotion.FindPropertyRelative(PROP_CANRUN);
            spCanJump = spCharLocomotion.FindPropertyRelative(PROP_CANJMP);
            spJumpForce = spCharLocomotion.FindPropertyRelative(PROP_JMPFRC);
            spJumpCount = spCharLocomotion.FindPropertyRelative(PROP_JMPNUM);
            spTimeJumps = spCharLocomotion.FindPropertyRelative(PROP_JMPTIM);
            spPushForce = spCharLocomotion.FindPropertyRelative(PROP_PUSHFC);
            spSave = serializedObject.FindProperty(PROP_SAVEOB);

            spFaceDirection = spCharLocomotion.FindPropertyRelative(PROP_FACEDR);
            spFaceDirectionTarget = spCharLocomotion.FindPropertyRelative(PROP_FACEDT);
            spUseNavmesh = spCharLocomotion.FindPropertyRelative(PROP_NAVMES);
        }

        protected void OnDisable()
        {
            character = null;
        }

        // INSPECTOR GUI: ----------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            PaintInspector();

            EditorGUILayout.Space();
            GlobalEditorID.Paint(character);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        public void PaintInspector()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    string.Format("Is Busy: {0}", character.characterLocomotion.isBusy),
                    MessageType.None
                );
            }

            PaintCharacterBasicProperties();
            PaintCharacterAdvancedProperties();
            PaintAnimatorComponent();
        }

        private void PaintCharacterBasicProperties()
        {
            sectionProperties1.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionProperties1.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.LabelField("Locomotion:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spIsControllable);
                    EditorGUILayout.PropertyField(spCanRun);
                    EditorGUILayout.PropertyField(spRunSpeed);
                    EditorGUILayout.PropertyField(spAngularSpeed);
                    EditorGUILayout.PropertyField(spGravity);
                    EditorGUILayout.PropertyField(spMaxFallSpeed);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Traversal:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spCanJump);
                    EditorGUILayout.PropertyField(spJumpForce);
                    EditorGUILayout.PropertyField(spJumpCount);
                    EditorGUILayout.PropertyField(spTimeJumps);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintCharacterAdvancedProperties()
        {
            sectionProperties2.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionProperties2.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.LabelField("Locomotion:", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(spFaceDirection);
                    if (spFaceDirection.intValue == (int) CharacterLocomotion.FACE_DIRECTION.Target)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(spFaceDirectionTarget);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(spUseNavmesh);
                    EditorGUILayout.PropertyField(spPushForce);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Save/Load:", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(spSave);

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintAnimatorComponent()
        {
            if (!character.gameObject.GetComponent<CharacterAnimator>())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.HelpBox("You might want to add a Character Animator component", MessageType.Info);
                if (GUILayout.Button("Add Character Animator"))
                {
                    Undo.AddComponent<CharacterAnimator>(character.gameObject);
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}