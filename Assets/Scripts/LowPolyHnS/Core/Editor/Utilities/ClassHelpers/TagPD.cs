using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        private const string UNTAGGED = "Untagged";
        public List<string> tags = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (tags.Count == 0) BuildTags();

                EditorGUI.BeginProperty(position, label, property);

                string propertyString = property.stringValue;
                int index = -1;
                if (propertyString == "")
                {
                    index = 0;
                }
                else
                {
                    for (int i = 1; i < tags.Count; i++)
                    {
                        if (tags[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                index = EditorGUI.Popup(position, label.text, index, tags.ToArray());
                if (index >= 1)
                {
                    property.stringValue = tags[index];
                }
                else
                {
                    property.stringValue = "";
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void BuildTags()
        {
            tags = new List<string>();

            tags.Add(UNTAGGED);
            tags.AddRange(InternalEditorUtility.tags);
        }
    }
}