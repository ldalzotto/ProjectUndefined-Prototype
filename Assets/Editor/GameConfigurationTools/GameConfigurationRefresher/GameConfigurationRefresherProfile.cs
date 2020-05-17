using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "GameConfigurationRefresherProfile", menuName = "Configuration/GameConfigurationRefresherProfile", order = 1)]
public class GameConfigurationRefresherProfile : SerializedScriptableObject
{
    public Dictionary<string, SingleGameConfigurationRefresherProfile> GameConfgurationsRefreshEditorProfile = new Dictionary<string, SingleGameConfigurationRefresherProfile>();
}

[System.Serializable]
public class SingleGameConfigurationRefresherProfile
{
    public bool Folded;
}
