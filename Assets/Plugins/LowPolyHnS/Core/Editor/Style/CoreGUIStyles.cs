using UnityEditor;
using UnityEngine;

namespace LowPolyHnS.Core
{
    public abstract class CoreGUIStyles
    {
        private const string TAG_BG_PATH = "Assets/Plugins/LowPolyHnS/Extra/Icons/Variables/Tag.png";
        private const string DROPZONE_NORMAL_PATH = "Assets/Plugins/LowPolyHnS/Characters/Icons/DropZoneNormal.png";
        private const string DROPZONE_ACTIVE_PATH = "Assets/Plugins/LowPolyHnS/Characters/Icons/DropZoneActive.png";
        private const string DROP_MARK_PATH = "Assets/Plugins/LowPolyHnS/Extra/Icons/EditorUI/DropMark.png";

        private static GUIStyle btnLeft;
        private static GUIStyle btnRight;
        private static GUIStyle btnMid;
        private static GUIStyle btnToggleLeftAdd;
        private static GUIStyle btnToggleLeftOn;
        private static GUIStyle btnToggleLeftOff;
        private static GUIStyle btnToggleMidOn;
        private static GUIStyle btnToggleMidOff;
        private static GUIStyle btnToggleRightOn;
        private static GUIStyle btnToggleRightOff;
        private static GUIStyle btnToggleNormalOn;
        private static GUIStyle btnToggleNormalOff;
        private static GUIStyle btnToggleOn;
        private static GUIStyle btnToggleOff;
        private static GUIStyle highlight;
        private static GUIStyle dropMark;
        private static GUIStyle boxExpanded;
        private static GUIStyle box;
        private static GUIStyle helpBox;
        private static GUIStyle searchbox;
        private static GUIStyle btnToolbar;
        private static GUIStyle btnGridLeftOn;
        private static GUIStyle btnGridLeftOff;
        private static GUIStyle btnGridMidOn;
        private static GUIStyle btnGridMidOff;
        private static GUIStyle btnGridRightOn;
        private static GUIStyle btnGridRightOff;
        private static GUIStyle btnRigArrow;
        private static GUIStyle btnLftArrow;
        private static GUIStyle itemPreferencesSidebar;
        private static GUIStyle labelTag;
        private static GUIStyle textarea;
        private static GUIStyle dropZoneNormal;
        private static GUIStyle dropZoneActive;
        private static GUIStyle globalIDText;

        public static GUIStyle GetButtonLeft()
        {
            if (btnLeft == null)
            {
                btnLeft = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                btnLeft.richText = true;
            }

            return btnLeft;
        }

        public static GUIStyle GetButtonRight()
        {
            if (btnRight == null)
            {
                btnRight = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                btnRight.richText = true;
            }

            return btnRight;
        }

        public static GUIStyle GetButtonMid()
        {
            if (btnMid == null)
            {
                btnMid = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                btnMid.richText = true;
            }

            return btnMid;
        }

        public static GUIStyle GetToggleButtonLeftOn()
        {
            if (btnToggleLeftOn == null)
            {
                btnToggleLeftOn = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                btnToggleLeftOn.alignment = TextAnchor.MiddleLeft;
                btnToggleLeftOn.normal = btnToggleLeftOn.onNormal;
                btnToggleLeftOn.hover = btnToggleLeftOn.onHover;
                btnToggleLeftOn.active = btnToggleLeftOn.onActive;
                btnToggleLeftOn.focused = btnToggleLeftOn.onFocused;
                btnToggleLeftOn.richText = true;
                btnToggleLeftOn.margin.bottom = 0;
            }

            return btnToggleLeftOn;
        }

        public static GUIStyle GetToggleButtonLeftOff()
        {
            if (btnToggleLeftOff == null)
            {
                btnToggleLeftOff = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                btnToggleLeftOff.alignment = TextAnchor.MiddleLeft;
                btnToggleLeftOff.richText = true;
                btnToggleLeftOff.margin.bottom = 0;
            }

            return btnToggleLeftOff;
        }

        public static GUIStyle GetToggleButtonLeftAdd()
        {
            if (btnToggleLeftAdd == null)
            {
                btnToggleLeftAdd = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                btnToggleLeftAdd.alignment = TextAnchor.MiddleCenter;
                btnToggleLeftAdd.richText = true;
            }

            return btnToggleLeftAdd;
        }

