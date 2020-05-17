using UnityEngine;

public class Coroutiner : MonoBehaviour
{
    private static Coroutiner instance;

    public static Coroutiner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Coroutiner>();
            }
            return instance;
        }
    }

}
