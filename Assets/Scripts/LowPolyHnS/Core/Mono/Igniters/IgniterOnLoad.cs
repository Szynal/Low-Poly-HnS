using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnLoad : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Load";
#endif

        private void Start()
        {
            SaveLoadManager.Instance.onLoad += OnLoad;
        }

        private void OnDestroy()
        {
            if (isExitingApplication) return;
            SaveLoadManager.Instance.onLoad -= OnLoad;
        }

        private void OnLoad(int profile)
        {
            ExecuteTrigger(gameObject);
        }
    }
}