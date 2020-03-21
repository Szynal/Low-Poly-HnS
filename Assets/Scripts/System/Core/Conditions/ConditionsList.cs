using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class ConditionsList : MonoBehaviour
    {
        public Condition[] Conditions = new Condition[0];


#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spConditions = null;
            for (int i = 0; i < Conditions.Length; ++i)
            {
                Condition condition = Conditions[i];
                if (condition == null || condition.gameObject == gameObject)
                {
                    continue;
                }

                Condition newCondition = gameObject.AddComponent(condition.GetType()) as Condition;
                EditorUtility.CopySerialized(condition, newCondition);

                if (spConditions == null)
                {
                    SerializedObject serializedObject = new SerializedObject(this);
                    spConditions = serializedObject.FindProperty("conditions");
                }

                spConditions.GetArrayElementAtIndex(i).objectReferenceValue = newCondition;
            }

            spConditions?.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif


        #region PUBLIC METHODS

        public bool Check(GameObject invoker = null, params object[] parameters)
        {
            if (Conditions == null)
            {
                return true;
            }

            return Conditions.Where(condition => condition != null)
                .All(condition => condition.Check(invoker, parameters));
        }

        #endregion
    }
}