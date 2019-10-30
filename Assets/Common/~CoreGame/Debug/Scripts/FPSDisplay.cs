using System.Text;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{

#if UNITY_EDITOR

    float deltaTime = 0.0f;


    private GUIStyle style;
    private StringBuilder FPSDisplayBuilder;

    private void Start()
    {
        this.style = new GUIStyle();
        int w = Screen.width, h = Screen.height;
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.red;
        this.FPSDisplayBuilder = new StringBuilder();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }


    private string GetFormattedString(float msec, float fps)
    {
        this.FPSDisplayBuilder.Clear();
        this.FPSDisplayBuilder.Append(msec).Append(" ms (").Append(fps).Append(" fps)");
        return this.FPSDisplayBuilder.ToString();
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;



        GUI.Label(rect, GetFormattedString(msec, fps), style);
    }

#endif
}
