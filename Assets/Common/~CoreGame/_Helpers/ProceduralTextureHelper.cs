using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public static class ProceduralTextureHelper
    {
        public static Texture2D CreateSliceTexture(int width)
        {
           return new Texture2D(width, 1, TextureFormat.RGB24, false);
        }

        public static void SetTextureSliceColor(Texture2D tex, List<StartEndSlice> slices, Color insideColor, Color outsideColor)
        {
            var colors = tex.GetPixels();
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = outsideColor;
            }

            foreach(var slice in slices)
            {
                ComputeColorsPixel(slice, ref colors, insideColor);
            }

            tex.SetPixels(colors);
            tex.Apply(false);
        }

        private static void ComputeColorsPixel(StartEndSlice slice, ref Color[] colors, Color insideColor)
        {
            var beginInt = Mathf.RoundToInt(slice.BeginIncluded);
            var endInt = Mathf.RoundToInt(slice.EndExcluded);

            for (var i = beginInt; i < endInt; i++)
            {
                colors[i] = insideColor;
            }
        }
    }
}
