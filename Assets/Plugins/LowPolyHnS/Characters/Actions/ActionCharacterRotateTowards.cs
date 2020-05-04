using System.Collections;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class ActionCharacterRotateTowards : IAction
    {
        private static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        public TargetCharacter character = new TargetCharacter();
        public TargetPosition target = new TargetPosition();

        private float duration;
        private bool wasControllable;
        private Character cacheCharacter;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            cacheCharacter = character.GetCharacter(target);
            if (cacheCharacter == null) return true;

            wasControllable = cacheCharacter.IsControllable();
            cacheCharacter.characterLocomotion.SetIsControllable(false);

            Vector3 rotationDirection = this.target.GetPosition(target) - cacheCharacter.gameObject.transform.position;

            rotationDirection = Vector3.Scale(rotationDirection, PLANE).normalized;
            duration = Vector3.Angle(
                           cacheCharacter.transform.TransformDirection(Vector3.forward),
                           rotationDirection
                       ) / cacheCharacter.characterLocomotion.angularSpeed;

            cacheCharacter.characterLocomotion.SetRotation(rotationDirection);

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            WaitForSeconds wait = new WaitForSeconds(duration);
            yield return wait;

            if (cacheCharacter != null)
            {
                CharacterLocomotion locomotion = cacheCharacter.characterLocomotion;
                locomotion.SetIsControllable(wasControllable);
            }

            yield return 0;
        }

#if UNITY_EDITOR

        public static new string NAME = "Character/Character Rotate Towards";
        private const string NODE_TITLE = "Rotate {0} towards {1}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, character, target);
        }

#endif
    }
}