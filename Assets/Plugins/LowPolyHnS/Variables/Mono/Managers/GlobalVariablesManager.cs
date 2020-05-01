using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [AddComponentMenu("")]
    public class GlobalVariablesManager : Singleton<GlobalVariablesManager>, IGameSave
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, Variable> variables;
        private bool igamesaveInitialized;

        // INITIALIZERS: --------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoad()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireInit();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            RequireInit();
            if (variables.ContainsKey(name))
            {
                return variables[name];
            }

            return null;
        }

        public string[] GetNames()
        {
            RequireInit();

            string[] names = new string[variables.Count];
            int index = 0;

            foreach (KeyValuePair<string, Variable> item in variables)
            {
                names[index] = item.Key;
                index += 1;
            }

            return names;
        }

        private void RequireInit(bool force = false)
        {
            if (variables != null && !force) return;

            if (!igamesaveInitialized)
            {
                SaveLoadManager.Instance.Initialize(this);
                igamesaveInitialized = true;
            }

            DatabaseVariables database = IDatabase.LoadDatabaseCopy<DatabaseVariables>();
            GlobalVariables globalVariables = database.GetGlobalVariables();

            variables = new Dictionary<string, Variable>();
            if (globalVariables == null) return;

            for (int i = 0; i < globalVariables.references.Length; ++i)
            {
                Variable variable = Instantiate(globalVariables.references[i]).variable;

                if (variable == null) continue;
                string variableName = variable.name;

                if (!variables.ContainsKey(variableName))
                {
                    variables.Add(variableName, variable);
                }
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            return "variables:global";
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();

            foreach (KeyValuePair<string, Variable> item in variables)
            {
                if (item.Value.CanSave() && item.Value.save)
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public void ResetData()
        {
            RequireInit(true);
        }

        public void OnLoad(object generic)
        {
            RequireInit();

            DatabaseVariables.Container container = (DatabaseVariables.Container) generic;
            int variablesContainerCount = container.variables.Count;

            for (int i = 0; i < variablesContainerCount; ++i)
            {
                Variable variablesContainerVariable = container.variables[i];
                string varName = variablesContainerVariable.name;

                if (variables.ContainsKey(varName) && variables[varName].CanSave() &&
                    variables[varName].save)
                {
                    if (variables[varName].Get() != variablesContainerVariable.Get())
                    {
                        variables[varName] = variablesContainerVariable;
                        VariablesManager.events.OnChangeGlobal(varName);
                    }
                }
            }
        }
    }
}