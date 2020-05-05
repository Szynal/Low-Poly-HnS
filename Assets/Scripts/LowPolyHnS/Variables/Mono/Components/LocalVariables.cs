using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Variables
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [AddComponentMenu("LowPolyHnS/Local Variables")]
    public class LocalVariables : GlobalID, IGameSave
    {
        public static Dictionary<string, LocalVariables> REGISTER = new Dictionary<string, LocalVariables>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public MBVariable[] references = new MBVariable[0];

        protected bool initalized;
        private Dictionary<string, Variable> variables;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            Initialize();
            if (variables != null && variables.ContainsKey(name))
            {
                return variables[name];
            }

            return null;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            Initialize();
            SaveLoadManager.Instance.Initialize(this);
        }

        protected void Initialize(bool force = false)
        {
            string gid = GetID();
            if (!REGISTER.ContainsKey(gid))
            {
                REGISTER.Add(gid, this);
            }

            RequireInit(force);
        }

        protected virtual void OnDestroy()
        {
            OnDestroyGID();
            if (!Application.isPlaying) return;
            if (exitingApplication) return;

            string gid = GetID();
            if (REGISTER.ContainsKey(gid))
            {
                REGISTER.Remove(gid);
            }

            if (SaveLoadManager.IS_EXITING) return;
            SaveLoadManager.Instance.OnDestroyIGameSave(this);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            SerializedProperty spReferences = null;
            for (int i = 0; i < references.Length; ++i)
            {
                MBVariable reference = references[i];
                if (reference != null && reference.gameObject != gameObject)
                {
                    MBVariable newVariable = gameObject.AddComponent<MBVariable>();
                    EditorUtility.CopySerialized(reference, newVariable);

                    if (spReferences == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spReferences = serializedObject.FindProperty("references");
                    }

                    spReferences.GetArrayElementAtIndex(i).objectReferenceValue = newVariable;
                }
            }

            if (spReferences != null) spReferences.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected virtual void RequireInit(bool force = false)
        {
            if (initalized && !force) return;
            initalized = true;

            variables = new Dictionary<string, Variable>();
            for (int i = 0; i < references.Length; ++i)
            {
                Variable variable = references[i].variable;
                if (variable == null) continue;

                string variableName = variable.name;
                if (!variables.ContainsKey(variableName))
                {
                    variables.Add(variableName, new Variable(variable));
                }
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public virtual string GetUniqueName()
        {
            string uniqueName = string.Format(
                "variables:local:{0}",
                GetID()
            );

            return uniqueName;
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public virtual object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();
            if (variables == null || variables.Count == 0)
            {
                return container;
            }

            foreach (KeyValuePair<string, Variable> item in variables)
            {
                if (item.Value != null && item.Value.CanSave() && item.Value.save)
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public virtual void ResetData()
        {
            RequireInit(true);
        }

        public virtual void OnLoad(object generic)
        {
            if (generic == null) return;

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
                        VariablesManager.events.OnChangeLocal(gameObject, varName);
                    }
                }
            }
        }
    }
}