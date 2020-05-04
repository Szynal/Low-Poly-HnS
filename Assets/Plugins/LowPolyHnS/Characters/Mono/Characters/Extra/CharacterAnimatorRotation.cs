using System;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [Serializable]
    public class CharacterAnimatorRotation
    {
        private const float ROTATION_SMOOTH = 0.1f;

        [Serializable]
        private class AnimFloat
        {
            public float target { get; private set; }
            public float value { get; private set; }
            private float time;

            public AnimFloat(float value)
            {
                target = value;
                this.value = value;
                time = 0f;
            }

            public float Update()
            {
                float t = (Time.time - time) / ROTATION_SMOOTH;
                value = Mathf.LerpAngle(value, target, t);

                return value;
            }

            public void SetTarget(float value)
            {
                target = value;
                time = Time.time;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private readonly AnimFloat x = new AnimFloat(0f);
        private readonly AnimFloat y = new AnimFloat(0f);
        private readonly AnimFloat z = new AnimFloat(0f);

        // UPDATER: -------------------------------------------------------------------------------

        public Quaternion Update()
        {
            Quaternion rotation = Quaternion.Euler(
                x.Update(),
                y.Update(),
                z.Update()
            );

            return rotation;
        }

        // PUBLIC GETTERS: ------------------------------------------------------------------------

        public Quaternion GetCurrentRotation()
        {
            return Quaternion.Euler(x.value, y.value, z.value);
        }

        public Quaternion GetTargetRotation()
        {
            return Quaternion.Euler(x.target, y.target, z.target);
        }

        // PUBLIC SETTERS: ------------------------------------------------------------------------

        public void SetPitch(float value)
        {
            x.SetTarget(value);
        }

        public void SetYaw(float value)
        {
            y.SetTarget(value);
        }

        public void SetRoll(float value)
        {
            z.SetTarget(value);
        }

        public void SetQuaternion(Quaternion rotation)
        {
            SetPitch(rotation.eulerAngles.x);
            SetYaw(rotation.eulerAngles.y);
            SetRoll(rotation.eulerAngles.z);
        }
    }
}