using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionHeadTrack : IAction
    {
        public enum TRACK_STATE
        {
            TrackTarget,
            Untrack
        }

        public TargetCharacter character = new TargetCharacter();
        public TRACK_STATE trackState = TRACK_STATE.TrackTarget;
        public TargetGameObject trackTarget = new TargetGameObject();
        public float speed = 0.5f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Character sourceCharacter = character.GetCharacter(target);
            if (sourceCharacter != null)
            {
                CharacterHeadTrack headTrack = sourceCharacter.GetHeadTracker();
                if (headTrack != null)
                {
                    switch (trackState)
                    {
                        case TRACK_STATE.TrackTarget:
                            headTrack.Track(trackTarget.GetTransform(target), speed);
                            break;

                        case TRACK_STATE.Untrack:
                            headTrack.Untrack();
                            break;
                    }
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Character/Head Track";
        private const string NODE_TITLE = "{0} {1} {2}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spTrackState;
        private SerializedProperty spTrackTarget;
        private SerializedProperty spSpeed;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            string source = character.ToString();

            string target = trackTarget.ToString();
            if (trackState == TRACK_STATE.Untrack) target = "";

            string track = "head ";
            if (trackState == TRACK_STATE.TrackTarget) track += "track";
            if (trackState == TRACK_STATE.Untrack) track += "untrack";


            return string.Format(
                NODE_TITLE,
                source,
                track,
                target
            );
        }

        protected override void OnEnableEditorChild()
        {
            spCharacter = serializedObject.FindProperty("character");
            spTrackState = serializedObject.FindProperty("trackState");
            spTrackTarget = serializedObject.FindProperty("trackTarget");
            spSpeed = serializedObject.FindProperty("speed");
        }

        protected override void OnDisableEditorChild()
        {
            spCharacter = null;
            spTrackState = null;
            spTrackTarget = null;
            spSpeed = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(spCharacter);
            EditorGUILayout.PropertyField(spTrackState);
            EditorGUILayout.PropertyField(spSpeed);

            if (spTrackState.intValue == (int) TRACK_STATE.TrackTarget)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spTrackTarget);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}