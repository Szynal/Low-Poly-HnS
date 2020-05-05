using System;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class TargetGameObject
    {
        public enum Target
        {
            Player,
            Camera,
            Invoker,
            GameObject,
            LocalVariable,
            ListVariable,
            GlobalVariable
        }

        [Serializable]
        public class ChangeEvent : UnityEvent
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.GameObject;

        public GameObject gameObject;
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        public ChangeEvent eventChangeVariable = new ChangeEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetGameObject()
        {
        }

        public TargetGameObject(Target target)
        {
            this.target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject GetGameObject(GameObject invoker)
        {
            GameObject result = null;

            switch (target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null) result = HookPlayer.Instance.gameObject;
                    break;

                case Target.Camera:
                    if (HookCamera.Instance != null) result = HookCamera.Instance.gameObject;
                    break;

                case Target.Invoker:
                    result = invoker;
                    break;

                case Target.GameObject:
                    result = gameObject;
                    break;

                case Target.ListVariable:
                    result = list.Get(invoker) as GameObject;
                    break;

                case Target.LocalVariable:
                    result = local.Get(invoker) as GameObject;
                    break;

                case Target.GlobalVariable:
                    result = global.Get(invoker) as GameObject;
                    break;
            }

            return result;
        }

        public Transform GetTransform(GameObject invoker)
        {
            GameObject targetGo = GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.transform;
        }

        public T GetComponent<T>(GameObject invoker) where T : Object
        {
            GameObject targetGo = GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponent<T>();
        }

        public object GetComponent(GameObject invoker, string type)
        {
            GameObject targetGo = GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponent(type);
        }

        public T GetComponentInChildren<T>(GameObject invoker) where T : Object
        {
            GameObject targetGo = GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponentInChildren<T>();
        }

        public T[] GetComponentsInChildren<T>(GameObject invoker) where T : Object
        {
            GameObject targetGo = GetGameObject(invoker);
            if (targetGo == null) return new T[0];
            return targetGo.GetComponentsInChildren<T>();
        }

        // EVENTS: --------------------------------------------------------------------------------

        public void StartListeningVariableChanges(GameObject invoker)
        {
            switch (target)
            {
                case Target.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        OnChangeVariable,
                        global.name
                    );
                    break;

                case Target.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        OnChangeVariable,
                        local.GetGameObject(invoker),
                        local.name
                    );
                    break;

                case Target.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        OnChangeVariable,
                        list.GetListVariables(invoker).gameObject
                    );
                    break;
            }
        }

        public void StopListeningVariableChanges(GameObject invoker)
        {
            switch (target)
            {
                case Target.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        OnChangeVariable,
                        global.name
                    );
                    break;

                case Target.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        OnChangeVariable,
                        local.GetGameObject(invoker),
                        local.name
                    );
                    break;

                case Target.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        OnChangeVariable,
                        list.GetListVariables(invoker).gameObject
                    );
                    break;
            }
        }

        private void OnChangeVariable(string variableID)
        {
            eventChangeVariable.Invoke();
        }

        private void OnChangeVariable(int index, object prev, object next)
        {
            eventChangeVariable.Invoke();
        }

        // UTILITIES: -----------------------------------------------------------------------------

        private const string NAME_LOCAL = "local[{0}]";
        private const string NAME_GLOBAL = "global[{0}]";

        public override string ToString()
        {
            string result = "(unknown)";
            switch (target)
            {
                case Target.Player:
                    result = "Player";
                    break;
                case Target.Invoker:
                    result = "Invoker";
                    break;
                case Target.Camera:
                    result = "Camera";
                    break;
                case Target.GameObject:
                    result = gameObject != null ? gameObject.name : "(null)";
                    break;

                case Target.LocalVariable:
                    result = string.Format(NAME_LOCAL, local.name);
                    break;

                case Target.ListVariable:
                    result = list.ToString();
                    break;

                case Target.GlobalVariable:
                    result = string.Format(NAME_GLOBAL, global.name);
                    break;
            }

            return result;
        }
    }
}