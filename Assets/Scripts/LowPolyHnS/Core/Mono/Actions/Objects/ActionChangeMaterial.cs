using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Serialization;

namespace LowPolyHnS.Core
{
#if UNITY_EDITOR

#endif

    [AddComponentMenu("")]
    public class ActionChangeMaterial : IAction
    {
        public Material material;

        [FormerlySerializedAs("target")] [Space]
        public TargetGameObject targetRenderer = new TargetGameObject();

        [Space] public NumberProperty materialIndex = new NumberProperty(0);

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject go = targetRenderer.GetGameObject(target);
            if (go != null)
            {
                Renderer render = go.GetComponent<Renderer>();
                int matIndex = materialIndex.GetInt(target);

                if (render != null)
                {
                    if (matIndex >= 0 && matIndex < render.materials.Length)
                    {
                        Material[] materials = render.materials;
                        materials[matIndex] = material;

                        render.materials = materials;
                    }
                }
            }

            return true;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

#if UNITY_EDITOR

        public static new string NAME = "Object/Change Material";
        private const string NODE_TITLE = "Change material {0}";

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, material == null
                ? "(none)"
                : material.name);
        }

#endif
    }
}