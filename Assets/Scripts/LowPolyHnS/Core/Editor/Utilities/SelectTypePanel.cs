using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LowPolyHnS.DataStructures;
using LowPolyHnS.Variables;
using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public class SelectTypePanel : PopupWindowContent
    {
        private const string SEARCHBOX_NAME = "searchbox";
        private const string SEARCHBOX_KEY = "LowPolyHnS-selecttypepanel-searchbox-{0}";

        private static readonly char[] SEPARATOR = {'/'};

        private const BindingFlags BINDING_FLAGS = BindingFlags.Public |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy;

        public const string ICONS_ACTIONS_PATH = "Assets/EditorIcons/Actions/";
        public const string ICONS_CONDITIONS_PATH = "Assets/EditorIcons/Conditions/";
        public const string ICONS_IGNITERS_PATH = "Assets/EditorIcons/Icons/Igniters/";
        public const string ICONS_VARIABLES_PATH = "Assets/EditorIcons/Variables/";
        public const string CUSTOM_ICON_PATH_VARIABLE = "CUSTOM_ICON_PATH";

        private const float ARROW_SIZE = 13f;
        public const float WINDOW_WIDTH = 200f;
        public const float WINDOW_HEIGHT = 300f;

        private class NodeData
        {
            public GUIContent content;
            public Type component;
            public int listIndex;

            public NodeData(string content, Type component = null)
            {
                Texture2D icon;
                string[] sections = content.Split(SEPARATOR);
                string name = sections[sections.Length - 1];

                if (component == null) icon = EditorGUIUtility.FindTexture("Folder Icon");
                else
                {
                    string iconsPath = "";
                    if (component.IsSubclassOf(typeof(ICondition))) iconsPath = ICONS_CONDITIONS_PATH;
                    else if (component.IsSubclassOf(typeof(IAction))) iconsPath = ICONS_ACTIONS_PATH;
                    else if (component.IsSubclassOf(typeof(Igniter))) iconsPath = ICONS_IGNITERS_PATH;
                    else if (component.IsSubclassOf(typeof(VariableBase))) iconsPath = ICONS_VARIABLES_PATH;

                    FieldInfo customIconsFieldInfo = component.GetField(CUSTOM_ICON_PATH_VARIABLE, BINDING_FLAGS);
                    if (customIconsFieldInfo != null)
                    {
                        string customIconsPath = (string) customIconsFieldInfo.GetValue(null);
                        if (!string.IsNullOrEmpty(customIconsPath))
                        {
                            iconsPath = customIconsPath;
                        }
                    }

                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath + name + ".png");
                    if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath + "Default.png");
                    if (icon == null) icon = EditorGUIUtility.FindTexture("GameObject Icon");
                }

                this.content = new GUIContent(" " + name, icon);
                this.component = component;
                listIndex = 0;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private GUIStyle searchBoxStyle;
        private GUIStyle searchFieldStyle;
        private GUIStyle searchCloseOnStyle;
        private GUIStyle searchCloseOffStyle;
        private GUIStyle headerBoxStyle;
        private GUIStyle headerTitleStyle;
        private GUIStyle headerBackStyle;
        private GUIStyle elementSelectorStyle;

        private int listIndex;
        private Rect listSelectedRect = Rect.zero;

        private string search = "";
        private bool searchFocus = true;
        private TreeNode<NodeData> categorizedTree;
        private List<TreeNode<NodeData>> uncategorizedList;

        private TreeNode<NodeData> currentBranch;
        private Stack<TreeNode<NodeData>> pathTrace;
        private KeyValuePair<string, TreeNode<NodeData>> cachedSearch;

        private Vector2 scroll;
        private Action<Type> callback;

        private string rootName;
        private Type baseType;
        private float winWidth;

        private bool keyPressedAny;
        private bool keyPressedRight;
        private bool keyPressedBack;
        private bool keyPressedUp;
        private bool keyPressedDown;
        private bool keyPressedEnter;
        private bool keyFlagVerticalMoved;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public SelectTypePanel(Action<Type> callback, string rootName, Type baseType, float width = 0.0f)
        {
            this.callback = callback;
            this.rootName = rootName;
            this.baseType = baseType;
            winWidth = width;
        }

        public override void OnOpen()
        {
            searchBoxStyle = new GUIStyle(GUI.skin.FindStyle("TabWindowBackground"));
            searchFieldStyle = new GUIStyle(GUI.skin.FindStyle("SearchTextField"));
            searchCloseOnStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButton"));
            searchCloseOffStyle = new GUIStyle(GUI.skin.FindStyle("SearchCancelButtonEmpty"));

            headerTitleStyle = new GUIStyle(GUI.skin.FindStyle("BoldLabel"));
            headerTitleStyle.padding = new RectOffset(
                20, headerTitleStyle.padding.right,
                headerTitleStyle.padding.top, headerTitleStyle.padding.bottom
            );

            headerBoxStyle = new GUIStyle(GUI.skin.FindStyle("IN BigTitle"));
            headerBoxStyle.margin = new RectOffset(0, 0, 0, 0);

            elementSelectorStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
            elementSelectorStyle.fixedHeight = 20f;
            elementSelectorStyle.padding = new RectOffset(
                5, 5, elementSelectorStyle.padding.top, elementSelectorStyle.padding.bottom
            );
            elementSelectorStyle.imagePosition = ImagePosition.ImageLeft;

            scroll = Vector2.zero;
            searchFocus = true;
            cachedSearch = new KeyValuePair<string, TreeNode<NodeData>>();

            CreateList();
            editorWindow.Focus();
            search = EditorPrefs.GetString(string.Format(SEARCHBOX_KEY, rootName), "");
        }

        public override void OnClose()
        {
            EditorPrefs.SetString(string.Format(SEARCHBOX_KEY, rootName), search);
            base.OnClose();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(
                Mathf.Max(winWidth, WINDOW_WIDTH),
                WINDOW_HEIGHT
            );
        }

        private void CreateList()
        {
            NodeData rootData = new NodeData(rootName);
            categorizedTree = new TreeNode<NodeData>(rootName, rootData);
            uncategorizedList = new List<TreeNode<NodeData>>(
                new TreeNode<NodeData>(rootName, rootData)
            );

            List<Type> types = GetAllClassTypesOf(baseType);
            int typesSize = types.Count;

            for (int i = 0; i < typesSize; ++i)
            {
                string actionName = (string) types[i].GetField("NAME", BINDING_FLAGS).GetValue(null);
                string[] categories = GetCategories(actionName);

                TreeNode<NodeData> node = categorizedTree;
                for (int j = 0; j < categories.Length; ++j)
                {
                    if (node.HasChild(categories[j]))
                    {
                        node = node.GetChild(categories[j]);
                    }
                    else
                    {
                        NodeData nodeData = new NodeData(categories[j]);
                        TreeNode<NodeData> treeNode = new TreeNode<NodeData>(categories[j], nodeData);
                        node = node.AddChild(treeNode);
                    }
                }

                NodeData leafData = new NodeData(actionName, types[i]);
                node.AddChild(new TreeNode<NodeData>(actionName, leafData));
                uncategorizedList.Add(new TreeNode<NodeData>(actionName, leafData));
            }

            currentBranch = categorizedTree;
            pathTrace = new Stack<TreeNode<NodeData>>();
        }

        // WINDOW GUI: ----------------------------------------------------------------------------

        public override void OnGUI(Rect rect)
        {
            HandleKeyboardInput();
            PaintSearch();

            if (string.IsNullOrEmpty(this.search))
            {
                listIndex = currentBranch.GetData().listIndex;
                PaintElements(currentBranch, pathTrace.Count > 0);
            }
            else
            {
                if (cachedSearch.Key != this.search)
                {
                    TreeNode<NodeData> emptyTree = new TreeNode<NodeData>("Search", null);
                    cachedSearch = new KeyValuePair<string, TreeNode<NodeData>>(this.search, emptyTree);
                    string search = this.search.ToLower();
                    int listCount = uncategorizedList.Count;

                    for (int i = 0; i < listCount; ++i)
                    {
                        string childID = uncategorizedList[i].GetID().ToLower();
                        if (childID.Contains(search))
                        {
                            cachedSearch.Value.AddChild(uncategorizedList[i]);
                        }
                    }

                    listIndex = 0;
                }

                PaintElements(cachedSearch.Value, false);
            }

            bool repaintEvent = false;
            repaintEvent = repaintEvent || Event.current.type == EventType.MouseMove;
            repaintEvent = repaintEvent || Event.current.type == EventType.MouseDown;
            repaintEvent = repaintEvent || keyPressedAny;

            if (repaintEvent) editorWindow.Repaint();
        }

        private void HandleKeyboardInput()
        {
            keyPressedAny = false;
            keyPressedRight = false;
            keyPressedBack = false;
            keyPressedUp = false;
            keyPressedDown = false;
            keyPressedEnter = false;

            if (Event.current.type != EventType.KeyDown) return;

            keyPressedAny = true;
            keyPressedRight = Event.current.keyCode == KeyCode.RightArrow;
            keyPressedBack = Event.current.keyCode == KeyCode.LeftArrow;
            keyPressedUp = Event.current.keyCode == KeyCode.UpArrow;
            keyPressedDown = Event.current.keyCode == KeyCode.DownArrow;

            keyPressedEnter = Event.current.keyCode == KeyCode.KeypadEnter ||
                              Event.current.keyCode == KeyCode.Return;

            keyFlagVerticalMoved = keyPressedUp ||
                                   keyPressedDown;
        }

        private void PaintSearch()
        {
            EditorGUILayout.BeginHorizontal(searchBoxStyle);

            GUI.SetNextControlName(SEARCHBOX_NAME);
            search = EditorGUILayout.TextField(search, searchFieldStyle);

            if (searchFocus)
            {
                EditorGUI.FocusTextInControl(SEARCHBOX_NAME);
                searchFocus = false;
            }

            GUIStyle style = string.IsNullOrEmpty(search) ? searchCloseOffStyle : searchCloseOnStyle;
            if (GUILayout.Button("", style))
            {
                search = "";
                GUIUtility.keyboardControl = 0;
                searchFocus = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool PaintTitle(string title, bool backButton)
        {
            bool goBack = false;
            EditorGUILayout.BeginHorizontal(headerBoxStyle);

            if (backButton && keyPressedBack)
            {
                Event.current.Use();
            }

            if (backButton && (GUILayout.Button(title, headerTitleStyle) || keyPressedBack))
            {
                listIndex = 0;
                goBack = true;
            }
            else if (!backButton)
            {
                EditorGUILayout.LabelField(title, headerTitleStyle);
            }

            if (Event.current.type == EventType.Repaint && backButton)
            {
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                CoreGUIStyles.GetButtonLeftArrow().Draw(
                    new Rect(
                        buttonRect.x,
                        buttonRect.y + buttonRect.height / 2.0f - ARROW_SIZE / 2.0f,
                        ARROW_SIZE, ARROW_SIZE
                    ),
                    false, false, false, false
                );
            }

            EditorGUILayout.EndHorizontal();
            return goBack;
        }

        private void PaintElements(TreeNode<NodeData> branch, bool backButton)
        {
            if (PaintTitle(branch.GetID(), backButton))
            {
                currentBranch = pathTrace.Pop();
                listIndex = currentBranch.GetData().listIndex;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none, GUI.skin.verticalScrollbar);
            int elemIndex = 0;

            foreach (TreeNode<NodeData> element in branch)
            {
                NodeData nodeData = element.GetData();

                bool mouseEnter = listIndex == elemIndex && Event.current.type == EventType.MouseDown;

                Rect buttonRect = GUILayoutUtility.GetRect(nodeData.content, elementSelectorStyle);

                bool buttonHasFocus = listIndex == elemIndex;
                if (Event.current.type == EventType.Repaint)
                {
                    if (listIndex == elemIndex) listSelectedRect = buttonRect;

                    elementSelectorStyle.Draw(
                        buttonRect,
                        nodeData.content,
                        buttonHasFocus,
                        buttonHasFocus,
                        false,
                        false
                    );

                    if (nodeData.component == null)
                    {
                        CoreGUIStyles.GetButtonRightArrow().Draw(
                            new Rect(
                                buttonRect.x + buttonRect.width - ARROW_SIZE,
                                buttonRect.y + buttonRect.height / 2.0f - ARROW_SIZE / 2.0f,
                                ARROW_SIZE, ARROW_SIZE
                            ),
                            false, false, false, false
                        );
                    }
                }

                if (buttonHasFocus)
                {
                    if (nodeData.component == null)
                    {
                        if (mouseEnter || keyPressedRight || keyPressedEnter)
                        {
                            if (keyPressedRight) Event.current.Use();
                            if (keyPressedEnter) Event.current.Use();

                            currentBranch.GetData().listIndex = listIndex;
                            pathTrace.Push(currentBranch);

                            currentBranch = currentBranch.GetChild(element.GetID());
                            listIndex = 0;
                        }
                    }
                    else
                    {
                        if (mouseEnter || keyPressedEnter)
                        {
                            if (keyPressedEnter) Event.current.Use();
                            if (callback != null) callback(nodeData.component);
                            editorWindow.Close();
                        }
                    }
                }

                if (Event.current.type == EventType.MouseMove &&
                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    listIndex = elemIndex;
                    currentBranch.GetData().listIndex = listIndex;
                }

                ++elemIndex;
            }

            if (keyPressedDown && listIndex < elemIndex - 1)
            {
                listIndex++;
                currentBranch.GetData().listIndex = listIndex;
                Event.current.Use();
            }
            else if (keyPressedUp && listIndex > 0)
            {
                listIndex--;
                currentBranch.GetData().listIndex = listIndex;
                Event.current.Use();
            }

            EditorGUILayout.EndScrollView();
            float scrollHeight = GUILayoutUtility.GetLastRect().height;

            if (Event.current.type == EventType.Repaint && keyFlagVerticalMoved)
            {
                keyFlagVerticalMoved = false;
                if (listSelectedRect != Rect.zero)
                {
                    if (scroll.y > listSelectedRect.y)
                    {
                        scroll = Vector2.up * listSelectedRect.position.y;
                        editorWindow.Repaint();
                    }
                    else if (scroll.y + scrollHeight < listSelectedRect.position.y + listSelectedRect.size.y)
                    {
                        float positionY = listSelectedRect.y + listSelectedRect.height - scrollHeight;
                        scroll = Vector2.up * positionY;
                        editorWindow.Repaint();
                    }
                }
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private List<Type> GetAllClassTypesOf(Type parentType)
        {
            List<Type> result = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                result.AddRange(assemblies[i].GetTypes().Where(
                    myType => myType.IsClass &&
                              !myType.IsAbstract &&
                              myType.IsSubclassOf(parentType)
                ));
            }

            return result;
        }

        public static string GetName(string name)
        {
            string[] categories = name.Split('/');
            if (categories.Length > 0) return categories[categories.Length - 1];
            return "no-name";
        }

        private static string[] GetCategories(string name)
        {
            string[] categories = name.Split('/');
            if (categories.Length > 1)
            {
                string[] subarrayCategories = new string[categories.Length - 1];
                for (int i = 0; i < subarrayCategories.Length; ++i)
                {
                    subarrayCategories[i] = categories[i];
                }

                categories = subarrayCategories;
            }
            else
            {
                categories = new string[0];
            }

            return categories;
        }
    }
}