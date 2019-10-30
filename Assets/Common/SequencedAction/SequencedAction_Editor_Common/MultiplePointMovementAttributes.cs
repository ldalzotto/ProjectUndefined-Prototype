using System;

namespace SequencedAction_Editor_Common
{
    public abstract class MultiplePointMovementAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MultiplePointMovementNested : MultiplePointMovementAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MultiplePointMovementAware : MultiplePointMovementAttribute
    {
    }
}