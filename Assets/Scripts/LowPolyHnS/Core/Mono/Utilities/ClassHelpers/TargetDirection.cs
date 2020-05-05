using System;
using LowPolyHnS.Core.Hooks;
using LowPolyHnS.Variables;
using UnityEngine;

namespace LowPolyHnS.Core
{
    [Serializable]
    public class TargetDirection
    {
        public enum Target
        {
            Player,
            Camera,
            CurrentDirection,
            Transform,
            Point,
            LocalVariable,
            GlobalVariable,
            ListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.CurrentDirection;
        public Vector3 offset = Vector3.zero;

        public Transform targetTransform = null;
        public Vector3 targetPoint = Vector3.zero;

        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetDirection()
        {
        }

        public TargetDirection(Target target)
        {
            this.target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetDirection(GameObject invoker, Space offsetSpace = Space.World)
        {
            Vector3 direction = Vector3.zero;

            switch (target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null)
                    {
                        Vector3 playerPosition = HookPlayer.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                playerPosition += offset;
                                break;

                            case Space.Self:
                                playerPosition += HookPlayer.Instance.transform.TransformDirection(offset);
                                break;
                        }

                        direction = playerPosition - invoker.transform.position;
                    }

                    break;

                case Target.Camera:
                    if (HookCamera.Instance != null)
                    {
                        Vector3 cameraPosition = HookCamera.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                cameraPosition += offset;
                                break;

                            case Space.Self:
                                cameraPosition += HookCamera.Instance.transform.TransformDirection(offset);
                                break;
                        }

                        direction = cameraPosition - invoker.transform.position;
                    }

                    break;

                case Target.CurrentDirection:
                    direction = invoker.transform.forward;
                    break;

                case Target.Transform:
                    if (targetTransform != null)
                    {
                        Vector3 transformPosition = targetTransform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                transformPosition += offset;
                                break;

                            case Space.Self:
                                transformPosition += targetTransform.TransformDirection(offset);
                                break;
                        }

                        direction = transformPosition - invoker.transform.position;
                    }

                    break;

                case Target.Point:
                    direction = targetPoint - invoker.transform.position;
                    break;

                case Target.LocalVariable:
                    Vector3 localPosition = Vector3.zero;

                    switch (local.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            localPosition = (Vector3) local.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = local.Get(invoker) as GameObject;
                            if (@object != null)
                            {
                                localPosition = @object.transform.position;
                            }

                            break;
                    }

                    direction = localPosition - invoker.transform.position;
                    break;

                case Target.GlobalVariable:
                    Vector3 globalPosition = Vector3.zero;
                    switch (global.GetDataType())
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            globalPosition = (Vector3) global.Get();
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = global.Get() as GameObject;
                            if (@object != null)
                            {
                                globalPosition = @object.transform.position;
                            }

                            break;
                    }

                    direction = globalPosition - invoker.transform.position;
                    break;

                case Target.ListVariable:
                    Vector3 listPosition = Vector3.zero;
                    switch (list.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            listPosition = (Vector3) list.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = list.Get(invoker) as GameObject;
                            if (@object != null)
                            {
                                listPosition = @object.transform.position;
                            }

                            break;
                    }

                    direction = listPosition - invoker.transform.position;
                    break;
            }

            return direction.normalized;
        }

        public override string ToString()
        {
            string result = "(unknown)";
            switch (target)
            {
                case Target.Player:
                    result = "Player";
                    break;

                case Target.Camera:
                    result = "Camera";
                    break;

                case Target.CurrentDirection:
                    result = "Current Direction";
                    break;

                case Target.Transform:
                    result = targetTransform == null
                        ? "(none)"
                        : targetTransform.gameObject.name;
                    break;

                case Target.Point:
                    result = targetPoint.ToString();
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