using System;
using System.Collections.Generic;
using LowPolyHnS.Core.Hooks;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("LowPolyHnS/Trigger", 0)]
    public class Trigger : MonoBehaviour
    {
        private static readonly Color COLOR_WHITE_LIGHT = new Color(256, 256f, 256, 0.5f);

        public enum Platforms
        {
            Editor,
            Mobile,
            tvOS,
            Desktop,
            PS4,
            XBoxOne,
            WiiU,
            Switch
        }

        public enum ItemOpts
        {
            Conditions,
            Actions
        }

        [Serializable]
        public class Item
        {
            public ItemOpts option = ItemOpts.Actions;
            public Conditions conditions;
            public Actions actions;

            public Item(Actions actions)
            {
                option = ItemOpts.Actions;
                this.actions = actions;
            }

            public Item(Conditions conditions)
            {
                option = ItemOpts.Conditions;
                this.conditions = conditions;
            }
        }

        public const int ALL_PLATFORMS_KEY = -1;

        [Serializable]
        public class PlatformIgniters : SerializableDictionaryBase<int, Igniter>
        {
        }

        public class TriggerEvent : UnityEvent<GameObject>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public PlatformIgniters igniters = new PlatformIgniters();
        public List<Item> items = new List<Item>();

        // ADVANCED PROPERTIES: -------------------------------------------------------------------

        public bool minDistance = false;
        public float minDistanceToPlayer = 5.0f;

        // EVENTS: --------------------------------------------------------------------------------

        public TriggerEvent onExecute = new TriggerEvent();

        // INITIALIZE: ----------------------------------------------------------------------------

        private void Awake()
        {
            EventSystemManager.Instance.Wakeup();
            SetupPlatformIgniter();
        }

        private void SetupPlatformIgniter()
        {
            bool overridePlatform = false;

#if UNITY_STANDALONE
            if (!overridePlatform) overridePlatform = CheckPlatformIgniter(Platforms.Desktop);
#endif

#if UNITY_EDITOR
            if (!overridePlatform) overridePlatform = CheckPlatformIgniter(Platforms.Editor);
#endif


            if (!overridePlatform)
            {
                if (!igniters.ContainsKey(ALL_PLATFORMS_KEY))
                {
                    Igniter igniter = gameObject.AddComponent<IgniterStart>();
                    igniter.Setup(this);
                }

                igniters[ALL_PLATFORMS_KEY].enabled = true;
            }
        }

        private bool CheckPlatformIgniter(Platforms platform)
        {
            if (igniters.ContainsKey((int) platform))
            {
                igniters[(int) Platforms.Editor].enabled = true;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty spIgniters = serializedObject.FindProperty("igniters");
            SerializedProperty spValues = spIgniters.FindPropertyRelative("values");

            bool updated = false;
            int spValuesSize = spValues.arraySize;
            for (int i = 0; i < spValuesSize; ++i)
            {
                SerializedProperty spIgniter = spValues.GetArrayElementAtIndex(i);
                Igniter igniter = spIgniter.objectReferenceValue as Igniter;
                if (igniter != null && igniter.gameObject != gameObject)
                {
                    Igniter newIgniter = gameObject.AddComponent(igniter.GetType()) as Igniter;
                    EditorUtility.CopySerialized(igniter, newIgniter);
                    spIgniter.objectReferenceValue = newIgniter;
                    updated = true;
                }
            }

            if (updated) serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Execute(GameObject target, params object[] parameters)
        {
            if (minDistance && HookPlayer.Instance != null)
            {
                float distance = Vector3.Distance(HookPlayer.Instance.transform.position, transform.position);
                if (distance > minDistanceToPlayer) return;
            }

            if (onExecute != null) onExecute.Invoke(target);
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i] == null) continue;
                switch (items[i].option)
                {
                    case ItemOpts.Actions:
                        if (items[i].actions == null) continue;
                        Actions actionsReference = items[i].actions;
                        if (string.IsNullOrEmpty(actionsReference.gameObject.scene.name))
                        {
                            actionsReference = Instantiate(
                                actionsReference,
                                transform.position,
                                transform.rotation
                            );
                        }

                        actionsReference.Execute(target, parameters);
                        break;

                    case ItemOpts.Conditions:
                        if (items[i].conditions == null) continue;
                        Conditions conditionsReference = items[i].conditions;
                        if (string.IsNullOrEmpty(conditionsReference.gameObject.scene.name))
                        {
                            conditionsReference = Instantiate(
                                conditionsReference,
                                transform.position,
                                transform.rotation
                            );
                        }

                        conditionsReference.Interact(target, parameters);
                        break;
                }
            }
        }

        public virtual void Execute()
        {
            Execute(gameObject);
        }

        // GIZMO METHODS: -------------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            bool containsActions = false;
            bool containsConditions = false;

            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].option == ItemOpts.Actions) containsActions = true;
                if (items[i].option == ItemOpts.Conditions) containsConditions = true;
            }

            int state = 0;
            state |= containsConditions ? 0 : 1;
            state |= containsActions ? 0 : 2;

            switch (state)
            {
                case 0:
                    Gizmos.DrawIcon(transform.position, "Trigger/trigger0", true);
                    break;
                case 1:
                    Gizmos.DrawIcon(transform.position, "Trigger/trigger1", true);
                    break;
                case 2:
                    Gizmos.DrawIcon(transform.position, "Trigger/trigger2", true);
                    break;
                case 3:
                    Gizmos.DrawIcon(transform.position, "Trigger/trigger3", true);
                    break;
            }

            if (minDistance)
            {
                Gizmos.color = COLOR_WHITE_LIGHT;
                Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
            }
        }
    }
}