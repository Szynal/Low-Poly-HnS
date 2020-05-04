using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterAnimatorEditor : Editor
    {
        private const string MSG_EMPTY_MODEL = "Drop a model from your project or load the default Character.";
        private const string PATH_DEFAULT_MODEL = "Assets/Plugins/LowPolyHnS/Characters/Models/Character.fbx";

        private const string PATH_DEFAULT_RCONT =
            "Assets/Plugins/LowPolyHnS/Characters/Animations/Controllers/Locomotion.controller";

        private const string MSG_PREFAB_INSTANCE_TITLE = "Cannot restructure Prefab instance";

        private const string MSG_PREFAB_INSTANCE_BODY = "You can open the Prefab in Prefab Mode " +
                                                        "to change the 3D model or unpack the Prefab instance to remove its Prefab connection.";

        private const string MSG_PREFAB_INSTANCE_OK = "Ok";
        private const string MSG_PREFAB_INSTANCE_OPEN = "Open Prefab";

        // PROPERTIES: ----------------------------------------------------------------------------

        private CharacterAnimator characterAnimator;
        private CharacterEditor.Section sectionModel;
        private CharacterEditor.Section sectionIK;
        private CharacterEditor.Section sectionRagdoll;

        private SerializedProperty spAnimator;
        private SerializedProperty spDefaultState;
        private bool isDraggingModel;

        private SerializedProperty spUseFootIK;
        private SerializedProperty spFootLayerMask;
        private SerializedProperty spUseHandIK;
        private SerializedProperty spUseSmartHeadIK;
        private SerializedProperty spUseProceduralLanding;

        private SerializedProperty spAutoInitRagdoll;
        private SerializedProperty spRagdollMass;
        private SerializedProperty spStableTimeout;
        private SerializedProperty spStandFaceUp;
        private SerializedProperty spStandFaceDown;
        private SerializedProperty spTimeScaleCoefficient;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected void OnEnable()
        {
            characterAnimator = (CharacterAnimator) target;

            string iconModelPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimModel.png");
            Texture2D iconModel = AssetDatabase.LoadAssetAtPath<Texture2D>(iconModelPath);
            sectionModel = new CharacterEditor.Section("Character Model", iconModel, Repaint);

            string iconIKPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimIK.png");
            Texture2D iconIK = AssetDatabase.LoadAssetAtPath<Texture2D>(iconIKPath);
            sectionIK = new CharacterEditor.Section("Inverse Kinematics", iconIK, Repaint);

            string iconRagdollPath = Path.Combine(CharacterEditor.CHARACTER_ICONS_PATH, "CharacterAnimRagdoll.png");
            Texture2D iconRagdoll = AssetDatabase.LoadAssetAtPath<Texture2D>(iconRagdollPath);
            sectionRagdoll = new CharacterEditor.Section("Ragdoll", iconRagdoll, Repaint);

            spAnimator = serializedObject.FindProperty("animator");
            spDefaultState = serializedObject.FindProperty("defaultState");

            spUseFootIK = serializedObject.FindProperty("useFootIK");
            spFootLayerMask = serializedObject.FindProperty("footLayerMask");
            spUseHandIK = serializedObject.FindProperty("useHandIK");
            spUseSmartHeadIK = serializedObject.FindProperty("useSmartHeadIK");
            spUseProceduralLanding = serializedObject.FindProperty("useProceduralLanding");

            spAutoInitRagdoll = serializedObject.FindProperty("autoInitializeRagdoll");
            spRagdollMass = serializedObject.FindProperty("ragdollMass");
            spStableTimeout = serializedObject.FindProperty("stableTimeout");
            spStandFaceUp = serializedObject.FindProperty("standFaceUp");
            spStandFaceDown = serializedObject.FindProperty("standFaceDown");

            spTimeScaleCoefficient = serializedObject.FindProperty("timeScaleCoefficient");
        }

        // INSPECTOR GUI: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            PaintAnimModel();
            PaintAnimIK();
            PaintAnimRagdoll();

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private void PaintAnimModel()
        {
            sectionModel.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionModel.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());

                    EditorGUILayout.PropertyField(spAnimator);
                    EditorGUILayout.PropertyField(spTimeScaleCoefficient);
                    EditorGUILayout.PropertyField(spDefaultState);

                    if (spAnimator.objectReferenceValue == null)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(MSG_EMPTY_MODEL, MessageType.Warning);
                        PaintChangeModel();
                    }
                    else
                    {
                        EditorGUILayout.Space();
                        PaintChangeModel();
                        if (((Animator) spAnimator.objectReferenceValue).applyRootMotion)
                        {
                            Animator reference = (Animator) spAnimator.objectReferenceValue;
                            reference.applyRootMotion = false;
                            spAnimator.objectReferenceValue = reference;

                            serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintAnimIK()
        {
            sectionIK.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionIK.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(spUseFootIK);
                    if (spUseFootIK.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(spFootLayerMask);
                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.PropertyField(spUseHandIK);
                    EditorGUILayout.PropertyField(spUseSmartHeadIK);
                    EditorGUILayout.PropertyField(spUseProceduralLanding);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private void PaintAnimRagdoll()
        {
            sectionRagdoll.PaintSection();
            using (var group = new EditorGUILayout.FadeGroupScope(sectionRagdoll.state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(spAutoInitRagdoll);
                    EditorGUILayout.PropertyField(spRagdollMass);
                    EditorGUILayout.PropertyField(spStableTimeout);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(spStandFaceUp);
                    EditorGUILayout.PropertyField(spStandFaceDown);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void PaintChangeModel()
        {
            Event evt = Event.current;
            Rect rect = GUILayoutUtility.GetRect(
                0.0f,
                50.0f,
                GUILayout.ExpandWidth(true)
            );

            float dropPadding = 2f;
            Rect dropRect = new Rect(
                rect.x + EditorGUIUtility.labelWidth + dropPadding,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth - 2f * dropPadding,
                rect.height
            );

            GUIStyle styleDropZone = isDraggingModel
                ? CoreGUIStyles.GetDropZoneActive()
                : CoreGUIStyles.GetDropZoneNormal();

            GUI.Box(dropRect, "Drop your 3D model", styleDropZone);

            Rect buttonRectA = GUILayoutUtility.GetRect(GUIContent.none, CoreGUIStyles.GetButtonLeft());
            buttonRectA = new Rect(
                buttonRectA.x + EditorGUIUtility.labelWidth,
                buttonRectA.y,
                buttonRectA.width / 2f - EditorGUIUtility.labelWidth / 2.0f,
                buttonRectA.height
            );

            Rect buttonRectB = new Rect(
                buttonRectA.x + buttonRectA.width,
                buttonRectA.y,
                buttonRectA.width - 2f,
                buttonRectA.height
            );

            if (GUI.Button(buttonRectA, "Default Character", CoreGUIStyles.GetButtonLeft()))
            {
                GameObject prefabDefault = AssetDatabase.LoadAssetAtPath<GameObject>(PATH_DEFAULT_MODEL);
                LoadCharacter(prefabDefault);
            }

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    isDraggingModel = false;
                    if (!dropRect.Contains(evt.mousePosition)) break;
                    if (DragAndDrop.objectReferences.Length != 1) break;

                    GameObject draggedObject = DragAndDrop.objectReferences[0] as GameObject;
                    if (draggedObject == null) break;

                    bool prefabAllowed = PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Model ||
                                         PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Regular ||
                                         PrefabUtility.GetPrefabAssetType(draggedObject) == PrefabAssetType.Variant;

                    if (!prefabAllowed) break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragUpdated)
                    {
                        isDraggingModel = true;
                    }
                    else if (evt.type == EventType.DragPerform)
                    {
                        isDraggingModel = false;

                        DragAndDrop.AcceptDrag();
                        LoadCharacter(draggedObject);
                    }

                    break;
            }
        }

        private Rect GetButtonRect()
        {
            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            return new Rect(
                buttonRect.x + EditorGUIUtility.labelWidth, buttonRect.y,
                buttonRect.width - EditorGUIUtility.labelWidth, buttonRect.height
            );
        }

        private void LoadCharacter(GameObject prefab)
        {
            if (prefab == null) return;
            if (prefab.GetComponentInChildren<Animator>() == null) return;
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(characterAnimator.gameObject))
            {
                bool enterPrefabMode = EditorUtility.DisplayDialog(
                    MSG_PREFAB_INSTANCE_TITLE,
                    MSG_PREFAB_INSTANCE_BODY,
                    MSG_PREFAB_INSTANCE_OPEN,
                    MSG_PREFAB_INSTANCE_OK
                );

                if (enterPrefabMode)
                {
                    string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                }

                return;
            }

            GameObject instance = Instantiate(prefab);
            instance.name = prefab.name;

            instance.transform.SetParent(characterAnimator.transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            Animator instanceAnimator = instance.GetComponentInChildren<Animator>();
            RuntimeAnimatorController rc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PATH_DEFAULT_RCONT);
            instanceAnimator.runtimeAnimatorController = rc;

            if (spAnimator.objectReferenceValue != null)
            {
                Animator previous = (Animator) spAnimator.objectReferenceValue;
                DestroyImmediate(previous.gameObject);
            }

            spAnimator.objectReferenceValue = instanceAnimator;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}