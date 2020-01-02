using System;
using System.Security.Policy;

namespace SkillAction
{
    public interface IEM_SkillActionExecution
    {
        void ExecuteSkillAction(SkillAction SkillAction);
        bool ActionAuthorizedToBeExecuted<T>(T SkillActionDefinition) where T : SkillActionDefinition;
    }
}