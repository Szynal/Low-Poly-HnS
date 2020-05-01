using UnityEngine;

namespace LowPolyHnS.Core
{
    public class EditorSortableList
    {
        public class SwapIndexes
        {
            public int src;
            public int dst;
        }

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private bool sortActions;
        private bool isDragging;

        private int actionDragDstIndex = -1;
        private int actionDragSrcIndex = -1;

        private Vector2 dragMousePosition = Vector2.zero;

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public bool CaptureSortEvents(Rect handleRect, int index)
        {
            bool forceRepaint = false;

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (handleRect.Contains(Event.current.mousePosition))
                    {
                        isDragging = true;
                        actionDragDstIndex = index;
                        actionDragSrcIndex = index;
                        dragMousePosition = Event.current.mousePosition;
                        forceRepaint = true;
                    }

                    break;

                case EventType.MouseDrag:
                    if (!isDragging) break;
                    dragMousePosition = Event.current.mousePosition;
                    forceRepaint = true;
                    break;

                case EventType.MouseUp:
                    if (!isDragging) break;
                    sortActions = true;
                    isDragging = false;
                    forceRepaint = true;
                    break;
            }

            return forceRepaint;
        }

        public void PaintDropPoints(Rect rect, int index, int arraySize)
        {
            if (isDragging)
            {
                Rect upperRect = GetUpperDropRect(rect);
                if (upperRect.Contains(dragMousePosition))
                {
                    if (actionDragSrcIndex < index) actionDragDstIndex = index - 1;
                    else actionDragDstIndex = index;
                    PaintDropMarker(upperRect);
                }

                if (index >= arraySize - 1)
                {
                    Rect lowerRect = GetLowerDropRect(rect);
                    if (lowerRect.Contains(dragMousePosition))
                    {
                        if (actionDragSrcIndex < index + 1) actionDragDstIndex = index;
                        else actionDragDstIndex = index + 1;
                        PaintDropMarker(lowerRect);
                    }
                }
            }
        }

        public SwapIndexes GetSortIndexes()
        {
            SwapIndexes result = null;
            if (sortActions && actionDragSrcIndex >= 0 && actionDragDstIndex >= 0)
            {
                result = new SwapIndexes
                {
                    src = actionDragSrcIndex,
                    dst = actionDragDstIndex
                };
            }

            sortActions = false;
            return result;
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private Rect GetUpperDropRect(Rect boundaries)
        {
            return new Rect(
                boundaries.x - 5f,
                boundaries.y - 9f,
                boundaries.width,
                18f
            );
        }

        private Rect GetLowerDropRect(Rect boundaries)
        {
            Rect upperRect = GetUpperDropRect(boundaries);
            return new Rect(
                upperRect.x,
                upperRect.y + boundaries.height,
                upperRect.width,
                upperRect.height
            );
        }

        private void PaintDropMarker(Rect rect)
        {
            GUI.BeginGroup(rect);
            GUI.BeginGroup(new Rect(5f, 9f, rect.width, 9f), CoreGUIStyles.GetDropMarker());
            GUI.EndGroup();
            GUI.EndGroup();
        }
    }
}