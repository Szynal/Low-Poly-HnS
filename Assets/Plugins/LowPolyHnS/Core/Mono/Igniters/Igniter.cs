using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("")]
    public abstract class Igniter : MonoBehaviour
    {
        private static readonly object[] NO_OBJECT = new object[0];

        // PROPERTIES: ----------------------------------------------------------------------------

        [HideInInspector] [SerializeField] private Trigger trigger;
        protected bool isExitingApplication;

        // INITIALIZER: ---------------------------------------------------------------------------

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

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void ExecuteTrigger()
        {
            ExecuteTrigger(null);
        }

        protected virtual void ExecuteTrigger(GameObject target)
        {
            ExecuteTrigger(target, NO_OBJECT);
        }

        protected virtual void ExecuteTrigger(GameObject target, params object[] parameters)
        {
            if (target == null) target = gameObject;
            if (trigger == null) return;
            trigger.Execute(target, parameters);
        }

        protected virtual void OnApplicationQuit()
        {
            isExitingApplication = true;
        }

        // PROTECTED UTILITY METHODS: -------------------------------------------------------------

        protected bool IsColliderPlayer(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null &&
                HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                return true;
            }

            return false;
        }

        // VIRTUAL EDITOR METHODS: ----------------------------------------------------------------

#if UNITY_EDITOR

        public static string NAME = "Never";
        public static string COMMENT = "";
        public static bool REQUIRES_COLLIDER = false;
        public static string ICON_PATH = "Assets/EditorIcons/Igniters/";

        public static bool PaintEditor(SerializedObject serialObject)
        {
            EditorGUI.BeginChangeCheck();
            if (serialObject.targetObject != null)
            {
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
            }

            return EditorGUI.EndChangeCheck();
        }

#endif
    }
}