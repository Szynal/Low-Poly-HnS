using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(Clause))]
    public class ClauseEditor : Editor
    {
        private const string MSG_EMTPY_CONDITIONS = "There are no Conditions. Create one pressing the button below.";
        private const string MSG_REMOVE_TITLE = "Are you sure you want to delete this Condition?";
        private const string MSG_REMOVE_DESCR = "This operation can't be undone.";

        private const string MSG_RM_ACTIONS_TITLE =
            "Do you want to remove the Actions gameObject attached to this Action?";

        private const string MSG_RM_ACTIONS_DESCR =
            "You can either remove the Action keeping the Actions or delete both.";

        private const string MSG_UNDEF_COND_1 = "This Condition is not set as an instance of an object.";
        private const string MSG_UNDEF_COND_2 = "Check if you disabled or uninstalled a module that defined it.";

        public const string PROP_DESCRIPTION = "description";
        public const string PROP_CONDITIONSLIST = "conditionsList";
        public const string PROP_ACTION = "actions";

        public SerializedProperty spClauses;

        public Clause instance;
        public Conditions parentConditions;
        public SerializedProperty spDescription;
        public SerializedProperty spConditionsList;
        public SerializedProperty spActions;

        private ActionsEditor actionsEditor;
        private IConditionsListEditor conditionsListEditor;
        public Rect handleDragRect;
        public Rect clauseRect;

        // INITIALIZERS: -----------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            instance = (Clause) target;
            instance.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            handleDragRect = Rect.zero;
            clauseRect = Rect.zero;

            spDescription = serializedObject.FindProperty(PROP_DESCRIPTION);
            spConditionsList = serializedObject.FindProperty(PROP_CONDITIONSLIST);
            spActions = serializedObject.FindProperty(PROP_ACTION);

            if (instance.actions != null)
            {
                actionsEditor = CreateEditor(instance.actions) as ActionsEditor;
            }

            if (instance.conditionsList == null)
            {
                IConditionsList cList = instance.gameObject.AddComponent<IConditionsList>();
                spConditionsList.objectReferenceValue = cList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            conditionsListEditor = CreateEditor(instance.conditionsList) as IConditionsListEditor;
        }

        public void OnDestroyClause()
        {
            instance = (Clause) target;

            if (conditionsListEditor != null)
            {
                conditionsListEditor.OnDestroyConditionsList();
                DestroyImmediate(instance.conditionsList, true);
            }

            if (instance.actions != null)
            {
                int actionsID = instance.actions.gameObject.GetInstanceID();
                if (actionsID == parentConditions.gameObject.GetInstanceID()) return;

                if (EditorUtility.DisplayDialog(MSG_RM_ACTIONS_TITLE, MSG_RM_ACTIONS_DESCR, "Delete", "Keep"))
                {
                    DestroyImmediate(instance.actions.gameObject, true);
                }
            }
        }

        // INSPECTOR: --------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Can't edit a Clause from here", MessageType.Warning);
        }

        public void OnClauseGUI()
        {
            if (target == null || serializedObject == null) return;
            serializedObject.Update();

            EditorGUILayout.PropertyField(spDescription);

            GUIContent gcIf = ClausesUtilities.Get(ClausesUtilities.Icon.If);
            Rect rectIf = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectIf, gcIf, EditorStyles.boldLabel);

            conditionsListEditor.OnInspectorGUI();
            EditorGUILayout.Space();

            GUIContent gcThen = ClausesUtilities.Get(ClausesUtilities.Icon.Then);
            Rect rectThen = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
            EditorGUI.LabelField(rectThen, gcThen, EditorStyles.boldLabel);

            ActionsEditor.Return returnActions = ActionsEditor.PaintActionsGUI(
                parentConditions.gameObject,
                spActions,
                actionsEditor
            );

            if (returnActions != null)
            {
                spActions = returnActions.spParentActions;
                actionsEditor = returnActions.parentActionsEditor;

                if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}