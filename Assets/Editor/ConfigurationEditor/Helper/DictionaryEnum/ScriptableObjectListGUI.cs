using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ConfigurationEditor
{
    public interface IScriptableObjectListGUI<V> where V : ScriptableObject
    {
        void GUITick(ref List<V> scriptableObjectValues);
    }

    public class ScriptableObjectListGUI<V> : IScriptableObjectListGUI<V> where V : ScriptableObject
    {
        public void GUITick(ref List<V> scriptableObjectValues)
        {
            throw new System.NotImplementedException();
        }
    }

}
