using UnityEngine;

namespace LowPolyHnS.Core
{
    [AddComponentMenu("")]
    public class IgniterOnSave : Igniter
    {
#if UNITY_EDITOR
        public new static string NAME = "General/On Save";
#endif

        private void Start()
        {
            SaveLoadManager.Instance.onSave += OnSave;
        }

        private void OnDestroy()
        {
            if (isExitingApplication) return;
            SaveLoadManager.Instance.onSave -= OnSave;
        }

        private void OnSave(int profile)
        {
            ExecuteTrigger(gameObject);
        }
    }
}