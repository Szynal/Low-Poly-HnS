using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public abstract class Igniter : MonoBehaviour
    {
        private static readonly object[] NoObject = new object[0];

        [HideInInspector] [SerializeField] private Trigger trigger;
        protected bool IsExitingApplication;

        public virtual void Setup(Trigger trigger)
        {
            this.trigger = trigger;
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif
        }

        protected void OnEnable()
        {
#if UNITY_EDITOR
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif
        }

        protected void OnValidate()
        {
#if UNITY_EDITOR
            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif
        }


        #region PROTECTED METHODS

        protected virtual void ExecuteTrigger()
        {
            ExecuteTrigger(null);
        }

        protected virtual void ExecuteTrigger(GameObject target)
        {
            ExecuteTrigger(target, NoObject);
        }

        protected virtual void ExecuteTrigger(GameObject target, params object[] parameters)
        {
            if (target == null) target = gameObject;
            if (trigger == null) return;
            trigger.Execute(target, parameters);
        }

        protected virtual void OnApplicationQuit()
        {
            IsExitingApplication = true;
        }

        #endregion


        #region PROTECTED UTILITY METHODS

        protected bool IsColliderPlayer(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                return true;
            }

            return false;
        }

        #endregion

#if UNITY_EDITOR

        public static string Name = "Never";
        public static string Comment = "";
        public static bool RequiresCollider = false;
        public static string IconPath = "Assets/EditorIcons/Igniters/";

        public static bool PaintEditor(SerializedObject serialObject)
        {
            EditorGUI.BeginChangeCheck();
            if (serialObject.targetObject == null)
            {
                return EditorGUI.EndChangeCheck();
            }

            serialObject.Update();
            SerializedProperty iterator = serialObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if ("m_Script" == iterator.propertyPath) continue;
                EditorGUILayout.PropertyField(iterator, true);
            }

            serialObject.ApplyModifiedProperties();

            return EditorGUI.EndChangeCheck();
        }

#endif
    }
}