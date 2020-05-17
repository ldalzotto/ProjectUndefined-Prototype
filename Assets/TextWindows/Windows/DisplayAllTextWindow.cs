using TextMesh;
using UnityEngine;

namespace TextWindows
{
    public class DisplayAllTextWindow
    {
        private ProceduralTextWindow ProceduralTextWindow;

        public DisplayAllTextWindow(GameObject parent, string textToDisplay, ProceduralTextWindowDefinition ProceduralTextWindowDefinition, ProceduralTextParametersV2 ProceduralTextParametersV2 = null)
        {
            this.ProceduralTextWindow = new ProceduralTextWindow(parent, textToDisplay, ProceduralTextWindowDefinition, ProceduralTextParametersV2);
            this.ProceduralTextWindow.GenerateAndDisplayAllText();
        }

        public void SetTransformPosition(Vector2 position)
        {
            this.ProceduralTextWindow.SetTransformPosition(position);
        }

        public void GUITick()
        {
            this.ProceduralTextWindow.GUITick();
        }
    }
}