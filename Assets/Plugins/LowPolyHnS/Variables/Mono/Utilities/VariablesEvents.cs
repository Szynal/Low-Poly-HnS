using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Variables
{
    public class VariablesEvents
    {
        public class VarEvent : UnityEvent<string>
        {
        }

        public class ListEvent : UnityEvent<int, object, object>
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, VarEvent> onVariableChange = new Dictionary<string, VarEvent>();

        private Dictionary<string, ListEvent> onListAny = new Dictionary<string, ListEvent>();
        private Dictionary<string, ListEvent> onListChg = new Dictionary<string, ListEvent>();
        private Dictionary<string, ListEvent> onListAdd = new Dictionary<string, ListEvent>();
        private Dictionary<string, ListEvent> onListRmv = new Dictionary<string, ListEvent>();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariablesEvents()
        {
            onVariableChange = new Dictionary<string, VarEvent>();

            onListAny = new Dictionary<string, ListEvent>();
            onListChg = new Dictionary<string, ListEvent>();
            onListAdd = new Dictionary<string, ListEvent>();
            onListRmv = new Dictionary<string, ListEvent>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnChangeGlobal(string variableID)
        {
            if (!onVariableChange.ContainsKey(variableID)) return;
            VarEvent varEvent = onVariableChange[variableID];
            if (varEvent != null) varEvent.Invoke(variableID);
        }

        public void OnChangeLocal(GameObject gameObject, string variableID)
        {
            if (gameObject == null) return;

            string localID = GetLocalID(gameObject, variableID);
            if (!onVariableChange.ContainsKey(localID)) return;

            VarEvent varEvent = onVariableChange[localID];
            if (varEvent != null) varEvent.Invoke(variableID);
        }

        public void OnListChange(GameObject gameObject, int index, object prevElem, object newElem)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            ListEvent listEvent = null;

            if (onListChg.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, prevElem, newElem);
            }

            if (onListAny.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, prevElem, newElem);
            }
        }

        public void OnListAdd(GameObject gameObject, int index, object newElem)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            ListEvent listEvent = null;

            if (onListAdd.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, null, newElem);
            }

            if (onListAny.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, null, newElem);
            }
        }

        public void OnListRemove(GameObject gameObject, int index, object prevElem)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            ListEvent listEvent = null;

            if (onListRmv.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, prevElem, null);
            }

            if (onListAny.TryGetValue(listID, out listEvent) && listEvent != null)
            {
                listEvent.Invoke(index, prevElem, null);
            }
        }

        // SETTERS: -------------------------------------------------------------------------------

        public void SetOnChangeGlobal(UnityAction<string> action, string variableID)
        {
            if (!onVariableChange.ContainsKey(variableID))
            {
                onVariableChange.Add(variableID, new VarEvent());
            }

            onVariableChange[variableID].AddListener(action);
        }

        public void SetOnChangeLocal(UnityAction<string> action, GameObject gameObject, string variableID)
        {
            if (gameObject == null) return;

            string localID = GetLocalID(gameObject, variableID);
            if (!onVariableChange.ContainsKey(localID))
            {
                onVariableChange.Add(localID, new VarEvent());
            }

            onVariableChange[localID].AddListener(action);
        }

        /// <summary>
        ///     Start listening for any List Variable change: Adding, Removing and Changing an element
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="gameObject">Game object.</param>
        public void StartListenListAny(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            if (!onListAny.ContainsKey(listID))
            {
                onListAny.Add(listID, new ListEvent());
            }

            onListAny[listID].AddListener(action);
        }

        /// <summary>
        ///     Start listening for a change in a List Variable element
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="gameObject">List Variable.</param>
        public void StartListenListChg(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            if (!onListChg.ContainsKey(listID))
            {
                onListChg.Add(listID, new ListEvent());
            }

            onListChg[listID].AddListener(action);
        }

        /// <summary>
        ///     Start listening for elements being added to the List Variables
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="gameObject">Game object.</param>
        public void StartListenListAdd(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            if (!onListAdd.ContainsKey(listID))
            {
                onListAdd.Add(listID, new ListEvent());
            }

            onListAdd[listID].AddListener(action);
        }

        /// <summary>
        ///     Start listening for elements being removed from the List Variables
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="gameObject">Game object.</param>
        public void StartListenListRmv(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;

            string listID = GetListID(gameObject);
            if (!onListRmv.ContainsKey(listID))
            {
                onListRmv.Add(listID, new ListEvent());
            }

            onListRmv[listID].AddListener(action);
        }

        // REMOVERS: ------------------------------------------------------------------------------

        public void RemoveChangeGlobal(UnityAction<string> action, string variableID)
        {
            if (!onVariableChange.ContainsKey(variableID)) return;
            onVariableChange[variableID].RemoveListener(action);
        }

        public void RemoveChangeLocal(UnityAction<string> action, GameObject gameObject, string variableID)
        {
            if (gameObject == null) return;

            string localID = GetLocalID(gameObject, variableID);
            if (!onVariableChange.ContainsKey(localID)) return;
            onVariableChange[localID].RemoveListener(action);
        }

        public void StopListenListAny(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;
            string listID = GetListID(gameObject);

            if (!onListAny.ContainsKey(listID)) return;
            onListAny[listID].RemoveListener(action);
        }

        public void StopListenListChg(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;
            string listID = GetListID(gameObject);

            if (!onListChg.ContainsKey(listID)) return;
            onListChg[listID].RemoveListener(action);
        }

        public void StopListenListAdd(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;
            string listID = GetListID(gameObject);

            if (!onListAdd.ContainsKey(listID)) return;
            onListAdd[listID].RemoveListener(action);
        }

        public void StopListenListRmv(UnityAction<int, object, object> action, GameObject gameObject)
        {
            if (gameObject == null) return;
            string listID = GetListID(gameObject);

            if (!onListRmv.ContainsKey(listID)) return;
            onListRmv[listID].RemoveListener(action);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private string GetLocalID(GameObject gameObject, string variableID)
        {
            return string.Format("local:{0}:{1}", gameObject.GetInstanceID(), variableID);
        }

        private string GetListID(GameObject gameObject)
        {
            return string.Format("list:{0}", gameObject.GetInstanceID());
        }
    }
}