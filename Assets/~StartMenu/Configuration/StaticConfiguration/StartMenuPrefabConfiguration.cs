using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    [Serializable]
    [CreateAssetMenu(fileName = "StartMenuPrefabConfiguration", menuName = "Configuration/StartMenu/StaticConfiguration/StartMenuPrefabConfiguration", order = 1)]
    public class StartMenuPrefabConfiguration : SerializedScriptableObject
    {
        public Button StartMenuButtonBasePrefab;
    }

}
