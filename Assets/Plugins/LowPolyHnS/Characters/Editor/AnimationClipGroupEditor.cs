using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [CustomEditor(typeof(AnimationClipGroup))]
    public class AnimationClipGroupEditor : Editor
    {
        private AnimationClipGroup group;
        private bool isDragginClip;

        // INITIALIZE: ----------------------------------------------------------------------------

        private void OnEnable()
        {
            group = target as AnimationClipGroup;
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (target == null || group == null) return;
            if (!string.IsNullOrEmpty(group.message))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(group.message, MessageType.Info);
                EditorGUILayout.Space();
            }

            PaintDropClip();
            PaintAssets();
        }

        private void PaintDropClip()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            Event evt = Event.current;
            Rect dropRect = GUILayoutUtility.GetRect(
                0.0f,
                50.0f,
                GUILayout.ExpandWidth(true)
            );

            GUIStyle styleDropZone = isDragginClip
                ? CoreGUIStyles.GetDropZoneActive()
                : CoreGUIStyles.GetDropZoneNormal();

            GUI.Box(dropRect, "Drop animation clip", styleDropZone);

            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.button);
            buttonRect = new Rect(
                buttonRect.x + EditorGUIUtility.labelWidth,
                buttonRect.y,
                buttonRect.width - EditorGUIUtility.labelWidth,
                buttonRect.height
            );

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    isDragginClip = false;
                    if (!dropRect.Contains(evt.mousePosition)) break;
                    if (DragAndDrop.objectReferences.Length == 0) break;


                    AnimationClip draggedClip = DragAndDrop.objectReferences[0] as AnimationClip;
                    if (draggedClip == null) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragUpdated)
                    {
                        isDragginClip = true;
                    }
                    else if (evt.type == EventType.DragPerform)
                    {
                        isDragginClip = false;

                        DragAndDrop.AcceptDrag();

                        AnimationClip clip = Instantiate(draggedClip);
                        clip.name = draggedClip.name;

                        AssetDatabase.AddObjectToAsset(clip, group);
                        string path = AssetDatabase.GetAssetPath(group);
                        AssetDatabase.ImportAsset(path);

                        Event.current.Use();
                    }

                    break;
            }
        }

        private const float DELETE_WIDTH = 25f;

        private void PaintAssets()
        {
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(target.GetInstanceID())
            );

            int removeIndex = -1;
            for (int i = 0; i < subAssets.Length; ++i)
            {
                if (subAssets[i] as AnimationClip == null) continue;

                Rect rectTotal = GUILayoutUtility.GetRect(
                    GUIContent.none,
                    CoreGUIStyles.GetButtonMid()
                );

                Rect rectLabel = new Rect(
                    rectTotal.x,
                    rectTotal.y,
                    rectTotal.width - DELETE_WIDTH,
                    rectTotal.height
                );

                Rect rectButon = new Rect(
                    rectLabel.x + rectLabel.width,
                    rectLabel.y,
                    DELETE_WIDTH,
                    rectLabel.height
                );

                EditorGUI.LabelField(
                    rectLabel,
                    subAssets[i].name,
                    CoreGUIStyles.GetToggleButtonLeftOn()
                );

                if (GUI.Button(rectButon, "X", CoreGUIStyles.GetButtonRight()))
                {
                    removeIndex = i;
                }
            }

            if (removeIndex >= 0)
            {
                AssetDatabase.RemoveObjectFromAsset(subAssets[removeIndex]);

                string path = AssetDatabase.GetAssetPath(target);
                AssetDatabase.ImportAsset(path);
            }
        }
    }
}