using System.Collections;
using LowPolyHnS.Core;
using LowPolyHnS.Inventory;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    public class ActionCharacterGathering : IAction
    {
        private static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        public TargetCharacter Character = new TargetCharacter(TargetCharacter.Target.Player);
        public TargetPosition TargetPosition = new TargetPosition();

        private float duration;
        private bool wasControllable;
        private Character cacheCharacter;

        public AnimationClip clip;
        public float AnimationSpeed = 1f;
        private float fadeIn = 0.1f;
        private float fadeOut = 0.1f;
        private bool waitTillComplete = true;

        private CharacterAnimator characterAnimator;
        private bool forceStop;

        public LootObject LootObject;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            cacheCharacter = Character.GetCharacter(target);
            if (cacheCharacter == null)
            {
                return true;
            }

            wasControllable = cacheCharacter.IsControllable();
            cacheCharacter.characterLocomotion.SetIsControllable(false);

            Vector3 rotationDirection =
                TargetPosition.GetPosition(target) - cacheCharacter.gameObject.transform.position;

            rotationDirection = Vector3.Scale(rotationDirection, PLANE).normalized;
            duration = Vector3.Angle(cacheCharacter.transform.TransformDirection(Vector3.forward), rotationDirection) /
                       cacheCharacter.characterLocomotion.angularSpeed;

            cacheCharacter.characterLocomotion.SetRotation(rotationDirection);

            return !waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(duration);
            yield return waitForSeconds;

            if (cacheCharacter != null)
            {
                CharacterLocomotion locomotion = cacheCharacter.characterLocomotion;
                locomotion.SetIsControllable(wasControllable);


                forceStop = false;

                cacheCharacter.characterLocomotion.SetIsControllable(false);

                if (clip != null && cacheCharacter.GetCharacterAnimator() != null)
                {
                    characterAnimator = cacheCharacter.GetCharacterAnimator();
                    characterAnimator.CrossFadeGesture(
                        clip, AnimationSpeed, null,
                        fadeIn, fadeOut
                    );
                }

                if (clip != null && cacheCharacter != null && cacheCharacter.GetCharacterAnimator() != null)
                {
                    if (waitTillComplete)
                    {
                        float wait = Time.time + clip.length / AnimationSpeed;

                        WaitUntil waitUntil = new WaitUntil(() => forceStop || Time.time > wait);
                        yield return waitUntil;
                    }
                }

                if (LootObject != null)
                {
                    LootObject.LootResult loot = LootObject.Get();

                    if (loot.item != null && loot.amount > 0)
                    {
                        InventoryManager.Instance.AddItemToInventory(
                            loot.item.uuid,
                            loot.amount
                        );
                    }
                }

                cacheCharacter.characterLocomotion.SetIsControllable(true);
            }

            yield return 0;
        }


#if UNITY_EDITOR

        public new static string NAME = "Character/Gathering";
        private const string NODE_TITLE = "Gathering some {1}";

        public override string GetNodeTitle()
        {
            return LootObject != null ? string.Format(NODE_TITLE, "", LootObject.name) : NODE_TITLE;
        }

#endif
    }
}