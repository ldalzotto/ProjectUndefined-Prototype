using System;
using System.Collections.Generic;

namespace CoreGame
{
    public static class FunctionalExtensions
    {
        public static void IfNotNull<T>(this T input, Action<T> action)
        {
            if (input != null)
            {
                action.Invoke(input);
            }
        }

        public static void IfTypeEqual<COMPARISON_TYPE>(this object input, Action<COMPARISON_TYPE> action)
        {
            if (input != null && input.GetType() == typeof(COMPARISON_TYPE))
            {
                action.Invoke((COMPARISON_TYPE)input);
            }
        }

        public static void IfNotNullAndTypeNotContainedInList<T>(this T input, List<Type> containedTypeList, Action<T> action)
        {
            input.IfNotNull((i) =>
            {
                containedTypeList.IfNotNull((l) =>
                {
                    if (!containedTypeList.Contains(i.GetType()))
                    {
                        action.Invoke(input);
                    }
                });
            });
        }

        public static T With<T>(this T input, Action<T> action)
        {
            input.IfNotNull((i) => action.Invoke(i));
            return input;
        }
    }
}
