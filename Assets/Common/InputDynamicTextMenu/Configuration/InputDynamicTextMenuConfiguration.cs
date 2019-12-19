using System;
using OdinSerializer;
using TMPro;
using UnityEngine;

namespace InputDynamicTextMenu
{
    [Serializable]
    public class InputDynamicTextMenuConfiguration : SerializedScriptableObject
    {
        public GameObject UIInputDynamicTextMenuModulesContainerPrefab;
        public GameObject InputDynamicTextMenuModuleTemplatePrefab;
    }
}