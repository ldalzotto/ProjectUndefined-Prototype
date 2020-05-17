using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartMenu
{
    [Serializable]
    [CreateAssetMenu(fileName = "StartMenuStaticConfiguration", menuName = "Configuration/StartMenu/StaticConfiguration/StartMenuStaticConfiguration", order = 1)]
    public class StartMenuStaticConfiguration : SerializedScriptableObject
    {
        public StartMenuPrefabConfiguration StartMenuPrefabConfiguration;
    }

}
