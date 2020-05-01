using System;
using System.Collections.Generic;
using LowPolyHnS.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LowPolyHnS.Variables
{
    [DisallowMultipleComponent]
    [AddComponentMenu("LowPolyHnS/List Variables")]
    public class ListVariables : LocalVariables
    {
        public enum Position
        {
            Index,
            First,
            Last,
            Previous,
            Current,
            Next,
            Random
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool save = true;
        public Variable.DataType type = Variable.DataType.GameObject;

        public List<Variable> variables;
        public int iterator { get; private set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Start()
        {
            if (!Application.isPlaying) return;

            Initialize(true);
            if (save) SaveLoadManager.Instance.Initialize(this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(int index)
        {
            Initialize();
            if (index < 0 || index >= variables.Count) return null;
            return variables[index];
        }

        public Variable Get(Position position, int index = 0)
        {
            Initialize();
            switch (position)
            {
                case Position.Index: return Get(index);
                case Position.First: return Get(0);
                case Position.Last: return Get(variables.Count - 1);
                case Position.Current: return Get(iterator);
                case Position.Next:
                    NextIterator();
                    return Get(iterator);

                case Position.Previous:
                    PrevIterator();
                    return Get(iterator);

                case Position.Random:
                    int randomIndex = Random.Range(0, variables.Count);
                    return Get(randomIndex);
            }

            return null;
        }

        public void Push(object value, int index)
        {
            Initialize();
            Variable variable = new Variable(
                Guid.NewGuid().ToString("N"),
                type,
                value,
                save
            );

            index = Math.Max(index, 0);
            index = Math.Min(index, variables.Count);
            variables.Insert(index, variable);

            VariablesManager.events.OnListAdd(gameObject, index, value);
        }

        public void Push(object value, Position position = Position.Last, int index = 0)
        {
            Initialize();
            switch (position)
            {
                case Position.Index:
                    Push(value, index);
                    break;

                case Position.First:
                    Push(value, 0);
                    break;

                case Position.Last:
                    Push(value, variables.Count);
                    break;

                case Position.Current:
                    Push(value, iterator);
                    break;

                case Position.Next:
                    Push(value, iterator + 1);
                    break;

                case Position.Previous:
                    Push(value, iterator - 1);
                    break;

                case Position.Random:
                    int random = Random.Range(0, variables.Count);
                    Push(value, random);
                    break;
            }
        }

        public void Remove(int index)
        {
            Initialize();
            if (index < 0 || index >= variables.Count) return;

            object value = variables[index].Get();
            variables.RemoveAt(index);

            VariablesManager.events.OnListRemove(gameObject, iterator, value);
        }

        public void Remove(Position position = Position.First, int index = 0)
        {
            Initialize();
            switch (position)
            {
                case Position.Index:
                    Remove(index);
                    break;

                case Position.First:
                    Remove(0);
                    break;

                case Position.Last:
                    Remove(variables.Count - 1);
                    break;

                case Position.Current:
                    Remove(iterator);
                    break;

                case Position.Next:
                    Remove(iterator + 1);
                    break;

                case Position.Previous:
                    Remove(iterator - 1);
                    break;

                case Position.Random:
                    int random = Random.Range(0, variables.Count);
                    Remove(random);
                    break;
            }
        }

        public void SetInterator(int value)
        {
            iterator = Mathf.Clamp(
                value,
                0, variables.Count - 1
            );
        }

        public void NextIterator()
        {
            iterator += 1;
            if (iterator >= variables.Count) iterator = 0;
        }

        public void PrevIterator()
        {
            iterator -= 1;
            if (iterator < 0) iterator = variables.Count - 1;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected override void RequireInit(bool force = false)
        {
            if (initalized && !force) return;
            initalized = true;

            variables = new List<Variable>();
            for (int i = 0; i < references.Length; ++i)
            {
                Variable variable = references[i].variable;
                if (variable == null) continue;

                variables.Add(new Variable(variable));
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override string GetUniqueName()
        {
            string uniqueName = string.Format(
                "variables:list:{0}",
                GetID()
            );

            return uniqueName;
        }

        public override object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();
            if (variables == null || variables.Count == 0)
            {
                return container;
            }

            foreach (Variable item in variables)
            {
                if (save && item != null && item.CanSave())
                {
                    container.variables.Add(item);
                }
            }

            return container;
        }

        public override void ResetData()
        {
            RequireInit(true);
        }

        public override void OnLoad(object generic)
        {
            if (generic == null) return;
            if (!save) return;

            DatabaseVariables.Container container = (DatabaseVariables.Container) generic;
            while (variables.Count > 0) Remove();

            for (int i = 0; i < container.variables.Count; ++i)
            {
                Push(container.variables[i].Get(), i);
            }
        }
    }
}