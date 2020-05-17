using UnityEngine;

public static class MyLog
{
    public static string Format(object message)
    {
        return "f:" + Time.frameCount + " " + message;
    }
}
