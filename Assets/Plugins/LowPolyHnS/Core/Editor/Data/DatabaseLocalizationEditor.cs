using System;
using System.Collections.Generic;
using System.IO;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LowPolyHnS.Localization
{
    [CustomEditor(typeof(DatabaseLocalization))]
    public class DatabaseLocalizationEditor : IDatabaseEditor
    {
        private const float LANGUAGES_NAME_WIDTH = 80f;
        private const string MSG_DELETE_TITLE = "Deleting a language will permanently remove all its translations";
        private const string MSG_DELETE_CONTN = "Are you sure you want to continue?";
        private const string MSG_SAVE = "Select where to export Translation file";
        private const string MSG_LOAD = "Select the translation file to load";

        [Serializable]
        public class Translations
        {
            [Serializable]
            public class Content
            {
                public int key;
                public string text;

                public Content(int key, string text)
                {
                    this.key = key;
                    this.text = text;
                }
            }

            public SystemLanguage language;
            public List<Content> translations;

            public Translations(SystemLanguage language)
            {
                this.language = language;
                translations = new List<Content>();
            }

            public void AddTranslation(int key, string text)
            {
                translations.Add(new Content(key, text));
            }
        }

        private static readonly string[] OPTIONS =
        {
            "Languages",
            "Texts"
        };

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private int option;

        private SerializedProperty spLanguages;
        private SerializedProperty spTranslationList;

        private ReorderableList languages;
        private GUIStyle styleLanguagesPadding;

        private bool stylesInitialized;
        private GUIStyle styleLabelText;

        // INITIALIZE: -------------------------------------------------------------------------------------------------

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;
            spLanguages = serializedObject.FindProperty("languages");
            spTranslationList = serializedObject.FindProperty("translationList");

            languages = new ReorderableList(
                serializedObject,
                spLanguages,
                true, false, true, true
            );

            languages.drawElementCallback = DrawLanguagesElement;
            languages.onCanRemoveCallback = DrawLanguageCanRemove;
            languages.onRemoveCallback = DrawLanguageRemove;
            languages.onAddDropdownCallback = DrawLanguageAdd;

            styleLanguagesPadding = new GUIStyle();
            styleLanguagesPadding.padding = new RectOffset(5, 5, 2, 2);
        }

        // OVERRIDE METHODS: -------------------------------------------------------------------------------------------
        
        public override string GetName()
        {
            return "Localization";
        }

        public override bool CanBeDecoupled()
        {
            return true;
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            if (!stylesInitialized)
            {
                InitializeStyles();
                stylesInitialized = true;
            }

            serializedObject.Update();

            option = GUILayout.Toolbar(option, OPTIONS);
            switch (option)
            {
                case 0:
                    PaintLanguages();
                    break;
                case 1:
                    PaintTexts();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        // LANGUAGES: --------------------------------------------------------------------------------------------------

        private void PaintLanguages()
        {
            EditorGUILayout.BeginVertical(styleLanguagesPadding);
            languages.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        private void DrawLanguagesElement(Rect rect, int index, bool active, bool focus)
        {
            Rect labelRect = new Rect(
                rect.x,
                rect.y + rect.height / 2.0f - EditorGUIUtility.singleLineHeight / 2.0f,
                LANGUAGES_NAME_WIDTH,
                EditorGUIUtility.singleLineHeight
            );

            SerializedProperty property = languages.serializedProperty.GetArrayElementAtIndex(index);
            string label = ((SystemLanguage) property.FindPropertyRelative("language").intValue).ToString();
            EditorGUI.LabelField(labelRect, label);

            Rect restRect = new Rect(
                rect.x + labelRect.width,
                rect.y + rect.height / 2.0f - EditorGUIUtility.singleLineHeight / 2.0f,
                rect.width - labelRect.width,
                EditorGUIUtility.singleLineHeight
            );

            DrawLanguagesElementData(restRect, index);
        }

        private void DrawLanguagesElementData(Rect rect, int index)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - 2 * LANGUAGES_NAME_WIDTH, rect.height);

            if (index == 0)
            {
                EditorGUI.LabelField(labelRect, "Main Language", EditorStyles.boldLabel);
            }
            else
            {
                string update = spLanguages.GetArrayElementAtIndex(index).FindPropertyRelative("updateDate")
                    .stringValue;
                if (!string.IsNullOrEmpty(update))
                {
                    update = string.Format("Last Update: {0}", update);
                    EditorGUI.LabelField(labelRect, update);
                }
            }

            EditorGUI.BeginDisabledGroup(index == 0);

            Rect btnRect1 = new Rect(
                rect.x + (rect.width - 2 * LANGUAGES_NAME_WIDTH), rect.y,
                LANGUAGES_NAME_WIDTH, rect.height
            );

            Rect btnRect2 = new Rect(
                rect.x + (rect.width - 1 * LANGUAGES_NAME_WIDTH), rect.y,
                LANGUAGES_NAME_WIDTH, rect.height
            );

            if (GUI.Button(btnRect1, "Import...", EditorStyles.miniButtonLeft))
            {
                ImportLanguage(index);
            }

            if (GUI.Button(btnRect2, "Export...", EditorStyles.miniButtonRight))
            {
                ExportLanguage(index);
            }

            EditorGUI.EndDisabledGroup();
        }

        private bool DrawLanguageCanRemove(ReorderableList list)
        {
            return list.serializedProperty.arraySize > 1;
        }

        private void DrawLanguageRemove(ReorderableList list)
        {
            if (list.index < 0 || list.index >= list.count) return;

            if (EditorUtility.DisplayDialog(MSG_DELETE_TITLE, MSG_DELETE_CONTN, "Yes", "Cancel"))
            {
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawLanguageAdd(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
            {
                int languagesSize = spLanguages.arraySize;
                bool languageFound = false;
                for (int i = 0; !languageFound && i < languagesSize; ++i)
                {
                    if (spLanguages.GetArrayElementAtIndex(i).FindPropertyRelative("language").intValue ==
                        (int) language)
                    {
                        menu.AddDisabledItem(new GUIContent(language.ToString()));
                        languageFound = true;
                    }
                }

                if (!languageFound)
                {
                    menu.AddItem(
                        new GUIContent(language.ToString()), false,
                        AddLanguage, language
                    );
                }
            }

            menu.ShowAsContext();
        }

        // TEXTS METHODS: ----------------------------------------------------------------------------------------------

        private const string MSG_DELETE = "Delete";
        private const string MSG_TEXTS = "List of all the Localized texts. Delete those unused from here.";

        private void PaintTexts()
        {
            EditorGUILayout.HelpBox(MSG_TEXTS, MessageType.Info);
            int translationsCount = spTranslationList.arraySize;
            int removeIndex = -1;

            for (int i = 0; i < translationsCount; ++i)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                SerializedProperty spText = spTranslationList.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField(spText.FindPropertyRelative("placeholder").stringValue, styleLabelText);

                if (GUILayout.Button(MSG_DELETE, EditorStyles.miniButton, GUILayout.Width(50f)))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (removeIndex >= 0 && removeIndex < translationsCount)
            {
                spTranslationList.DeleteArrayElementAtIndex(removeIndex);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private void ExportLanguage(int index)
        {
            SerializedProperty spLanguages = languages.serializedProperty.GetArrayElementAtIndex(index);
            int language = spLanguages.FindPropertyRelative("language").intValue;
            int size = spTranslationList.arraySize;
            Translations translations = new Translations((SystemLanguage) language);

            for (int i = 0; i < size; ++i)
            {
                SerializedProperty property = spTranslationList.GetArrayElementAtIndex(i);
                int key = property.FindPropertyRelative("key").intValue;

                if (key == 0) continue;

                bool translationFound = false;
                SerializedProperty content = property.FindPropertyRelative("content");
                for (int j = 0; !translationFound && j < content.arraySize; ++j)
                {
                    SerializedProperty contentItem = content.GetArrayElementAtIndex(j);
                    if (contentItem.FindPropertyRelative("language").intValue == language)
                    {
                        string text = contentItem.FindPropertyRelative("text").stringValue;
                        if (!string.IsNullOrEmpty(text))
                        {
                            translationFound = true;
                            translations.AddTranslation(key, text);
                        }
                    }
                }

                if (!translationFound)
                {
                    string placeholder = property.FindPropertyRelative("placeholder").stringValue;
                    translations.AddTranslation(key, placeholder);
                }
            }

            string fileName = string.Format("{0}.json", ((SystemLanguage) language).ToString());
            string savePath = EditorUtility.SaveFilePanel(MSG_SAVE, "", fileName, "json");

            if (string.IsNullOrEmpty(savePath))
            {
                EditorApplication.Beep();
                return;
            }

            File.WriteAllText(savePath, EditorJsonUtility.ToJson(translations, true));
        }

        private void ImportLanguage(int index)
        {
            string filePath = EditorUtility.OpenFilePanelWithFilters(MSG_LOAD, "", new[] {"JSON", "json"});
            if (string.IsNullOrEmpty(filePath))
            {
                EditorApplication.Beep();
                return;
            }

            string json = File.ReadAllText(filePath);
            Translations translations = JsonUtility.FromJson<Translations>(json);

            if (translations == null)
            {
                Debug.LogErrorFormat("Unable to parse translation file {0}: {1}", filePath, json);
                EditorApplication.Beep();
                return;
            }

            int languagesCount = spLanguages.arraySize;
            for (int i = 0; i < languagesCount; ++i)
            {
                SerializedProperty language = spLanguages.GetArrayElementAtIndex(i);
                if (language.FindPropertyRelative("language").intValue == (int) translations.language)
                {
                    language.FindPropertyRelative("updateDate").stringValue = DateTime.Now.ToString("g");
                }
            }

            int translationsCount = translations.translations.Count;
            for (int i = 0; i < translationsCount; ++i)
            {
                int key = translations.translations[i].key;
                string text = translations.translations[i].text;

                int size = spTranslationList.arraySize;
                for (int j = 0; j < size; ++j)
                {
                    SerializedProperty item = spTranslationList.GetArrayElementAtIndex(j);
                    if (item.FindPropertyRelative("key").intValue == key)
                    {
                        SerializedProperty itemContent = item.FindPropertyRelative("content");
                        int itemContentSize = itemContent.arraySize;
                        bool languageFound = false;
                        for (int k = 0; k < itemContentSize; ++k)
                        {
                            SerializedProperty itemContentK = itemContent.GetArrayElementAtIndex(k);
                            if (itemContentK.FindPropertyRelative("language").intValue == (int) translations.language)
                            {
                                languageFound = true;
                                itemContentK.FindPropertyRelative("text").stringValue = text;
                            }
                        }

                        if (!languageFound)
                        {
                            itemContent.InsertArrayElementAtIndex(itemContentSize);
                            SerializedProperty newItem = itemContent.GetArrayElementAtIndex(itemContentSize);
                            newItem.FindPropertyRelative("language").intValue = (int) translations.language;
                            newItem.FindPropertyRelative("text").stringValue = text;
                        }
                    }
                }
            }
        }

        private void AddLanguage(object data)
        {
            int addIndex = spLanguages.arraySize;
            string date = DateTime.Now.ToString("g");

            spLanguages.InsertArrayElementAtIndex(addIndex);
            spLanguages.GetArrayElementAtIndex(addIndex).FindPropertyRelative("language").intValue = (int) data;
            spLanguages.GetArrayElementAtIndex(addIndex).FindPropertyRelative("updateDate").stringValue = date;

            languages.index = addIndex;

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void InitializeStyles()
        {
            styleLabelText = new GUIStyle(EditorStyles.label);
            styleLabelText.wordWrap = true;
            styleLabelText.richText = true;
        }

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public void AddTranslation(int key, string placeholder = "")
        {
            OnEnable();
            int index = spTranslationList.arraySize;
            spTranslationList.InsertArrayElementAtIndex(index);
            SerializedProperty spTranslationElem = spTranslationList.GetArrayElementAtIndex(index);

            spTranslationElem.FindPropertyRelative("key").intValue = key;
            spTranslationElem.FindPropertyRelative("placeholder").stringValue = placeholder;

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public void RemoveTranslation(int key)
        {
            OnEnable();
            int size = spTranslationList.arraySize;

            for (int i = size - 1; i >= 0; --i)
            {
                if (spTranslationList.GetArrayElementAtIndex(i).FindPropertyRelative("key").intValue == key)
                {
                    spTranslationList.DeleteArrayElementAtIndex(i);
                }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public SerializedProperty GetPlaceholder(int key)
        {
            int translationsSize = spTranslationList.arraySize;
            for (int i = 0; i < translationsSize; ++i)
            {
                SerializedProperty item = spTranslationList.GetArrayElementAtIndex(i);
                if (item.FindPropertyRelative("key").intValue == key)
                {
                    return item.FindPropertyRelative("placeholder");
                }
            }

            return null;
        }
    }
}