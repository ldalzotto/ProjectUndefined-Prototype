using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects_Interfaces;
using UnityEditor;
using UnityEngine;

namespace AIObjects_Editor
{
    [CustomEditor(typeof(RuntimePatrolGraphNodes))]
    public class RuntimePatrolGraphNodesCustomEditor : Editor
    {
        private void OnEnable()
        {
            SceneView.duringSceneGui += this.SceneTick;
            Debug.Log("RuntimePatrolGraphNodesCustomEditor enabled");
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.SceneTick;
        }

        private void OnDestroy()
        {
            this.OnDisable();
        }

        private List<PatrolGraphNode> ProcessedPatrolGraphNodeBuffer = new List<PatrolGraphNode>(10);

        private void SceneTick(SceneView SceneView)
        {
            var RuntimePatrolGraphNodes = target as RuntimePatrolGraphNodes;
            foreach (var patrolGraphNode in RuntimePatrolGraphNodes.PatrolGraphNodes)
            {
                DrawnNodeWithLinks(patrolGraphNode);
            }

            // DrawArrow
            this.ProcessedPatrolGraphNodeBuffer.Clear();
        }

        private void DrawnNodeWithLinks(PatrolGraphNode PatrolGraphNode)
        {
            if (!this.ProcessedPatrolGraphNodeBuffer.Contains(PatrolGraphNode))
            {
                Handles.DrawWireDisc(PatrolGraphNode.WorldPosition, Vector3.up, 1);
                Handles.Label(PatrolGraphNode.WorldPosition + new Vector3(0, 1, 0), PatrolGraphNode.name, MyEditorStyles.LabelWhite);

                if (PatrolGraphNode.WorldRotationEnabled)
                {
                    HandlesHelper.DrawArrow(PatrolGraphNode.WorldPosition, PatrolGraphNode.WorldPosition + (Quaternion.Euler(PatrolGraphNode.WorldRotation) * Vector3.forward),
                        Color.yellow);
                }

                foreach (var patrolGraphNodeLink in PatrolGraphNode.Links)
                {
                    Color linkColor = Color.black;
                    if (patrolGraphNodeLink.AIMovementSpeed == AIMovementSpeedAttenuationFactor.WALK)
                    {
                        linkColor = Color.green;
                    }
                    else if (patrolGraphNodeLink.AIMovementSpeed == AIMovementSpeedAttenuationFactor.RUN)
                    {
                        linkColor = Color.red;
                    }

                    HandlesHelper.DrawArrow(PatrolGraphNode.WorldPosition, patrolGraphNodeLink.PatrolGraphNode.WorldPosition, linkColor);
                }

                this.ProcessedPatrolGraphNodeBuffer.Add(PatrolGraphNode);

                foreach (var patrolGraphNodeLink in PatrolGraphNode.Links)
                {
                    DrawnNodeWithLinks(patrolGraphNodeLink.PatrolGraphNode);
                }
            }
        }
    }
}