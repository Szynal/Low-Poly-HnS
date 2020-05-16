using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [CustomEditor(typeof(PlayerCharacter))]
    public class PlayerCharacterEditor : CharacterEditor
    {
        private const string SECTION_INPUT = "Player Input";
        private const string SECTION_ATTRIBUTE = "Player Attribute";

        private const string PROP_INPUTT = "inputType";
        private const string PROP_MOUSEB = "mouseButtonMove";
        private const string PROP_LAYERM = "mouseLayerMask";
        private const string PROP_INVERT = "invertAxis";
        private const string PROP_INPUT_JMP = "jumpKey";

        private const string PROP_USE_ACC = "useAcceleration";
        private const string PROP_ACC = "acceleration";
        private const string PROP_DEC = "deceleration";

        private const string PROP_CLICK = "RippleClickEffect";

        private const string PROP_STRENGTH = "Strength";
        private const string PROP_AGILITY = "Agility";
        private const string PROP_INTELLIGENCE = "Intelligence";

        // PROPERTIES: ----------------------------------------------------------------------------

        private Section sectionInput;
        private SerializedProperty spInputType;
        private SerializedProperty spMouseButtonMove;
        private SerializedProperty spMouseLayerMask;
        private SerializedProperty spInvertAxis;
        private SerializedProperty spInputJump;

        private SerializedProperty spUseAcceleration;
        private SerializedProperty spAcceleration;
        private SerializedProperty spDeceleration;

        private SerializedProperty spRippleClickEffect;
        
        private Section sectionAttribute;
        private SerializedProperty spStrength;
        private SerializedProperty spAgility;
        private SerializedProperty spIntelligence;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected new void OnEnable()
        {
            base.OnEnable();

            string iconInputPath = Path.Combine(CHARACTER_ICONS_PATH, "PlayerInput.png");
            Texture2D iconInput = AssetDatabase.LoadAssetAtPath<Texture2D>(iconInputPath);
            sectionInput = new Section(SECTION_INPUT, iconInput, Repaint);
            sectionAttribute = new Section(SECTION_ATTRIBUTE, null, Repaint);

            spInputType = serializedObject.FindProperty(PROP_INPUTT);
            spMouseButtonMove = serializedObject.FindProperty(PROP_MOUSEB);
            spMouseLayerMask = serializedObject.FindProperty(PROP_LAYERM);
            spInvertAxis = serializedObject.FindProperty(PROP_INVERT);
            spInputJump = serializedObject.FindProperty(PROP_INPUT_JMP);

            spUseAcceleration = serializedObject.FindProperty(PROP_USE_ACC);
            spAcceleration = serializedObject.FindProperty(PROP_ACC);
            spDeceleration = serializedObject.FindProperty(PROP_DEC);

            spStrength = serializedObject.FindProperty(PROP_STRENGTH);
            spAgility = serializedObject.FindProperty(PROP_AGILITY);
            spIntelligence = serializedObject.FindProperty(PROP_INTELLIGENCE);

            spRippleClickEffect = serializedObject.FindProperty(PROP_CLICK);

            if (spMouseLayerMask.intValue == 0)
            {
                spMouseLayerMask.intValue = ~0;
            }
        }

        protected new void OnDisable()
        {
            base.OnDisable();
        }

        // INSPECTOR GUI: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            PaintInspector();
            sectionInput.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionInput.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(spInputType);
                    EditorGUI.indentLevel++;

                    if (spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.PointAndClick ||
                        spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.FollowPointer ||
                        spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.FollowAndClickPointer)
                    {
                        EditorGUILayout.PropertyField(spMouseButtonMove);
                    }

                    if (spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.PointAndClick)
                    {
                        EditorGUILayout.PropertyField(spMouseLayerMask);
                        if (spMouseLayerMask.intValue == 0)
                        {
                            spMouseLayerMask.intValue = ~0;
                        }

                        EditorGUILayout.PropertyField(spRippleClickEffect);
                    }

                    if (spInputType.intValue == (int) PlayerCharacter.INPUT_TYPE.FollowAndClickPointer)
                    {
                        EditorGUILayout.PropertyField(spMouseLayerMask);
                        if (spMouseLayerMask.intValue == 0)
                        {
                            spMouseLayerMask.intValue = ~0;
                        }

                        EditorGUILayout.PropertyField(spRippleClickEffect);
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(spInputJump);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(spUseAcceleration);
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(!spUseAcceleration.boolValue);

                    EditorGUILayout.PropertyField(spAcceleration);
                    EditorGUILayout.PropertyField(spDeceleration);

                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndVertical();
                }
            }

            PaintCharacterAttributeProperties();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }


        private void PaintCharacterAttributeProperties()
        {
            sectionAttribute.PaintSection();
            using (EditorGUILayout.FadeGroupScope group = new EditorGUILayout.FadeGroupScope(sectionAttribute.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.LabelField("Attribute:", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spStrength);


                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spAgility);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spIntelligence);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndVertical();
                }
            }
        }
    }
}