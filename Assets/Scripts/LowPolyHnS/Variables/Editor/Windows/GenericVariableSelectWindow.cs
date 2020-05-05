using System;
using LowPolyHnS.Core;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    public abstract class GenericVariableSelectWindow : PopupWindowContent
    {
        private const float SEARCH_HEIGHT = 28f;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private SearchField searchField;
        private string searchText = "";
        private bool searchFocus = true;

        private float windowWidth;
        private Vector2 scroll = Vector2.zero;
        private bool keyPressedEnter;
        private bool keyPressedUp;
        private bool keyPressedDown;

        private int varIndex;
        private Rect varRect = Rect.zero;

        private Action<string> callback;
        private GUIContent[] variables;
        private int allowTypesMask;

        private GUIStyle styleItem;
        private GUIStyle styleBackground;

        // INITIALIZERS: --------------------------------------------------------------------------

        public GenericVariableSelectWindow(Rect ctaRect, Action<string> callback, int allowTypesMask)
        {
            windowWidth = ctaRect.width;
            this.callback = callback;
            this.allowTypesMask = allowTypesMask;
        }

        public override void OnOpen()
        {
            variables = GetVariables(allowTypesMask);

            searchField = new SearchField();
            searchFocus = true;
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            styleItem = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
            styleItem.fixedHeight = 20f;
            styleItem.padding = new RectOffset(
                5,
                5,
                styleItem.padding.top,
                styleItem.padding.bottom
            );

            styleItem.margin = new RectOffset(0, 0, 0, 0);
            styleItem.imagePosition = ImagePosition.ImageLeft;

            styleBackground = new GUIStyle();
            styleBackground.margin = new RectOffset(0, 0, 0, 0);
            styleBackground.padding = new RectOffset(0, 0, 0, 0);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(Mathf.Max(150, windowWidth), 300);
        }

        // VIRTUAL & ABSTRACT METHODS: ------------------------------------------------------------

        protected abstract GUIContent[] GetVariables(int allowTypesMask);
        protected abstract void PaintFooter();

        // PAINT METHODS: -------------------------------------------------------------------------

        public override void OnGUI(Rect rect)
        {
            HandleKeyboardInput();
            PaintSearch(rect);

            scroll = GUILayout.BeginScrollView(
                scroll,
                GUIStyle.none,
                GUI.skin.verticalScrollbar
            );

            for (int i = 0; i < variables.Length; ++i)
            {
                if (variables[i].text.Contains(searchText))
                {
                    PaintVariable(i);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            float scrollHeight = GUILayoutUtility.GetLastRect().height;

            if (keyPressedDown && varIndex < variables.Length - 1)
            {
                varIndex++;
                Event.current.Use();
            }
            else if (keyPressedUp && varIndex > 0)
            {
                varIndex--;
                Event.current.Use();
            }

            if (Event.current.type == EventType.Repaint &&
                (keyPressedUp || keyPressedDown))
            {
                if (varRect != Rect.zero)
                {
                    if (scroll.y > varRect.y)
                    {
                        scroll = Vector2.up * varRect.position.y;
                        editorWindow.Repaint();
                    }
                    else if (scroll.y + scrollHeight < varRect.position.y + varRect.size.y)
                    {
                        float positionY = varRect.y + varRect.height - scrollHeight;
                        scroll = Vector2.up * positionY;
                        editorWindow.Repaint();
                    }
                }
            }

            PaintFooter();

            if (Event.current.type == EventType.MouseMove ||
                Event.current.type == EventType.MouseDown)
            {
                editorWindow.Repaint();
            }
        }

        // PAINT METHODS: -------------------------------------------------------------------------

        private void PaintSearch(Rect rect)
        {
            Rect rectWrap = GUILayoutUtility.GetRect(rect.width, SEARCH_HEIGHT);
            Rect rectSearch = new Rect(
                rectWrap.x + 5f,
                rectWrap.y + 5f,
                rectWrap.width - 10f,
                rectWrap.height - 10f
            );

            GUI.BeginGroup(rectWrap, CoreGUIStyles.GetSearchBox());
            searchText = searchField.OnGUI(rectSearch, searchText);

            if (searchFocus)
            {
                searchField.SetFocus();
                searchFocus = false;
            }

            GUI.EndGroup();
        }

        private void PaintVariable(int index)
        {
            bool mouseEnter = varIndex == index && Event.current.type == EventType.MouseDown;
            Rect buttonRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);

            bool buttonHasFocus = varIndex == index;
            if (Event.current.type == EventType.Repaint)
            {
                if (varIndex == index) varRect = buttonRect;

                styleItem.Draw(
                    buttonRect,
                    variables[index],
                    buttonHasFocus,
                    buttonHasFocus,
                    false,
                    false
                );
            }

            if (buttonHasFocus && (mouseEnter || keyPressedEnter))
            {
                if (keyPressedEnter) Event.current.Use();
                Callback(variables[index].text);
                editorWindow.Close();
            }

            if (Event.current.type == EventType.MouseMove &&
                GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                varIndex = index;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void HandleKeyboardInput()
        {
            keyPressedUp = false;
            keyPressedDown = false;
            keyPressedEnter = false;

            if (Event.current.type != EventType.KeyDown) return;

            keyPressedUp = Event.current.keyCode == KeyCode.UpArrow;
            keyPressedDown = Event.current.keyCode == KeyCode.DownArrow;

            keyPressedEnter = Event.current.keyCode == KeyCode.KeypadEnter ||
                              Event.current.keyCode == KeyCode.Return;
        }

        private void Callback(string name)
        {
            name = VariableEditor.ProcessName(name);
            if (callback != null) callback(name);
        }
    }
}