using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [CustomEditor(typeof(CharacterState), true)]
    public class CharacterStateEditor : Editor
    {
        private const string PATH_RTC_SIM =
            "Assets/Scripts/LowPolyHnS/Characters/Animations/Overriders/Simple.overrideController";

        private const string PATH_RTC_LOC =
            "Assets/Scripts/LowPolyHnS/Characters/Animations/Overriders/Locomotion.overrideController";

        private static readonly GUIContent GC_OTHER = new GUIContent("Animator");
        private static readonly GUIContent GC_AVATAR = new GUIContent("Avatar Mask (optional)");

        private const string MSG_SIM = "Set a State where the character uses one single animation";
        private const string MSG_LOC = "Use your own animations using the built-in transitions";
        private const string MSG_OTH = "Add your custom Animator";

        // PROPERTIES: ----------------------------------------------------------------------------

        private Editor editorAnimatorSim;
        private Editor editorAnimatorLoc;
        private Editor editorAnimatorOth;

        private SerializedProperty spType;
        private SerializedProperty spRtcSim;
        private SerializedProperty spRtcLoc;
        private SerializedProperty spRtcOth;

        private SerializedProperty spEnterClip;
        private SerializedProperty spEnterMask;
        private SerializedProperty spExitClip;
        private SerializedProperty spExitMask;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            spType = serializedObject.FindProperty("type");
            spRtcSim = serializedObject.FindProperty("rtcSimple");
            spRtcLoc = serializedObject.FindProperty("rtcLocomotion");
            spRtcOth = serializedObject.FindProperty("rtcOther");

            spEnterClip = serializedObject.FindProperty("enterClip");
            spEnterMask = serializedObject.FindProperty("enterAvatarMask");
            spExitClip = serializedObject.FindProperty("exitClip");
            spExitMask = serializedObject.FindProperty("exitAvatarMask");

            editorAnimatorSim = CreateEditor(spRtcSim.objectReferenceValue);
            editorAnimatorLoc = CreateEditor(spRtcLoc.objectReferenceValue);
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spType);

            switch (spType.intValue)
            {
                case (int) CharacterState.StateType.Simple:
                    EditorGUILayout.HelpBox(MSG_SIM, MessageType.Info);
                    editorAnimatorSim.OnInspectorGUI();
                    break;

                case (int) CharacterState.StateType.Locomotion:
                    EditorGUILayout.HelpBox(MSG_LOC, MessageType.Info);
                    editorAnimatorLoc.OnInspectorGUI();
                    break;

                case (int) CharacterState.StateType.Other:
                    EditorGUILayout.HelpBox(MSG_OTH, MessageType.Info);
                    EditorGUILayout.PropertyField(spRtcOth, GC_OTHER);
                    break;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spEnterClip);
            EditorGUILayout.PropertyField(spEnterMask, GC_AVATAR);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spExitClip);
            EditorGUILayout.PropertyField(spExitMask, GC_AVATAR);

            serializedObject.ApplyModifiedProperties();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // FACTORY METHODS: -----------------------------------------------------------------------

        [MenuItem("Assets/Create/LowPolyHnS/Characters/Simple State", false, 50)]
        public static void CreateCharacterState_Simple()
        {
            CharacterState state = CreateState("StateSimple.asset");
            state.type = CharacterState.StateType.Simple;
            state.name = "Simple Character State";
        }

        [MenuItem("Assets/Create/LowPolyHnS/Characters/Locomotion State", false, 50)]
        public static void CreateCharacterState_Locomotion()
        {
            CharacterState state = CreateState("StateLocomotion.asset");
            state.type = CharacterState.StateType.Locomotion;
            state.name = "Locomotion Character State";
        }

        [MenuItem("Assets/Create/LowPolyHnS/Characters/Advanced State", false, 50)]
        public static void CreateCharacterState_Advanced()
        {
            CharacterState state = CreateState("StateAdvanced.asset");
            state.type = CharacterState.StateType.Locomotion;
            state.name = "Advanced Character State";
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static CharacterState CreateState(string stateName)
        {
            CharacterState asset = CreateInstance<CharacterState>();

            AnimatorOverrideController rtcSim = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(PATH_RTC_SIM);
            AnimatorOverrideController rtcLoc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(PATH_RTC_LOC);

            AnimatorOverrideController assetRTCSim = Instantiate(rtcSim);
            AnimatorOverrideController assetRTCLoc = Instantiate(rtcLoc);

            assetRTCSim.name = "Simple";
            assetRTCLoc.name = "Locomotion";

            assetRTCSim.hideFlags = HideFlags.HideInHierarchy;
            assetRTCLoc.hideFlags = HideFlags.HideInHierarchy;

            asset.rtcSimple = assetRTCSim;
            asset.rtcLocomotion = assetRTCLoc;


            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }
    }
}