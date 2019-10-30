using System;

public class RandomHelper
{
    public static T RandomBetweenEnumValue<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        Random random = new Random();
        T randomEnum = (T)values.GetValue(random.Next(values.Length));
        return randomEnum;
    }
}
