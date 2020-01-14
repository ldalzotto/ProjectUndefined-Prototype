using Input;

namespace Skill
{
    public interface IEM_SkillSystem_ExposedMethods
    {
        InputID GetInputIdAssociatedToTheInteractiveObjectAction(string actionUniqueID);
    }
}