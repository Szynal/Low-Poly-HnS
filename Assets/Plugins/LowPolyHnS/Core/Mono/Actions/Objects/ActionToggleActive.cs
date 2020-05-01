namespace LowPolyHnS.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using LowPolyHnS.Core;
    using LowPolyHnS.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionToggleActive : IAction 
	{
        public TargetGameObject target = new TargetGameObject();

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            GameObject targetValue = this.target.GetGameObject(target);
            if (targetValue != null) targetValue.SetActive(!targetValue.activeSelf);

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Object/Toggle Active";
		private const string NODE_TITLE = "Toggle object {1} active state";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(
				NODE_TITLE, 
				this.target
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
		}

		protected override void OnDisableEditorChild ()
		{
			this.spTarget = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spTarget);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}