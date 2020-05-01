using System;
using UnityEngine;

namespace LowPolyHnS.Variables
{
    [Serializable]
    public class Variable
    {
        private const bool SAVE_BY_DEFAULT = true;

        public enum VarType
        {
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        public enum DataType
        {
            Null,
            String,
            Number,
            Bool,
            Color,
            Vector2,
            Vector3,
            Texture2D,
            Sprite,
            GameObject
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public string name = "";
        public bool save = SAVE_BY_DEFAULT;
        public int type = (int) DataType.Null;
        public int tags = 0;

        [SerializeField] private VariableStr varStr = new VariableStr();
        [SerializeField] private VariableNum varNum = new VariableNum();
        [SerializeField] private VariableBol varBol = new VariableBol();
        [SerializeField] private VariableCol varCol = new VariableCol();
        [SerializeField] private VariableVc2 varVc2 = new VariableVc2();
        [SerializeField] private VariableVc3 varVc3 = new VariableVc3();
        [SerializeField] private VariableTxt varTxt = new VariableTxt();
        [SerializeField] private VariableSpr varSpr = new VariableSpr();
        [SerializeField] private VariableObj varObj = new VariableObj();

        // INITIALIZERS: --------------------------------------------------------------------------

        public Variable()
        {
        }

        public Variable(Variable variable)
        {
            name = variable.name;
            type = variable.type;
            save = variable.save;

            varStr = new VariableStr(variable.varStr.Get());
            varNum = new VariableNum(variable.varNum.Get());
            varBol = new VariableBol(variable.varBol.Get());
            varCol = new VariableCol(variable.varCol.Get());
            varVc2 = new VariableVc2(variable.varVc2.Get());
            varVc3 = new VariableVc3(variable.varVc3.Get());
            varTxt = new VariableTxt(variable.varTxt.Get());
            varSpr = new VariableSpr(variable.varSpr.Get());
            varObj = new VariableObj(variable.varObj.Get());
        }

        public Variable(string name, DataType type, object value, bool save = SAVE_BY_DEFAULT)
        {
            this.name = name;
            Set(type, value);
            this.save = save;
        }

        // GETTERS: -------------------------------------------------------------------------------

        public object Get()
        {
            switch ((DataType) type)
            {
                case DataType.String: return varStr.Get();
                case DataType.Number: return varNum.Get();
                case DataType.Bool: return varBol.Get();
                case DataType.Color: return varCol.Get();
                case DataType.Vector2: return varVc2.Get();
                case DataType.Vector3: return varVc3.Get();
                case DataType.Texture2D: return varTxt.Get();
                case DataType.Sprite: return varSpr.Get();
                case DataType.GameObject: return varObj.Get();
            }

            return null;
        }

        public T Get<T>()
        {
            return (T) Get();
        }

        public bool CanSave()
        {
            switch ((DataType) type)
            {
                case DataType.String: return varStr.CanSave();
                case DataType.Number: return varNum.CanSave();
                case DataType.Bool: return varBol.CanSave();
                case DataType.Color: return varCol.CanSave();
                case DataType.Vector2: return varVc2.CanSave();
                case DataType.Vector3: return varVc3.CanSave();
                case DataType.Texture2D: return varTxt.CanSave();
                case DataType.Sprite: return varSpr.CanSave();
                case DataType.GameObject: return varObj.CanSave();
            }

            return false;
        }

        public static bool CanSave(DataType type)
        {
            return VariableBase.CanSaveType(type);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public void Set(DataType type, object value)
        {
            this.type = (int) type;
            Update(value);
        }

        public void Update(object value)
        {
            switch ((DataType) type)
            {
                case DataType.String:
                    varStr.Set((string) value);
                    break;
                case DataType.Number:
                    varNum.Set(Convert.ToSingle(value));
                    break;
                case DataType.Bool:
                    varBol.Set((bool) value);
                    break;
                case DataType.Color:
                    varCol.Set((Color) value);
                    break;
                case DataType.Vector2:
                    varVc2.Set((Vector2) value);
                    break;
                case DataType.Vector3:
                    varVc3.Set((Vector3) value);
                    break;
                case DataType.Texture2D:
                    varTxt.Set((Texture2D) value);
                    break;
                case DataType.Sprite:
                    varSpr.Set((Sprite) value);
                    break;
                case DataType.GameObject:
                    varObj.Set((GameObject) value);
                    break;
            }
        }
    }
}