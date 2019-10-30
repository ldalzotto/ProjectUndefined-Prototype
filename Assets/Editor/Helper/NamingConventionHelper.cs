using LevelManagement;

public class NamingConventionHelper
{
    public static string BuildName(string baseName, LevelZonesID levelZoneID, PrefixType prefixType, SufixType sufixType)
    {
        string prefixTypeString = GetPrefix(prefixType);
        string sufix = GetSufix(sufixType);
        return levelZoneID.ToString() + "_" + prefixTypeString + "_" + baseName + "_" + sufix;
    }

    public static string BuildName(string baseName, PrefixType prefixType, SufixType sufixType)
    {
        string prefixTypeString = GetPrefix(prefixType);
        string sufix = GetSufix(sufixType);
        return prefixTypeString + "_" + baseName + "_" + sufix;
    }

    private static string GetSufix(SufixType sufixType)
    {
        var sufix = string.Empty;
        switch (sufixType)
        {
            case SufixType.MODEL:
                sufix = "Model";
                break;
            case SufixType.ATTRACTIVE_OBJECT:
                sufix = "AttractiveObject";
                break;
            case SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA:
                sufix = "AttractiveObject_Conf";
                break;
            case SufixType.PARTICLE_SYSTEM:
                sufix = "ParticleSystem";
                break;
            case SufixType.AI_FEEDBACK_MARK_INHERENT_DATA:
                sufix = "AIFeedBackMark_Conf";
                break;
            case SufixType.NONE:
                sufix = "";
                break;
        }

        return sufix;
    }

    private static string GetPrefix(PrefixType prefixType)
    {
        var prefixTypeString = string.Empty;
        switch (prefixType)
        {
            case PrefixType.ATTRACTIVE_OBJECT:
                prefixTypeString = "AttractiveObject";
                break;
            case PrefixType.PLAYER_ACTION:
                prefixTypeString = "PlayerAction";
                break;
            case PrefixType.WHEEL_NODE:
                prefixTypeString = "WheelNode";
                break;
            case PrefixType.AI_FEEDBACK_MARK:
                prefixTypeString = "AIFeedBackMark";
                break;
        }

        return prefixTypeString;
    }
}

public enum PrefixType
{
    ATTRACTIVE_OBJECT = 0,
    PLAYER_ACTION = 1,
    WHEEL_NODE = 2,
    AI_FEEDBACK_MARK = 3
}

public enum SufixType
{
    MODEL = 0,
    ATTRACTIVE_OBJECT = 1,
    ATTRACTIVE_OBJECT_INHERENT_DATA = 2,
    NONE = 3,
    PARTICLE_SYSTEM = 4,
    AI_FEEDBACK_MARK_INHERENT_DATA = 5
}