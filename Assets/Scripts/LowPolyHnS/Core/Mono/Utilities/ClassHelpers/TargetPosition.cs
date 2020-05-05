using System;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class TargetPosition
    {
        public enum Target
        {
            Player,
            Camera,
            Invoker,
            Transform,
            Position,
            LocalVariable,
            GlobalVariable,
            ListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.Position;
        public Vector3 offset = Vector3.zero;

        public Transform targetTransform = null;
        public Vector3 targetPosition = Vector3.zero;
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetPosition()
        {
        }

        public TargetPosition(Target target)
        {
            this.target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetPosition(GameObject invoker, Space offsetSpace = Space.World)
        {
            Vector3 resultPosition = Vector3.zero;
            Vector3 resultOffset = Vector3.zero;

            switch (target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null)
                    {
                        resultPosition = HookPlayer.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                resultOffset = offset;
                                break;

                            case Space.Self:
                                resultOffset = HookPlayer.Instance.transform.TransformDirection(offset);
                                break;
                        }
                    }

                    break;

                case Target.Camera:
                    if (HookCamera.Instance != null)
                    {
                        resultPosition = HookCamera.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                resultOffset = offset;
                                break;

                            case Space.Self:
                                resultOffset = HookCamera.Instance.transform.TransformDirection(offset);
                                break;
                        }
                    }

                    break;

                case Target.Invoker:
                    resultPosition = invoker.transform.position;
                    resultOffset = offset;
                    break;

                case Target.Transform:
                    if (targetTransform != null)
                    {
                        if (targetTransform != null)
                        {
                            resultPosition = targetTransform.position;
                            switch (offsetSpace)
                            {
                                case Space.World:
                                    resultOffset = offset;
                                    break;

                                case Space.Self:
                                    resultOffset = targetTransform.TransformDirection(offset);
                                    break;
                            }
                        }
                    }

                    break;

                case Target.Position:
                    resultPosition = targetPosition;
                    resultOffset = Vector3.zero;
                    break;

                case Target.LocalVariable:
                    resultOffset = Vector3.zero;
                    switch (local.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3) local.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = local.Get(invoker) as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }

                            break;
                    }

                    break;

                case Target.GlobalVariable:
                    resultOffset = Vector3.zero;
                    switch (global.GetDataType())
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3) global.Get();
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = global.Get() as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }

                            break;
                    }

                    break;

                case Target.ListVariable:
                    resultOffset = Vector3.zero;
                    switch (list.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3) list.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = list.Get(invoker) as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }

                            break;
                    }

                    break;
            }

            return resultPosition + resultOffset;
        }

        public Quaternion GetRotation(GameObject invoker)
        {
            Quaternion rotation = invoker.transform.rotation;
            switch (target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null) rotation = HookPlayer.Instance.transform.rotation;
                    break;

                case Target.Transform:
                    if (targetTransform != null) rotation = targetTransform.rotation;
                    break;

                case Target.LocalVariable:
                    if (local.GetDataType(invoker) == Variable.DataType.GameObject)
                    {
                        GameObject localResult = local.Get(invoker) as GameObject;
                        if (localResult != null)
                        {
                            rotation = localResult.transform.rotation;
                        }
                    }

                    break;

                case Target.GlobalVariable:
                    if (global.GetDataType() == Variable.DataType.GameObject)
                    {
                        GameObject globalResult = global.Get() as GameObject;
                        if (globalResult != null)
                        {
                            rotation = globalResult.transform.rotation;
                        }
                    }

                    break;

                case Target.ListVariable:
                    if (list.GetDataType(invoker) == Variable.DataType.GameObject)
                    {
                        GameObject listResult = list.Get(invoker) as GameObject;
                        if (listResult != null)
                        {
                            rotation = listResult.transform.rotation;
                        }
                    }

                    break;
            }

            return rotation;
        }

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

                case Target.Transform:
                    result = targetTransform == null
                        ? "(none)"
                        : targetTransform.gameObject.name;
                    break;

                case Target.Position:
                    result = targetPosition.ToString();
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