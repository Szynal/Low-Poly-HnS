using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("")]
    public class CharacterAttachments : MonoBehaviour
    {
        [Serializable]
        public class EventData
        {
            public GameObject attachment;
            public HumanBodyBones bone;
            public bool isDestroy;
        }

        [Serializable]
        public class AttachmentEvent : UnityEvent<EventData>
        {
        }

        [Serializable]
        public class Attachment
        {
            public GameObject prefab;
            public GameObject instance;
            public Vector3 locPosition = Vector3.zero;
            public Quaternion locRotation = Quaternion.identity;

            public Attachment(GameObject instance, GameObject prefab = null)
            {
                this.prefab = prefab;
                this.instance = instance;
                locPosition = instance.transform.localPosition;
                locRotation = instance.transform.localRotation;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Animator animator;
        public Dictionary<HumanBodyBones, List<Attachment>> attachments { get; private set; }

        public AttachmentEvent onAttach = new AttachmentEvent();
        public AttachmentEvent onDetach = new AttachmentEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public void Setup(Animator animator)
        {
            this.animator = animator;
            attachments = new Dictionary<HumanBodyBones, List<Attachment>>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Attach(HumanBodyBones bone, GameObject prefab, Vector3 position, Quaternion rotation,
            Space space = Space.Self)
        {
            if (!attachments.ContainsKey(bone)) attachments.Add(bone, new List<Attachment>());

            GameObject instance = prefab;
            if (string.IsNullOrEmpty(prefab.scene.name))
            {
                instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            instance.transform.SetParent(animator.GetBoneTransform(bone));

            switch (space)
            {
                case Space.Self:
                    instance.transform.localPosition = position;
                    instance.transform.localRotation = rotation;
                    break;

                case Space.World:
                    instance.transform.position = position;
                    instance.transform.rotation = rotation;
                    break;
            }

            attachments[bone].Add(new Attachment(instance, prefab));

            if (onAttach != null)
            {
                onAttach.Invoke(new EventData
                {
                    attachment = instance,
                    bone = bone
                });
            }
        }

        public List<GameObject> Detach(HumanBodyBones bone)
        {
            return DetachOrDestroy(bone, false);
        }

        public bool Detach(GameObject instance)
        {
            return DetachOrDestroy(instance, false);
        }

        public void Remove(HumanBodyBones bone)
        {
            DetachOrDestroy(bone, true);
        }

        public void Remove(GameObject instance)
        {
            DetachOrDestroy(instance, true);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private List<GameObject> DetachOrDestroy(HumanBodyBones bone, bool destroy)
        {
            List<Attachment> objects = new List<Attachment>();
            List<GameObject> results = new List<GameObject>();
            if (attachments.ContainsKey(bone))
            {
                objects = new List<Attachment>(attachments[bone]);
                attachments.Remove(bone);

                for (int i = 0; i < objects.Count; ++i)
                {
                    if (objects[i] != null && objects[i].instance != null)
                    {
                        objects[i].instance.transform.SetParent(null);

                        if (onDetach != null)
                        {
                            onDetach.Invoke(new EventData
                            {
                                attachment = objects[i].instance,
                                bone = bone,
                                isDestroy = destroy
                            });
                        }

                        if (destroy) Destroy(objects[i].instance);
                        else results.Add(objects[i].instance);
                    }
                }
            }

            return results;
        }

        private bool DetachOrDestroy(GameObject instance, bool destroy)
        {
            foreach (KeyValuePair<HumanBodyBones, List<Attachment>> item in attachments)
            {
                if (item.Value == null) continue;

                int subItemIndex = -1;
                for (int i = 0; i < attachments[item.Key].Count; ++i)
                {
                    if (attachments[item.Key][i].instance == instance)
                    {
                        subItemIndex = i;
                        break;
                    }
                }

                if (subItemIndex >= 0)
                {
                    attachments[item.Key].RemoveAt(subItemIndex);
                    instance.transform.SetParent(null);

                    if (onDetach != null)
                    {
                        onDetach.Invoke(new EventData
                        {
                            attachment = instance,
                            bone = item.Key,
                            isDestroy = destroy
                        });
                    }

                    if (destroy) Destroy(instance);

                    if (attachments[item.Key].Count == 0)
                    {
                        attachments.Remove(item.Key);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}