        public static GUIStyle GetToggleButtonMidOn()
        {
            if (btnToggleMidOn == null)
            {
                btnToggleMidOn = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                btnToggleMidOn.alignment = TextAnchor.MiddleLeft;
                btnToggleMidOn.normal = btnToggleMidOn.onNormal;
                btnToggleMidOn.hover = btnToggleMidOn.onHover;
                btnToggleMidOn.active = btnToggleMidOn.onActive;
                btnToggleMidOn.focused = btnToggleMidOn.onFocused;
                btnToggleMidOn.richText = true;
                btnToggleMidOn.margin.bottom = 0;
            }

            return btnToggleMidOn;
        }

        public static GUIStyle GetToggleButtonMidOff()
        {
            if (btnToggleMidOff == null)
            {
                btnToggleMidOff = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                btnToggleMidOff.alignment = TextAnchor.MiddleLeft;
                btnToggleMidOff.richText = true;
                btnToggleMidOff.margin.bottom = 0;
            }

            return btnToggleMidOff;
        }

        public static GUIStyle GetToggleButtonRightOn()
        {
            if (btnToggleRightOn == null)
            {
                btnToggleRightOn = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                btnToggleRightOn.alignment = TextAnchor.MiddleLeft;
                btnToggleRightOn.normal = btnToggleRightOn.onNormal;
                btnToggleRightOn.hover = btnToggleRightOn.onHover;
                btnToggleRightOn.active = btnToggleRightOn.onActive;
                btnToggleRightOn.focused = btnToggleRightOn.onFocused;
                btnToggleRightOn.richText = true;
                btnToggleRightOn.margin.bottom = 0;
            }

            return btnToggleRightOn;
        }

        public static GUIStyle GetToggleButtonRightOff()
        {
            if (btnToggleRightOff == null)
            {
                btnToggleRightOff = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                btnToggleRightOff.alignment = TextAnchor.MiddleLeft;
                btnToggleRightOff.richText = true;
                btnToggleRightOff.margin.bottom = 0;
            }

            return btnToggleRightOff;
        }

        public static GUIStyle GetToggleButtonNormalOn()
        {
            if (btnToggleNormalOn == null)
            {
                btnToggleNormalOn = new GUIStyle(GetToggleButtonNormalOff());
                btnToggleNormalOn.alignment = TextAnchor.MiddleLeft;
                btnToggleNormalOn.normal = btnToggleNormalOn.onNormal;
                btnToggleNormalOn.hover = btnToggleNormalOn.onHover;
                btnToggleNormalOn.active = btnToggleNormalOn.onActive;
                btnToggleNormalOn.focused = btnToggleNormalOn.onFocused;
                btnToggleNormalOn.richText = true;
                btnToggleNormalOn.margin.bottom = 0;
            }

            return btnToggleNormalOn;
        }

        public static GUIStyle GetToggleButtonNormalOff()
        {
            if (btnToggleNormalOff == null)
            {
                btnToggleNormalOff = new GUIStyle(GUI.skin.GetStyle("Button"));
                btnToggleNormalOff.alignment = TextAnchor.MiddleLeft;
                btnToggleNormalOff.richText = true;
                btnToggleNormalOff.fixedHeight = 20f;
                btnToggleNormalOff.margin.bottom = 0;
            }

            return btnToggleNormalOff;
        }

        public static GUIStyle GetToggleButtonOn()
        {
            if (btnToggleOn == null)
            {
                btnToggleOn = new GUIStyle(GetToggleButtonOff());
                btnToggleOn.alignment = TextAnchor.MiddleLeft;
                btnToggleOn.normal = btnToggleOn.onNormal;
                btnToggleOn.hover = btnToggleOn.onHover;
                btnToggleOn.active = btnToggleOn.onActive;
                btnToggleOn.focused = btnToggleOn.onFocused;
                btnToggleOn.richText = true;
                btnToggleOn.margin.bottom = 0;
            }

            return btnToggleOn;
        }

        public static GUIStyle GetToggleButtonOff()
        {
            if (btnToggleOff == null)
            {
                btnToggleOff = new GUIStyle(GUI.skin.GetStyle("LargeButton"));
                btnToggleOff.alignment = TextAnchor.MiddleLeft;
                btnToggleOff.richText = true;
                btnToggleOff.margin.bottom = 0;
                btnToggleOff.padding = new RectOffset(
                    30, btnToggleOff.padding.right,
                    btnToggleOff.padding.top,
                    btnToggleOff.padding.bottom
                );
            }

            return btnToggleOff;
        }

