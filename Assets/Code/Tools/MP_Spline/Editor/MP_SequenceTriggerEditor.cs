using UnityEngine;
using UnityEditor;

namespace MegaPixel.Tools.Spline {

	[CustomEditor(typeof(MP_SequenceTrigger))]
	public class MP_SequenceTriggerEditor: Editor {

		public override void OnInspectorGUI() {

			DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Helper", EditorStyles.boldLabel);

			if (GUILayout.Button("Add New Sequence")) {

				MP_SequenceTrigger _trigger = (MP_SequenceTrigger) target;
				_trigger.AddNewSequence();
			}
		}
	}
}