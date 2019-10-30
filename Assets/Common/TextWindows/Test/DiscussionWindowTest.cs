using System.Collections.Generic;
using TextMesh;
using TextWindows;
using UnityEngine;

namespace DefaultNamespace
{
    public class DiscussionWindowTest : MonoBehaviour
    {
        public Font Font;
        public Material FontMaterial;
        public Texture2D Texture2D;
        private DisplayAllTextWindow DisplayAllTextWindow;
        public ProceduralTextWindowDefinition ProceduralTextWindowDefinition;

        private void Start()
        {
            DisplayAllTextWindow = new DisplayAllTextWindow(FindObjectOfType<Canvas>().gameObject, "Test <quad>", this.ProceduralTextWindowDefinition,
                new ProceduralTextParametersV2(new ProceduralTextParameterParser(
                    null, new List<IParameterImage>()
                    {
                        new TextureParameterImage(this.Texture2D)
                    }
                )));
        }

        private void OnGUI()
        {
            this.DisplayAllTextWindow.GUITick();
        }
    }

    public class TextureParameterImage : IParameterImage
    {
        private Texture2D Texture2D;

        public TextureParameterImage(Texture2D texture2D)
        {
            Texture2D = texture2D;
        }

        public void GUITick(Rect RenderRect)
        {
            GUI.DrawTexture(RenderRect, this.Texture2D);
        }
    }
}