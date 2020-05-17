using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObject_Animation;
using RTPuzzle;
using SequencedAction;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public class RootAnimationCutsceneTemplate : LocalPuzzleCutsceneTemplate
    {
        [WireCircleWorld(R = 0f, G = 1f, B = 0f, UseTransform = false, PositionFieldName = nameof(RootAnimationCutsceneTemplate.StartPoint))] [WireArrow(SourceFieldName = nameof(RootAnimationCutsceneTemplate.StartPoint), TargetFieldName = nameof(RootAnimationCutsceneTemplate.MovePoint))]
        public AIMoveToActionInputData StartPoint;

        public AIMoveToActionInputData MovePoint;
        public PlayContextAnimationActionInput Animation;
        public AIMoveToActionInputData EndPoint;

        public override List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new AIWarpActionV2(associatedInteractiveObject, this.StartPoint.WorldPoint, () => new List<ASequencedAction>()
                {
                    new AIMoveToActionV2(associatedInteractiveObject, this.MovePoint.WorldPoint, this.MovePoint.AIMovementSpeed, () => new List<ASequencedAction>()
                    {
                        new PlayContextAnimationAction(associatedInteractiveObject.AnimationController, this.Animation, () => new List<ASequencedAction>()
                        {
                            //   new AIMoveToActionV2(associatedInteractiveObject, this.EndPoint.WorldPoint, this.EndPoint.AIMovementSpeed, () => null)
                        })
                    })
                })
            };
        }
    }
}