using CoreGame;
using UnityEngine;

namespace GlobalShaderConstants
{
    public class GlobalShaderConstantsManager : GameSingleton<GlobalShaderConstantsManager>
    {
        public const string UNSCALED_TIME = "UNSCALED_TIME";

        public void Tick()
        {
            Shader.SetGlobalFloat(UNSCALED_TIME, Time.unscaledTime);
        }
    }
}