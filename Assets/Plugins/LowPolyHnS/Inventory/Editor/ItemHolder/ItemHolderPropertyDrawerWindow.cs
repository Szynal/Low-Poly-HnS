using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    public class ItemHolderPropertyDrawerWindow : PopupWindowContent
    {
        private const string INPUTTEXT_NAME = "LowPolyHnS-itemholder-input";
        private const float WIN_HEIGHT = 300f;
        private static DatabaseInventory DATABASE_INVENTORY;

        private Rect windowRect = Rect.zero;
        private bool inputfieldFocus = true;
        private Vector2 scroll = Vector2.zero;
        private int itemsIndex = -1;

        private string searchText = "";
        private List<int> suggestions = new List<int>();

        private GUIStyle inputBGStyle;
        private GUIStyle suggestionHeaderStyle;
        private GUIStyle suggestionItemStyle;
        private GUIStyle searchFieldStyle;
        private GUIStyle searchCloseOnStyle;
        private GUIStyle searchCloseOffStyle;

        //ItemHolderPropertyDrawer itemHolderPropertyDrawer;
        private SerializedProperty property;

        private bool keyPressedAny;
        private bool keyPressedUp;
        private bool keyPressedDown;
        private bool keyPressedEnter;
        private bool keyFlagVerticalMoved;
        private Rect itemSelectedRect = Rect.zero;

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public ItemHolderPropertyDrawerWindow(Rect activatorRect, SerializedProperty property)
        {
            windowRect = new Rect(
                activatorRect.x,
                activatorRect.y + activatorRect.height,
                activatorRect.width,
                WIN_HEIGHT
            );

            inputfieldFocus = true;
            scroll = Vector2.zero;
            this.property = property;

            if (DATABASE_INVENTORY == null) DATABASE_INVENTORY = IDatabase.LoadDatabase<DatabaseInventory>();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(windowRect.width, WIN_HEIGHT);
        }

        public override void OnOpen()
        {
            inputBGStyle = new GUIStyle(GUI.skin.FindStyle("TabWindowBackground"));
            suggestionHeaderStyle = new GUIStyle(GUI.skin.FindStyle("IN BigTitle"));
            suggestionHeaderStyle.margin = new RectOffset(0, 0, 0, 0);
            suggestionItemStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
            searchFieldStyle = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
            searchCloseOnStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
            searchCloseOffStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButtonEmpty"));

            inputfieldFocus = true;

            searchText = "";
            suggestions = DATABASE_INVENTORY.GetItemSuggestions(searchText);
        }

        // GUI METHODS: ------------------------------------------------------------------------------------------------

        public override void OnGUI(Rect windowRect)
        {
            if (property == null)
            {
                editorWindow.Close();
                return;
            }

            property.serializedObject.Update();

            HandleKeyboardInput();

            string modSearchText = searchText;
            PaintInputfield(ref modSearchText);
            PaintSuggestions(ref modSearchText);

            searchText = modSearchText;

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (keyPressedEnter)
            {
                editorWindow.Close();
                Event.current.Use();
            }

            bool repaintEvent = false;
            repaintEvent = repaintEvent || Event.current.type == EventType.MouseMove;
            repaintEvent = repaintEvent || Event.current.type == EventType.MouseDown;
            repaintEvent = repaintEvent || keyPressedAny;
            if (repaintEvent) editorWindow.Repaint();
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private void HandleKeyboardInput()
        {
            keyPressedAny = false;
            keyPressedUp = false;
            keyPressedDown = false;
            keyPressedEnter = false;

            if (Event.current.type != EventType.KeyDown) return;

            keyPressedAny = true;
            keyPressedUp = Event.current.keyCode == KeyCode.UpArrow;
            keyPressedDown = Event.current.keyCode == KeyCode.DownArrow;

            keyPressedEnter = Event.current.keyCode == KeyCode.KeypadEnter ||
                              Event.current.keyCode == KeyCode.Return;

            keyFlagVerticalMoved = keyPressedUp ||
                                   keyPressedDown;
        }

        private void PaintInputfield(ref string modifiedText)
        {
            EditorGUILayout.BeginHorizontal(inputBGStyle);

            GUI.SetNextControlName(INPUTTEXT_NAME);
            modifiedText = EditorGUILayout.TextField(GUIContent.none, modifiedText, searchFieldStyle);


            GUIStyle style = string.IsNullOrEmpty(searchText)
                ? searchCloseOffStyle
                : searchCloseOnStyle;

            if (inputfieldFocus)
            {
                EditorGUI.FocusTextInControl(INPUTTEXT_NAME);
                inputfieldFocus = false;
            }

            if (GUILayout.Button("", style))
            {
                modifiedText = "";
                GUIUtility.keyboardControl = 0;
                GUIUtility.keyboardControl = 0;
                inputfieldFocus = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void PaintSuggestions(ref string modifiedText)
        {
            EditorGUILayout.BeginHorizontal(suggestionHeaderStyle);
            EditorGUILayout.LabelField("Suggestions", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            if (modifiedText != searchText)
            {
                suggestions = DATABASE_INVENTORY.GetItemSuggestions(modifiedText);
            }

            int suggestionCount = suggestions.Count;

            if (suggestionCount > 0)
            {
                for (int i = 0; i < suggestionCount; ++i)
                {
                    Item item = DATABASE_INVENTORY.inventoryCatalogue.items[suggestions[i]];
                    string itemName = string.IsNullOrEmpty(item.itemName.content) ? "No-name" : item.itemName.content;
                    GUIContent itemContent = new GUIContent(itemName);

                    Rect itemRect = GUILayoutUtility.GetRect(itemContent, suggestionItemStyle);
                    bool itemHasFocus = i == itemsIndex;
                    bool mouseEnter = itemHasFocus && Event.current.type == EventType.MouseDown;

                    if (Event.current.type == EventType.Repaint)
                    {
                        suggestionItemStyle.Draw(
                            itemRect,
                            itemContent,
                            itemHasFocus,
                            itemHasFocus,
                            false,
                            false
                        );
                    }

                    if (itemsIndex == i) itemSelectedRect = itemRect;

                    if (itemHasFocus)
                    {
                        if (mouseEnter || keyPressedEnter)
                        {
                            if (keyPressedEnter) Event.current.Use();
                            modifiedText = itemName;
                            SerializedProperty spItem =
                                property.FindPropertyRelative(ItemHolderPropertyDrawer.PROP_ITEM);
                            spItem.objectReferenceValue = item;
                            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                            property.serializedObject.Update();

                            editorWindow.Close();
                        }
                    }

                    if (Event.current.type == EventType.MouseMove &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        itemsIndex = i;
                    }
                }

                if (keyPressedDown && itemsIndex < suggestionCount - 1)
                {
                    itemsIndex++;
                    Event.current.Use();
                }
                else if (keyPressedUp && itemsIndex > 0)
                {
                    itemsIndex--;
                    Event.current.Use();
                }
            }

            EditorGUILayout.EndScrollView();
            float scrollHeight = GUILayoutUtility.GetLastRect().height;

            if (Event.current.type == EventType.Repaint && keyFlagVerticalMoved)
            {
                keyFlagVerticalMoved = false;
                if (itemSelectedRect != Rect.zero)
                {
                    bool isUpperLimit = scroll.y > itemSelectedRect.y;
                    bool isLowerLimit = scroll.y + scrollHeight <
                                        itemSelectedRect.position.y + itemSelectedRect.size.y;

                    if (isUpperLimit)
                    {
                        scroll = Vector2.up * itemSelectedRect.position.y;
                        editorWindow.Repaint();
                    }
                    else if (isLowerLimit)
                    {
                        float positionY = itemSelectedRect.y + itemSelectedRect.height - scrollHeight;
                        scroll = Vector2.up * positionY;
                        editorWindow.Repaint();
                    }
                }
            }
        }
    }
}