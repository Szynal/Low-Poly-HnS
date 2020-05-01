using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class IConditionsList : MonoBehaviour
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public ICondition[] conditions = new ICondition[0];

        // CONSTRUCTORS: --------------------------------------------------------------------------

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spConditions = null;
            for (int i = 0; i < conditions.Length; ++i)
            {
                ICondition condition = conditions[i];
                if (condition != null && condition.gameObject != gameObject)
                {
                    ICondition newCondition = gameObject.AddComponent(condition.GetType()) as ICondition;
                    EditorUtility.CopySerialized(condition, newCondition);

                    if (spConditions == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spConditions = serializedObject.FindProperty("conditions");
                    }

                    spConditions.GetArrayElementAtIndex(i).objectReferenceValue = newCondition;
                }
            }

            if (spConditions != null) spConditions.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Check(GameObject invoker = null, params object[] parameters)
        {
            if (conditions == null) return true;

            for (int i = 0; i < conditions.Length; ++i)
            {
                if (conditions[i] == null) continue;
                if (!conditions[i].Check(invoker, parameters)) return false;
            }

            return true;
        }
    }
}