using System;
using System.Collections.Generic;
using CoreGame;

namespace SkillAction
{
    public class SkillActionPlayerSystem
    {
        private Dictionary<Type, SkillAction> SkillActionsIndexedByType = new Dictionary<Type, SkillAction>();
        private Dictionary<Type, CooldownActionState> SkillActionCooldown = new Dictionary<Type, CooldownActionState>();

        public void FixedTick(float d)
        {
            foreach (var skillAction in SkillActionsIndexedByType.Values)
                skillAction.FixedTick(d);

            this.DiscardFinishedSkillActions();
        }

        public void BeforeInteractiveObjectTick(float d)
        {
            foreach (var skillAction in SkillActionsIndexedByType.Values)
                skillAction.BeforeInteractiveObjectTick(d);

            this.DiscardFinishedSkillActions();
        }

        public void Tick(float d)
        {
            foreach (var skillAction in SkillActionsIndexedByType.Values)
                skillAction.Tick(d);

            this.DiscardFinishedSkillActions();
            this.UpdateCooldowns(d);
        }

        public void AfterInteractiveObjectTick(float d)
        {
            foreach (var skillAction in SkillActionsIndexedByType.Values)
                skillAction.AfterInteractiveObjectTick(d);

            this.DiscardFinishedSkillActions();
        }

        public void LateTick(float d)
        {
            foreach (var skillAction in SkillActionsIndexedByType.Values)
                skillAction.LateTick(d);

            this.DiscardFinishedSkillActions();
        }

        private void UpdateCooldowns(float d)
        {
            List<Type> CooldownEndedType = null;
            foreach (var skillActionCooldownKey in SkillActionCooldown.Keys)
            {
                this.SkillActionCooldown[skillActionCooldownKey].Tick(d);
                if (!this.SkillActionCooldown[skillActionCooldownKey].IsOnCooldown())
                {
                    if (CooldownEndedType == null)
                    {
                        CooldownEndedType = new List<Type>();
                    }

                    CooldownEndedType.Add(skillActionCooldownKey);
                }
            }

            if (CooldownEndedType != null)
            {
                foreach (var type in CooldownEndedType)
                {
                    this.SkillActionsIndexedByType.Remove(type);
                }
            }
        }

        private void DiscardFinishedSkillActions()
        {
            List<SkillAction> finishedSkillActions = null;
            foreach (var skillAction in SkillActionsIndexedByType.Values)
            {
                if (skillAction.HasEnded())
                {
                    if (finishedSkillActions == null)
                    {
                        finishedSkillActions = new List<SkillAction>();
                    }

                    finishedSkillActions.Add(skillAction);
                }
            }

            if (finishedSkillActions != null)
            {
                foreach (var finishedSkillAction in finishedSkillActions)
                {
                    this.SkillActionsIndexedByType.Remove(finishedSkillAction.SourceSkillActionDefinition.GetType());
                    finishedSkillAction.OnActionEnd();
                }
            }
        }

        public bool ActionAuthorizedToBeExecuted<T>(T SkillActionDefinition) where T : SkillActionDefinition
        {
            bool executeAction = SkillActionDefinition.SkillActionDefinitionStruct.CoolDownTime == 0f;
            if (!executeAction)
            {
                this.SkillActionCooldown.TryGetValue(SkillActionDefinition.GetType(), out CooldownActionState CooldownActionState);
                executeAction = CooldownActionState == null || !CooldownActionState.IsOnCooldown();
            }

            return executeAction;
        }

        public void ExecuteGameAction(SkillAction SkillAction)
        {
            if (ActionAuthorizedToBeExecuted(SkillAction.SourceSkillActionDefinition))
            {
                this.SkillActionsIndexedByType.Add(SkillAction.SourceSkillActionDefinition.GetType(), SkillAction);
                this.SkillActionCooldown[SkillAction.SourceSkillActionDefinition.GetType()] = new CooldownActionState(SkillAction.SourceSkillActionDefinition.SkillActionDefinitionStruct.CoolDownTime);
                SkillAction.OnActionStart();
                SkillAction.Tick(0f);
            }
        }
    }

    class CooldownActionState
    {
        private float TargetCooldownTime;
        private float CurrentTimeElapsed;

        public CooldownActionState(float targetCooldownTime)
        {
            TargetCooldownTime = targetCooldownTime;
            CurrentTimeElapsed = 0f;
        }

        public void Tick(float d)
        {
            this.CurrentTimeElapsed += d;
        }

        public bool IsOnCooldown()
        {
            return this.TargetCooldownTime > 0f && this.CurrentTimeElapsed <= this.TargetCooldownTime;
        }
    }
}