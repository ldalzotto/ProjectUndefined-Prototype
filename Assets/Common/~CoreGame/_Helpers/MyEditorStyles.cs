
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class MyEditorStyles
{
    public static GUIStyle SceneDrawDynamicLabelStyle;

    public static GUIStyle LabelWhite;
    public static GUIStyle LabelRed;
    public static GUIStyle LabelYellow;
    public static GUIStyle LabelMagenta;
    public static GUIStyle LabelBlue;
    public static GUIStyle LabelGreen;

    static MyEditorStyles()
    {
        MyEditorStyles.SceneDrawDynamicLabelStyle = BuildLabelStyle(Color.black);
        MyEditorStyles.LabelWhite = BuildLabelStyle(Color.white);
        MyEditorStyles.LabelRed = BuildLabelStyle(Color.red);
        MyEditorStyles.LabelYellow = BuildLabelStyle(Color.yellow);
        MyEditorStyles.LabelMagenta = BuildLabelStyle(Color.magenta);
        MyEditorStyles.LabelBlue = BuildLabelStyle(Color.blue);
        MyEditorStyles.LabelGreen = BuildLabelStyle(Color.green);
    }

    public static GUIStyle BuildLabelStyle(Color color)
    {
        var label = new GUIStyle(EditorStyles.label);
        label.normal.textColor = color;
        return label;
    }
}

#endif

public class MyColors
{
    public static Color TransparentBlack;
    public static Color PaleBlue;
    public static Color Coral;
    public static Color HotPink;
    public static Color MayaBlue;
    public static Color MintGreen;

    private static Color[] RandomColorsList;
    public static Color GetColorOnIndex(int index)
    {
        return MyColors.RandomColorsList[Mathf.FloorToInt(Mathf.Repeat(index, MyColors.RandomColorsList.Length))];
    }

    static MyColors()
    {
        MyColors.TransparentBlack = new Color(0, 0, 0, 0);
        MyColors.PaleBlue = new Color(0.709f, 0.827f, 0.905f);
        MyColors.Coral = new Color(1f, 127f / 255f, 80f / 255f);
        MyColors.HotPink = new Color(1f, 105f / 255f, 180f / 255f);
        MyColors.MayaBlue = new Color(115f / 255f, 194f / 255f, 251f / 255f);
        MyColors.MintGreen = new Color(62f / 255f, 180f / 255f, 137f / 255f);
        MyColors.RandomColorsList = new Color[] { MyColors.PaleBlue, MyColors.Coral, MyColors.HotPink, MyColors.MayaBlue, MyColors.MintGreen };
    }
}
