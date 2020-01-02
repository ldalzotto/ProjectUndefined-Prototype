using UnityEngine.UI;

namespace SkillAction
{
    /// <summary>
    /// Skill Actions are action that have constraint either from cooldown <see cref="SkillActionDefinitionStruct.CoolDownTime"/> or from a future execution count limit.
    /// From a functional point of view, the action performed interacts with the environment (by spawning objects, triggering )
    /// </summary>
    public abstract class SkillAction
    {
        public SkillActionDefinition SourceSkillActionDefinition { get; private set; }
        protected SkillActionDefinitionStruct SkillActionDefinitionStruct;

        public SkillAction(SkillActionDefinition SkillActionDefinition)
        {
            this.SourceSkillActionDefinition = SkillActionDefinition;
            this.SkillActionDefinitionStruct = SkillActionDefinition.SkillActionDefinitionStruct;
        }
        
        public abstract bool HasEnded();

        public virtual void OnActionStart()
        {
        }

        public virtual void OnActionEnd()
        {
        }

        public virtual void FixedTick(float d)
        {
        }

        public virtual void BeforeInteractiveObjectTick(float d)
        {
        }

        public virtual void Tick(float d)
        {
        }

        public virtual void AfterInteractiveObjectTick(float d)
        {
        }

        public virtual void LateTick(float d)
        {
        }
    }
}