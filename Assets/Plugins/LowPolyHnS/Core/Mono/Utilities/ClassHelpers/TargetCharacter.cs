using System;
using LowPolyHnS.Characters;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class TargetCharacter
    {
        public enum Target
        {
            Player,
            Invoker,
            Character,
            LocalVariable,
            GlobalVariable,
            ListVariable
        }

        [Serializable]
        public class ChangeEvent : UnityEvent
        {
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.Character;
        public Character character;
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        private int cacheInstanceID;
        private int cacheCharacterID;
        private Character cacheCharacter;

        public ChangeEvent eventChangeVariable = new ChangeEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetCharacter()
        {
        }

        public TargetCharacter(Target target)
        {
            this.target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Character GetCharacter(GameObject invoker)
        {
            switch (target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null) cacheCharacter = HookPlayer.Instance.Get<Character>();
                    break;

                case Target.Invoker:
                    if (invoker == null)
                    {
                        cacheCharacter = null;
                        break;
                    }

                    if (cacheCharacter == null || invoker.GetInstanceID() != cacheCharacterID)
                    {
                        cacheCharacter = invoker.GetComponentInChildren<Character>();
                        cacheCharacterID = invoker.GetInstanceID();
                    }

                    if (cacheCharacter == null || invoker.GetInstanceID() != cacheCharacterID)
                    {
                        cacheCharacter = invoker.GetComponentInParent<Character>();
                        cacheCharacterID = invoker.GetInstanceID();
                    }

                    break;

                case Target.Character:
                    if (character != null) cacheCharacter = character;
                    break;

                case Target.LocalVariable:
                    GameObject localResult = local.Get(invoker) as GameObject;
                    if (localResult != null && localResult.GetInstanceID() != cacheInstanceID)
                    {
                        cacheCharacter = localResult.GetComponentInChildren<Character>();
                        if (cacheCharacter == null) localResult.GetComponentInParent<Character>();
                    }

                    break;

                case Target.GlobalVariable:
                    GameObject globalResult = global.Get(invoker) as GameObject;
                    if (globalResult != null && globalResult.GetInstanceID() != cacheInstanceID)
                    {
                        cacheCharacter = globalResult.GetComponentInChildren<Character>();
                        if (cacheCharacter == null) globalResult.GetComponentInParent<Character>();
                    }

                    break;

                case Target.ListVariable:
                    GameObject listResult = list.Get(invoker) as GameObject;
                    if (listResult != null && listResult.GetInstanceID() != cacheInstanceID)
                    {
                        cacheCharacter = listResult.GetComponentInChildren<Character>();
                        if (cacheCharacter == null) listResult.GetComponentInParent<Character>();
                    }

                    break;
            }

            cacheInstanceID = cacheCharacter == null
                ? 0
                : cacheCharacter.gameObject.GetInstanceID();

            return cacheCharacter;
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
                case Target.Character:
                    result = character == null
                        ? "(none)"
                        : character.gameObject.name;
                    break;
                case Target.LocalVariable:
                    result = local.ToString();
                    break;
                case Target.GlobalVariable:
                    result = global.ToString();
                    break;
                case Target.ListVariable:
                    result = list.ToString();
                    break;
            }

            return result;
        }
    }
}