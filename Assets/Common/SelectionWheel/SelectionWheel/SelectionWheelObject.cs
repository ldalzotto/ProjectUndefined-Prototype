using System.Collections.Generic;
using CoreGame;
using Input;
using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelObject
    {
        private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
        private ActionWheelNodePositionManager ActionWheelNodePositionManager;
        private SelectionWheelObjectAnimation SelectionWheelObjectAnimation;

        private SelectionWheelPositionManager SelectionWheelPositionManager;
        private SelectionWheelNode[] wheelNodes;
        public SelectionWheelGameObject SelectionWheelGameObject { get; private set; }

        #region State

        public bool IsWheelEnabled { get; private set; }
        private Transform followingWorldTransform;

        #endregion

        public void AwakeWheel(List<SelectionWheelNodeData> wheelNodeDatas, Transform followingWorldTransform)
        {
            if (!IsWheelEnabled)
            {
                IsWheelEnabled = true;
                this.followingWorldTransform = followingWorldTransform;

                var SelectionWheelGlobalConfiguration = SelectionWheelGlobalConfigurationGameObject.Get().SelectionWheelGlobalConfiguration;

                if (SelectionWheelGameObject == null)
                {
                    SelectionWheelGameObject = new SelectionWheelGameObject(CoreGameSingletonInstances.GameCanvas);
                    SelectionWheelObjectAnimation = new SelectionWheelObjectAnimation(SelectionWheelGameObject, SelectionWheelGlobalConfiguration.SelectionWheelEnterAnimation, OnExitAnimationFinished);
                }

                SelectionWheelPositionManager = new SelectionWheelPositionManager(this, SelectionWheelGlobalConfiguration, this.followingWorldTransform);
                SelectionWheelObjectAnimation.PlayEnterAnimation();
                ActionWheelActiveNodeManager = new ActionWheelActiveNodeManager(SelectionWheelGlobalConfiguration.NonSelectedMaterial, SelectionWheelGlobalConfiguration.SelectedMaterial);
                ActionWheelNodePositionManager = new ActionWheelNodePositionManager(SelectionWheelGlobalConfiguration.ActionWheelNodePositionManagerComponent, GameInputManager.Get(), ActionWheelActiveNodeManager);
                wheelNodes = new SelectionWheelNode[wheelNodeDatas.Count];
                for (var i = 0; i < wheelNodeDatas.Count; i++)
                {
                    var wheelNode = SelectionWheelNode.Instantiate(wheelNodeDatas[i]);
                    wheelNode.transform.SetParent(SelectionWheelGameObject.SelectionWheelNodeContainerGameObject.transform, false);
                    wheelNodes[i] = wheelNode;
                }

                ActionWheelNodePositionManager.InitNodes(wheelNodes);
                ActionWheelActiveNodeManager.SelectedNodeChanged(wheelNodes);
            }
        }

        public void SleepWheel(bool destroyImmediate = false)
        {
            if (IsWheelEnabled)
            {
                IsWheelEnabled = false;
                this.followingWorldTransform = null;
                SelectionWheelObjectAnimation.PlayExitAnimation();
                if (destroyImmediate) OnExitAnimationFinished();
            }
        }

        public void RefreshWheel(List<SelectionWheelNodeData> wheelNodeDatas)
        {
            if (IsWheelEnabled)
            {
                var currentFollowingWorldTransform = this.followingWorldTransform;
                this.SleepWheel(true);
                this.AwakeWheel(wheelNodeDatas, currentFollowingWorldTransform);
            }
        }

        private void OnExitAnimationFinished()
        {
            foreach (Transform child in SelectionWheelGameObject.SelectionWheelNodeContainerGameObject.transform)
                MonoBehaviour.Destroy(child.gameObject);
        }

        public void Tick(float d)
        {
            if (IsWheelEnabled)
            {
                SelectionWheelPositionManager.Tick(d);
                ActionWheelNodePositionManager.Tick(d, wheelNodes);

                foreach (var wheelNode in wheelNodes)
                    wheelNode.Tick(d);
            }
        }

        public void LateTick(float d)
        {
            if (SelectionWheelObjectAnimation != null) SelectionWheelObjectAnimation.LateTick(d);
        }

        public SelectionWheelNodeData GetSelectedNodeData()
        {
            if (ActionWheelActiveNodeManager.ActiveNode != null) return ActionWheelActiveNodeManager.ActiveNode.WheelNodeData;

            return null;
        }
    }

    #region Wheel Position

    internal class SelectionWheelPositionManager
    {
        private Transform FollowWorldTransform;
        private SelectionWheelGlobalConfiguration SelectionWheelGlobalConfigurationRef;
        private SelectionWheelObject SelectionWheelObjectRef;

        public SelectionWheelPositionManager(SelectionWheelObject selectionWheelObjectRef, SelectionWheelGlobalConfiguration selectionWheelGlobalConfigurationRef, Transform FollowWorldTransform)
        {
            SelectionWheelObjectRef = selectionWheelObjectRef;
            SelectionWheelGlobalConfigurationRef = selectionWheelGlobalConfigurationRef;
            this.FollowWorldTransform = FollowWorldTransform;
        }

        public void Tick(float d)
        {
            SelectionWheelObjectRef.SelectionWheelGameObject.SetTransformPosition
                (Camera.main.WorldToScreenPoint(FollowWorldTransform.position) + new Vector3(0, SelectionWheelGlobalConfigurationRef.ActionWheelNodePositionManagerComponent.DistanceFromCenter, 0));
        }
    }

    #endregion

    #region Node position

    internal class ActionWheelNodePositionManager
    {
        private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
        private ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;
        private GameInputManager GameInputManager;

        private bool isRotating = false;

        public ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent actionWheelNodePositionManagerComponent, GameInputManager gameInputManager, ActionWheelActiveNodeManager actionWheelActiveNodeManager)
        {
            ActionWheelNodePositionManagerComponent = actionWheelNodePositionManagerComponent;
            GameInputManager = gameInputManager;
            ActionWheelActiveNodeManager = actionWheelActiveNodeManager;
        }

        public void Tick(float d, SelectionWheelNode[] wheelActionNodes)
        {
            if (wheelActionNodes.Length > 1)
            {
                if (!isRotating)
                {
                    var joystickAxis = GameInputManager.CurrentInput.LocomotionAxis();
                    if (joystickAxis.x >= 0.5)
                    {
                        isRotating = true;
                        for (var i = 0; i < wheelActionNodes.Length; i++) wheelActionNodes[i].TargetWheelAngleDeg += 360 / wheelActionNodes.Length;

                        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                    }
                    else if (joystickAxis.x <= -0.5)
                    {
                        isRotating = true;
                        for (var i = 0; i < wheelActionNodes.Length; i++) wheelActionNodes[i].TargetWheelAngleDeg -= 360 / wheelActionNodes.Length;

                        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                    }
                }

                if (RepositionNodesSmooth(wheelActionNodes, d)) isRotating = false;
            }
        }

        private bool RepositionNodesSmooth(SelectionWheelNode[] wheelActionNodes, float d)
        {
            for (var i = 0; i < wheelActionNodes.Length; i++)
            {
                wheelActionNodes[i].CurrentAngleDeg = Mathf.Lerp(wheelActionNodes[i].CurrentAngleDeg, wheelActionNodes[i].TargetWheelAngleDeg, d * ActionWheelNodePositionManagerComponent.RotationSpeed);
                var nodePosition = Vector3.up;
                nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].CurrentAngleDeg) * nodePosition;
                nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
                wheelActionNodes[i].transform.localPosition = nodePosition;
            }

            if (Mathf.Abs(wheelActionNodes[0].TargetWheelAngleDeg - wheelActionNodes[0].CurrentAngleDeg) < 5)
                return true;
            else
                return false;
        }

        public void InitNodes(SelectionWheelNode[] wheelActionNodes)
        {
            for (var i = 0; i < wheelActionNodes.Length; i++)
            {
                wheelActionNodes[i].TargetWheelAngleDeg = 360 / wheelActionNodes.Length * i;
                wheelActionNodes[i].CurrentAngleDeg = wheelActionNodes[i].TargetWheelAngleDeg;
                var nodePosition = Vector3.up;
                nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].TargetWheelAngleDeg) * nodePosition;
                nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
                wheelActionNodes[i].transform.localPosition = nodePosition;
            }
        }
    }


    internal class ActionWheelActiveNodeManager
    {
        private SelectionWheelNode activeNode;

        private Material nonSelectedMaterial;
        private Material selectedMaterial;

        public ActionWheelActiveNodeManager(Material nonSelectedMaterial, Material selectedMaterial)
        {
            this.nonSelectedMaterial = nonSelectedMaterial;
            this.selectedMaterial = selectedMaterial;
        }

        public SelectionWheelNode ActiveNode => activeNode;

        public void SelectedNodeChanged(SelectionWheelNode[] wheelActionNodes)
        {
            if (activeNode != null)
            {
                activeNode.SetMaterial(nonSelectedMaterial);
                activeNode.SetActiveText(false);
            }

            for (var i = 0; i < wheelActionNodes.Length; i++)
                if (wheelActionNodes[i].TargetWheelAngleDeg % 360 == 0)
                {
                    activeNode = wheelActionNodes[i];
                    activeNode.SetMaterial(selectedMaterial);
                    activeNode.SetActiveText(true);
                    return;
                }
        }
    }

    #endregion
}