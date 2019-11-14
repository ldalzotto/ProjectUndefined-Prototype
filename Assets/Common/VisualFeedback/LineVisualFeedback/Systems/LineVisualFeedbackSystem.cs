using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace VisualFeedback
{
    public class LineVisualFeedbackSystem : AInteractiveObjectSystem
    {
        private IInteractiveGameObject InteractiveGameObjectRef;
        private List<ILinePositioning> linePositionings = new List<ILinePositioning>();
        private List<DottedLine> lines = new List<DottedLine>();
        private Vector3 positionOffsetFromNPC;

        private List<CoreInteractiveObject> sourceTriggeringInteractiveObjects = new List<CoreInteractiveObject>();

        public LineVisualFeedbackSystem(IInteractiveGameObject InteractiveGameObject)
        {
            InteractiveGameObjectRef = InteractiveGameObject;
            //position calculation
            positionOffsetFromNPC = Vector3.up * InteractiveGameObject.AverageModelLocalBounds.Bounds.max.y;
        }

        public override void Tick(float d)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                var startPosition = InteractiveGameObjectRef.GetTransform().WorldPosition + positionOffsetFromNPC;
                lines[i].Tick(d, startPosition, linePositionings[i].GetEndPosition(startPosition));
            }
        }

        public override void OnDestroy()
        {
            for (var i = 0; i < lines.Count; i++) DestroyLine(sourceTriggeringInteractiveObjects[i]);
        }

        #region External Events

        public void CreateLineFollowing(DottedLineID DottedLineID, CoreInteractiveObject TargetInteractiveGameObject)
        {
            sourceTriggeringInteractiveObjects.Add(TargetInteractiveGameObject);
            lines.Add(new DottedLine(DottedLineID));
            var targetGameObject = TargetInteractiveGameObject.InteractiveGameObject;
            linePositionings.Add(new LineFollowTransformPositioning(targetGameObject.InteractiveGameObjectParent.transform, targetGameObject.AverageModelLocalBounds));
        }

        /*
        public void CreateLineDirectionPositioning(DottedLineID DottedLineID, CoreInteractiveObject sourceInteractiveObject)
        {
            this.sourceTriggeringInteractiveObjects.Add(sourceInteractiveObject);
            this.lines.Add(DottedLine.CreateInstance(DottedLineID, PuzzleGameSingletonInstances.PuzzleGameConfigurationManager));
            this.linePositionings.Add(new LineDirectionPositioning(this.ModelObjectModule, sourceTriggeringObject));
        }
        */

        public void DestroyLine(CoreInteractiveObject sourceInteractiveObject)
        {
            var index = sourceTriggeringInteractiveObjects.IndexOf(sourceInteractiveObject);
            if (index >= 0)
            {
                sourceTriggeringInteractiveObjects.RemoveAt(index);
                linePositionings.RemoveAt(index);
                lines[index].OnDestroy();
                lines.RemoveAt(index);
            }
        }

        #endregion
    }
}