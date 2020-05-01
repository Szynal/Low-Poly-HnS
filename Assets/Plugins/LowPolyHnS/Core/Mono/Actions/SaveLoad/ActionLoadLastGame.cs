using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("")]
    public class ActionLoadLastGame : IAction
    {
        private bool complete;

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            SaveLoadManager.Instance.LoadLast(OnLoad);
            complete = false;

            WaitUntil waitUntil = new WaitUntil(() => complete);
            yield return waitUntil;

            yield return 0;
        }

        private void OnLoad()
        {
            complete = true;
        }

#if UNITY_EDITOR
        public static new string NAME = "Save & Load/Load Last Game";
        private const string NODE_TITLE = "Load Last Game";
        private const string MSG = "Loads the last saved game";

        public override string GetNodeTitle()
        {
            return NODE_TITLE;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(MSG, MessageType.Info);
        }
#endif
    }
}