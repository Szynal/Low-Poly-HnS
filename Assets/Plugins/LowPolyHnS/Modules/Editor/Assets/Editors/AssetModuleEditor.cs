using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.ModuleManager
{
    [CustomEditor(typeof(AssetModule))]
    public class AssetModuleEditor : Editor
    {
        private const string TITLE = "{0} - {1}.{2}.{3}";
        private const string PASSWORD = "LowPolyHnS";

        private const string ADMIN_ICON_PATH = "Assets/Plugins/LowPolyHnS/Modules/Icons/UI/AdminLock.png";
        private const string MSG_DEV = "Restricted Area. LowPolyHnS employees only";
        private static GUIContent GUICONTENT_ADMIN;

        // PROPERTIES: ----------------------------------------------------------------------------

        private string password = "";
        private AssetModule assetModule;

        private SerializedProperty spAdminLogin;
        private SerializedProperty spAdminOpen;

        private SerializedProperty spModuleID;
        private SerializedProperty spVersionMajor;
        private SerializedProperty spVersionMinor;
        private SerializedProperty spVersionPatch;

        private SerializedProperty spDisplayName;
        private SerializedProperty spDescription;
        private SerializedProperty spCategory;

        private SerializedProperty spDependencies;
        private SerializedProperty spTags;

        private SerializedProperty spAuthorName;
        private SerializedProperty spAuthorMail;
        private SerializedProperty spAuthorSite;

        private SerializedProperty spIncludesData;
        private SerializedProperty spCodePaths;
        private SerializedProperty spDataPaths;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            assetModule = (AssetModule) target;

            spAdminLogin = serializedObject.FindProperty("adminLogin");
            spAdminOpen = serializedObject.FindProperty("adminOpen");

            SerializedProperty spModule = serializedObject.FindProperty("module");
            spModuleID = spModule.FindPropertyRelative("moduleID");

            SerializedProperty spVersion = spModule.FindPropertyRelative("version");
            spVersionMajor = spVersion.FindPropertyRelative("major");
            spVersionMinor = spVersion.FindPropertyRelative("minor");
            spVersionPatch = spVersion.FindPropertyRelative("patch");

            spDisplayName = spModule.FindPropertyRelative("displayName");
            spDescription = spModule.FindPropertyRelative("description");
            spCategory = spModule.FindPropertyRelative("category");

            spDependencies = spModule.FindPropertyRelative("dependencies");
            spTags = spModule.FindPropertyRelative("tags");

            spAuthorName = spModule.FindPropertyRelative("authorName");
            spAuthorMail = spModule.FindPropertyRelative("authorMail");
            spAuthorSite = spModule.FindPropertyRelative("authorSite");

            spIncludesData = spModule.FindPropertyRelative("includesData");
            spCodePaths = spModule.FindPropertyRelative("codePaths");
            spDataPaths = spModule.FindPropertyRelative("dataPaths");
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PaintInfo();
            PaintDeveloper();

            serializedObject.ApplyModifiedProperties();
        }

        private void PaintInfo()
        {
            string title = string.Format(
                TITLE,
                spModuleID.stringValue,
                spVersionMajor.intValue,
                spVersionMinor.intValue,
                spVersionPatch.intValue
            );

            EditorGUILayout.LabelField(spDisplayName.stringValue, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(title, EditorStyles.label);
        }

        private void PaintDeveloper()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle style = spAdminOpen.boolValue
                ? CoreGUIStyles.GetToggleButtonNormalOn()
                : CoreGUIStyles.GetToggleButtonNormalOff();

            if (GUICONTENT_ADMIN == null)
            {
                Texture2D adminTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ADMIN_ICON_PATH);
                GUICONTENT_ADMIN = new GUIContent(" Settings", adminTexture);
            }

            if (GUILayout.Button(GUICONTENT_ADMIN, style))
            {
                spAdminOpen.boolValue = !spAdminOpen.boolValue;
            }

            if (spAdminOpen.boolValue)
            {
                if (spAdminLogin.boolValue)
                {
                    PaintDeveloperAdmin();
                }
                else
                {
                    EditorGUILayout.HelpBox(MSG_DEV, MessageType.Warning);
                    EditorGUILayout.BeginHorizontal();
                    password = EditorGUILayout.PasswordField(password);
                    if (GUILayout.Button("Sign in", EditorStyles.miniButton))
                    {
                        if (password == PASSWORD) spAdminLogin.boolValue = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void PaintDeveloperAdmin()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Module", CoreGUIStyles.GetButtonLeft()))
            {
                EditorApplication.update += BuildModuleDeferred;
            }

            if (GUILayout.Button("Logout", CoreGUIStyles.GetButtonRight()))
            {
                spAdminLogin.boolValue = false;
                password = "";
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spModuleID);
            EditorGUILayout.PropertyField(spDisplayName);
            EditorGUILayout.PropertyField(spDescription);
            EditorGUILayout.PropertyField(spCategory);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spVersionMajor);
            EditorGUILayout.PropertyField(spVersionMinor);
            EditorGUILayout.PropertyField(spVersionPatch);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Author", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spAuthorName);
            EditorGUILayout.PropertyField(spAuthorMail);
            EditorGUILayout.PropertyField(spAuthorSite);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spDependencies, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spTags, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Paths", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spCodePaths, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data Paths", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(spIncludesData);
            EditorGUILayout.PropertyField(spDataPaths, true);
            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();
        }

        private void BuildModuleDeferred()
        {
            EditorApplication.update -= BuildModuleDeferred;
            assetModule.BuildModule();
        }
    }
}