        /*
		public static GUIStyle GetHighlight()
		{
			if (CoreGUIStyles.highlight == null)
			{
				CoreGUIStyles.highlight = new GUIStyle(GUI.skin.GetStyle("LightmapEditorSelectedHighlight"));
			}
			
			return CoreGUIStyles.highlight;
		}*/

        public static GUIStyle GetDropMarker()
        {
            if (dropMark == null)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(DROP_MARK_PATH);

                dropMark = new GUIStyle();
                dropMark.normal.background = texture;
                dropMark.border = new RectOffset(7, 7, 0, 0);
                dropMark.overflow = new RectOffset(4, -4, 6, -6);
                dropMark.active = dropMark.normal;
                dropMark.hover = dropMark.normal;
                dropMark.focused = dropMark.normal;
                dropMark.onNormal = dropMark.normal;
                dropMark.onActive = dropMark.normal;
                dropMark.onFocused = dropMark.normal;
            }

            return dropMark;
        }

        public static GUIStyle GetBoxExpanded()
        {
            if (boxExpanded == null)
            {
                boxExpanded = new GUIStyle(EditorStyles.helpBox);
                boxExpanded.padding = new RectOffset(1, 1, 3, 3);
                boxExpanded.margin = new RectOffset(
                    boxExpanded.margin.left,
                    boxExpanded.margin.right,
                    0, 0
                );
            }

            return boxExpanded;
        }

        public static GUIStyle GetBox()
        {
            if (box == null)
            {
                box = new GUIStyle(EditorStyles.helpBox);
                box.margin = new RectOffset(0, 0, 0, 0);
                box.padding = new RectOffset(5, 5, 5, 5);
            }

            return box;
        }

        public static GUIStyle GetHelpBox()
        {
            if (helpBox == null)
            {
                helpBox = new GUIStyle(EditorStyles.helpBox);
                helpBox.margin = new RectOffset(0, 0, 0, 0);
                helpBox.padding = new RectOffset(0, 0, 0, 0);
            }

            return helpBox;
        }

        public static GUIStyle GetToolbarButton()
        {
            if (btnToolbar == null)
            {
                btnToolbar = new GUIStyle(EditorStyles.toolbarButton);
            }

            return btnToolbar;
        }

        public static GUIStyle GetSearchBox()
        {
            if (searchbox == null)
            {
                searchbox = new GUIStyle(GUI.skin.box);
                searchbox.margin = new RectOffset(0, 0, 0, 0);
                searchbox.padding = new RectOffset(5, 5, 5, 5);
            }

            return searchbox;
        }

        // GRID BUTTONS: --------------------------------------------------------------------------

        public static GUIStyle GetGridButtonLeftOn()
        {
            if (btnGridLeftOn == null)
            {
                btnGridLeftOn = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
                btnGridLeftOn.normal = btnGridLeftOn.onNormal;
                btnGridLeftOn.hover = btnGridLeftOn.onHover;
                btnGridLeftOn.active = btnGridLeftOn.onActive;
                btnGridLeftOn.focused = btnGridLeftOn.onFocused;
            }

            return btnGridLeftOn;
        }

        public static GUIStyle GetGridButtonLeftOff()
        {
            if (btnGridLeftOff == null)
            {
                btnGridLeftOff = new GUIStyle(GUI.skin.GetStyle("ButtonLeft"));
            }

            return btnGridLeftOff;
        }

        public static GUIStyle GetGridButtonMidOn()
        {
            if (btnGridMidOn == null)
            {
                btnGridMidOn = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
                btnGridMidOn.normal = btnGridMidOn.onNormal;
                btnGridMidOn.hover = btnGridMidOn.onHover;
                btnGridMidOn.active = btnGridMidOn.onActive;
                btnGridMidOn.focused = btnGridMidOn.onFocused;
            }

            return btnGridMidOn;
        }

        public static GUIStyle GetGridButtonMidOff()
        {
            if (btnGridMidOff == null)
            {
                btnGridMidOff = new GUIStyle(GUI.skin.GetStyle("ButtonMid"));
            }

            return btnGridMidOff;
        }

        public static GUIStyle GetGridButtonRightOn()
        {
            if (btnGridRightOn == null)
            {
                btnGridRightOn = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
                btnGridRightOn.normal = btnGridRightOn.onNormal;
                btnGridRightOn.hover = btnGridRightOn.onHover;
                btnGridRightOn.active = btnGridRightOn.onActive;
                btnGridRightOn.focused = btnGridRightOn.onFocused;
            }

            return btnGridRightOn;
        }

