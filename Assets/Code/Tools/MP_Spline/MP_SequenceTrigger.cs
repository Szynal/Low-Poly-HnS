using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MegaPixel.Tools.Spline {

	[System.Serializable]
	public struct MP_SequenceStruct {

		public MP_FollowSpline Follower;
		public float DelayBeforeNext;
		public bool PauseOnStarting;
	}

	[RequireComponent(typeof(BoxCollider))]
	public class MP_SequenceTrigger: MonoBehaviour {

		[Header("Settings")]
		public string TriggeringObjectTag = "Player";
		[SerializeField] bool drawGizmos = true;

		[Header("Sequence")]
		public List<MP_SequenceStruct> sequence = new List<MP_SequenceStruct>();

		Vector3 triggerSize = new Vector3(40f, 20f, 1f);

		private void OnTriggerEnter(Collider _other) {

			if (_other.tag == TriggeringObjectTag) {

				StartCoroutine(sequenceCoroutine());
			}
		}

		public void AddNewSequence() {

			sequence.Add(new MP_SequenceStruct());
		}

		IEnumerator sequenceCoroutine() {

			for (int i = 0; i < sequence.Count; i++) {

				MP_SequenceStruct _seq = sequence[i];

				if (_seq.Follower != null) {

					if (_seq.Follower.shouldBeInactiveBeforeStartOfMovement == false) {

						_seq.Follower.enabled = true;
						_seq.Follower.ShouldFollowSpline = true;

						#if UNITY_EDITOR

						if (_seq.PauseOnStarting == true) {

							EditorApplication.isPaused = true;
						}

						#endif

					} else {

						_seq.Follower.AppearAndMove();
					}
				}

				yield return new WaitForSeconds(_seq.DelayBeforeNext);
			}
		}

		void OnDrawGizmos() {

			if (drawGizmos == false) {

				return;
			}

			Gizmos.color = Color.blue;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(transform.position - new Vector3(transform.position.x, transform.position.y, transform.position.z), triggerSize);
		}
	}
}
