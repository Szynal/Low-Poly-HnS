using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomEditor(typeof(Hotspot))]
    public class HotspotEditor : Editor
    {
        private const string MSG_REQUIRE_COLLIDER = "A Collider component is required.";
        private const float ANIM_BOOL_SPEED = 3.0f;
        private const string PROP_CURSOR = "cursorData";
        private const string PROP_PROXIM = "proximityData";
        private const string PROP_HEADTR = "headTrackData";
        private const string PROP_ENABLED = "enabled";

        private const string PROP_PROXIM_RADIUS = "radius";
        private const string PROP_HEADTR_RADIUS = "radius";

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private Hotspot hotspot;
        private SerializedProperty spCursor;
        private SerializedProperty spCursorEnabled;
        private AnimBool cursorState;
        private SerializedProperty spProxim;
        private SerializedProperty spProximEnabled;
        private AnimBool proximState;
        private SerializedProperty spHeadTr;
        private SerializedProperty spHeadTrEnabled;
        private AnimBool headTrState;

        private SerializedProperty spProximRadius;
        private SerializedProperty spHeadTrRadius;

        // INITIALIZE: -------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            hotspot = (Hotspot) target;

            spCursor = serializedObject.FindProperty(PROP_CURSOR);
            spProxim = serializedObject.FindProperty(PROP_PROXIM);
            spHeadTr = serializedObject.FindProperty(PROP_HEADTR);

            spCursorEnabled = spCursor.FindPropertyRelative(PROP_ENABLED);
            spProximEnabled = spProxim.FindPropertyRelative(PROP_ENABLED);
            spHeadTrEnabled = spHeadTr.FindPropertyRelative(PROP_ENABLED);

            spProximRadius = spProxim.FindPropertyRelative(PROP_PROXIM_RADIUS);
            spHeadTrRadius = spHeadTr.FindPropertyRelative(PROP_HEADTR_RADIUS);

            SetupAnimBool(ref cursorState, spCursorEnabled.boolValue);
            SetupAnimBool(ref proximState, spProximEnabled.boolValue);
            SetupAnimBool(ref headTrState, spHeadTrEnabled.boolValue);
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            PaintItem(spCursor, spCursorEnabled, cursorState, "Cursor", true);
            PaintItem(spProxim, spProximEnabled, proximState, "Proximity Hint", false);
            PaintItem(spHeadTr, spHeadTrEnabled, headTrState, "Head Look At", false);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        private void PaintItem(SerializedProperty property, SerializedProperty enabled, AnimBool state,
            string title, bool requiresCollider)
        {
            PaintHeader(enabled, state, title);
            PaintContent(property, state, requiresCollider);
        }

        private void PaintHeader(SerializedProperty spEnabled, AnimBool state, string title)
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle style = spEnabled.boolValue
                ? CoreGUIStyles.GetToggleButtonOn()
                : CoreGUIStyles.GetToggleButtonOff();

            bool buttonPressed = GUILayout.Button(title, style);
            Rect buttonRect = GUILayoutUtility.GetLastRect();

            if (buttonPressed)
            {
                spEnabled.boolValue = !spEnabled.boolValue;
                state.target = spEnabled.boolValue;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Rect toggleRect = new Rect(
                    buttonRect.x + 5f,
                    buttonRect.y + (buttonRect.height / 2 - 8),
                    12,
                    12
                );

                bool isOn = spEnabled.boolValue;
                EditorStyles.toggle.Draw(toggleRect, GUIContent.none, false, false, isOn, isOn);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void PaintContent(SerializedProperty property, AnimBool state, bool requiresCollider)
        {
            using (var group = new EditorGUILayout.FadeGroupScope(state.faded))
            {
                if (group.visible)
                {
                    EditorGUILayout.BeginVertical(CoreGUIStyles.GetBoxExpanded());
                    EditorGUILayout.PropertyField(property);
                    if (requiresCollider && hotspot.gameObject.GetComponent<Collider>() == null)
                    {
                        EditorGUILayout.HelpBox(MSG_REQUIRE_COLLIDER, MessageType.Warning);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }

        // SCENE UI: ---------------------------------------------------------------------------------------------------

        private void OnSceneGUI()
        {
            if (spProximEnabled.boolValue)
            {
                serializedObject.Update();

                Handles.color = Color.white;
                spProximRadius.floatValue = Handles.RadiusHandle(
                    Quaternion.identity,
                    hotspot.transform.position,
                    spProximRadius.floatValue
                );

                spProximRadius.floatValue = Mathf.Clamp(spProximRadius.floatValue, 0.0f, 20.0f);

                serializedObject.ApplyModifiedProperties();
            }

            if (spHeadTrEnabled.boolValue)
            {
                serializedObject.Update();

                Handles.color = Color.cyan;
                spHeadTrRadius.floatValue = Handles.RadiusHandle(
                    Quaternion.identity,
                    hotspot.transform.position,
                    spHeadTrRadius.floatValue
                );

                spHeadTrRadius.floatValue = Mathf.Clamp(spHeadTrRadius.floatValue, 0.0f, 20.0f);

                serializedObject.ApplyModifiedProperties();
            }
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private void SetupAnimBool(ref AnimBool animBool, bool state)
        {
            animBool = new AnimBool(state, () => { Repaint(); });
            animBool.speed = ANIM_BOOL_SPEED;
        }

        // HIERARCHY CONTEXT MENU: -------------------------------------------------------------------------------------

        [MenuItem("GameObject/LowPolyHnS/Other/Hotspot", false, 0)]
        public static void CreateHotspot()
        {
            GameObject trigger = CreateSceneObject.Create("Hotspot");
            SphereCollider collider = trigger.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            trigger.AddComponent<Hotspot>();
        }
    }
}