        public static GUIStyle GetGridButtonRightOff()
        {
            if (btnGridRightOff == null)
            {
                btnGridRightOff = new GUIStyle(GUI.skin.GetStyle("ButtonRight"));
            }

            return btnGridRightOff;
        }

        public static GUIStyle GetButtonLeftArrow()
        {
            if (btnLftArrow == null)
            {
                btnLftArrow = new GUIStyle(GUI.skin.GetStyle("AC LeftArrow"));
            }

            return btnLftArrow;
        }

        public static GUIStyle GetButtonRightArrow()
        {
            if (btnRigArrow == null)
            {
                btnRigArrow = new GUIStyle(GUI.skin.GetStyle("AC RightArrow"));
            }

            return btnRigArrow;
        }

        public static GUIStyle GetItemPreferencesSidebar()
        {
            if (itemPreferencesSidebar == null)
            {
                itemPreferencesSidebar = new GUIStyle(GUI.skin.GetStyle("MenuItem"));
                itemPreferencesSidebar.alignment = TextAnchor.MiddleRight;
                itemPreferencesSidebar.active.background = null;
                itemPreferencesSidebar.fixedHeight = 30f;
            }

            return itemPreferencesSidebar;
        }

        public static GUIStyle GetLabelTag()
        {
            if (labelTag == null)
            {
                labelTag = new GUIStyle();
                Texture2D bg = AssetDatabase.LoadAssetAtPath<Texture2D>(TAG_BG_PATH);

                labelTag.normal.background = bg;
                labelTag.padding = new RectOffset(0, 0, 0, 0);
                labelTag.margin = new RectOffset(0, 0, 0, 0);
                labelTag.fontSize = 10;
                labelTag.fontStyle = FontStyle.Normal;
                labelTag.alignment = TextAnchor.MiddleCenter;
                labelTag.normal.textColor = Color.white;
            }

            return labelTag;
        }

        public static GUIStyle GetTextarea()
        {
            if (textarea == null)
            {
                textarea = new GUIStyle(EditorStyles.textArea);
                textarea.wordWrap = true;
            }

            return labelTag;
        }

        public static GUIStyle GetDropZoneNormal()
        {
            if (dropZoneNormal == null)
            {
                dropZoneNormal = new GUIStyle(EditorStyles.helpBox);
                Texture2D bgNormal = AssetDatabase.LoadAssetAtPath<Texture2D>(DROPZONE_NORMAL_PATH);

                dropZoneNormal.normal.background = bgNormal;
                dropZoneNormal.focused.background = bgNormal;
                dropZoneNormal.active.background = bgNormal;
                dropZoneNormal.hover.background = bgNormal;

                dropZoneNormal.border = new RectOffset(4, 4, 4, 4);
                dropZoneNormal.padding = new RectOffset(0, 0, 0, 0);
                dropZoneNormal.margin = new RectOffset(0, 0, 0, 0);
                dropZoneNormal.alignment = TextAnchor.MiddleCenter;
            }

            return dropZoneNormal;
        }

        public static GUIStyle GetDropZoneActive()
        {
            if (dropZoneActive == null)
            {
                dropZoneActive = new GUIStyle(GetDropZoneNormal());
                Texture2D bgActive = AssetDatabase.LoadAssetAtPath<Texture2D>(DROPZONE_ACTIVE_PATH);

                dropZoneActive.normal.background = bgActive;
                dropZoneActive.focused.background = bgActive;
                dropZoneActive.active.background = bgActive;
                dropZoneActive.hover.background = bgActive;

                dropZoneActive.normal.textColor = Color.black;
                dropZoneActive.focused.textColor = Color.black;
                dropZoneActive.active.textColor = Color.black;
                dropZoneActive.hover.textColor = Color.black;
            }

            return dropZoneActive;
        }

        public static GUIStyle GlobalIDText()
        {
            if (globalIDText == null)
            {
                globalIDText = new GUIStyle(EditorStyles.textField);
                globalIDText.margin = GetButtonMid().margin;
                globalIDText.padding = GetButtonMid().padding;
                globalIDText.contentOffset = GetButtonMid().contentOffset;
                globalIDText.alignment = TextAnchor.MiddleCenter;
            }

            return globalIDText;
        }
    }
}