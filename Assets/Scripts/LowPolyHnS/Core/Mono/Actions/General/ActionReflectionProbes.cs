using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class ActionReflectionProbes : IAction
    {
        public ReflectionProbe reflectionProbe;
        public bool waitTillComplete = true;

        private int renderID;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (reflectionProbe == null) return true;

            renderID = reflectionProbe.RenderProbe();
            return !waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            while (!reflectionProbe.IsFinishedRendering(renderID))
            {
                yield return null;
            }

            yield return 0;
        }

#if UNITY_EDITOR

        public static new string NAME = "General/Render Reflection Probe";
        private const string NODE_TITLE = "Render probe {0} {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                reflectionProbe == null
                    ? "(none)"
                    : reflectionProbe.gameObject.name,
                waitTillComplete ? "(and wait)" : string.Empty
            );
        }

#endif
    }
}