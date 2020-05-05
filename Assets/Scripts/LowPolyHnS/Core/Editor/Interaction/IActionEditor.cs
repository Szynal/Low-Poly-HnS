using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(IAction), true)]
    public class IActionEditor : Editor
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy;

        public SerializedProperty spActions;
        public IAction action;

        private SerializedProperty spAction;
        private Texture2D icon;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            action = (IAction) target;
            action.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            action.OnEnableEditor(action);

            Type actionType = action.GetType();
            string actionName = (string) actionType.GetField("NAME", BINDING_FLAGS).GetValue(null);

            FieldInfo customIconsFieldInfo = actionType.GetField(
                SelectTypePanel.CUSTOM_ICON_PATH_VARIABLE
            );

            string iconsPath = SelectTypePanel.ICONS_ACTIONS_PATH;
            if (customIconsFieldInfo != null)
            {
                string customIconsPath = (string) customIconsFieldInfo.GetValue(null);
                if (!string.IsNullOrEmpty(customIconsPath))
                {
                    iconsPath = customIconsPath;
                }
            }

            string actionIconPath = Path.Combine(
                iconsPath,
                SelectTypePanel.GetName(actionName) + ".png"
            );

            icon = AssetDatabase.LoadAssetAtPath<Texture2D>(actionIconPath);
            if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(iconsPath, "Default.png"));
            if (icon == null) icon = EditorGUIUtility.FindTexture("GameObject Icon");
        }

        private void OnDisable()
        {
            if (action == null) return;
            action.OnDisableEditor();
        }

        public void Setup(SerializedProperty spActions, int index)
        {
            action = (IAction) target;
            action.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            action.OnEnableEditor(target);
        }

        public Texture2D GetIcon()
        {
            return icon;
        }

        // INSPECTOR: -----------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null) return;
            action.OnInspectorGUIEditor();
        }
    }
}