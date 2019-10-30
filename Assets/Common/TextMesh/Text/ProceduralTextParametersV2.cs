using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TextMesh
{
    public class ProceduralTextParametersV2
    {
        public static Regex BaseParameterRegex = new Regex("\\${.*?}");
        private Regex ParameterNameExtractorRegex = new Regex("((?![\\${]).*(?=:))");
        private Regex ParameterNumberExtractorRegex = new Regex("((?<=[:]).*(?=}))");

        private ProceduralTextParameterParser ProceduralTextParameterParser;
        private Stack<IParameterImage> ParametersImage = new Stack<IParameterImage>();
        private List<ParameterImageDisplayed> CurrentlyDisplayedParametersImage = new List<ParameterImageDisplayed>();

        public ProceduralTextParametersV2(ProceduralTextParameterParser proceduralTextParameterParser)
        {
            ProceduralTextParameterParser = proceduralTextParameterParser;
        }

        public string ParseParameters(string inputText)
        {
            //Begin text Parameters
            Match parameterMatch = null;
            do
            {
                parameterMatch = BaseParameterRegex.Match(inputText);
                if (parameterMatch.Success)
                {
                    var ParameterNameMatch = ParameterNameExtractorRegex.Match(parameterMatch.Value);
                    var parameterOrderNumberMatch = ParameterNumberExtractorRegex.Match(parameterMatch.Value);

                    bool parameterReplaced = false;
                    if (ParameterNameMatch.Success && parameterOrderNumberMatch.Success)
                    {
                        var ProceduralTextParameterParserKey = new ProceduralTextParameterParserKey(Convert.ToInt32(parameterOrderNumberMatch.Value), ParameterNameMatch.Value);
                        if (ProceduralTextParameterParser.TextParameter.ContainsKey(ProceduralTextParameterParserKey))
                        {
                            //We replace the parameter text by a template that will be hidden by image
                            inputText = inputText.Substring(0, parameterMatch.Index)
                                        + ProceduralTextParameterParser.TextParameter[ProceduralTextParameterParserKey]
                                        + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));
                            parameterReplaced = true;
                        }
                    }

                    if (!parameterReplaced)
                    {
                        //We remove the parameter text
                        inputText = inputText.Substring(0, parameterMatch.Index)
                                    + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));
                    }
                }
            } while (parameterMatch.Success);
            //End text parameters

            //Begin text parameters
            int cnt = 0;
            foreach (char c in ProceduralText.RichTextQuadRegex.Replace(inputText, "@"))
            {
                if (c == '@') cnt++;
            }

            for (int i = cnt - 1; i >= 0; i--)
            {
                this.ParametersImage.Push(ProceduralTextParameterParser.ImagesParametersOrdered[i]);
            }
            //End text parameters

            return inputText;
        }

        public void OnIncrement(TextMesh textMesh, char incrementedChar, int charIdx)
        {
            //if this is an image
            if (incrementedChar == '@')
            {
                this.CurrentlyDisplayedParametersImage.Add(new ParameterImageDisplayed(this.ParametersImage.Pop(), textMesh.GetLetterAtIndex(charIdx)));
            }
        }

        public void GUITick(TextMesh textMesh)
        {
            var vertexCount = textMesh.GetVertexCount();
            for (var i = 0; i < this.CurrentlyDisplayedParametersImage.Count; i++)
            {
                int totalImagesCount = this.CurrentlyDisplayedParametersImage.Count + this.ParametersImage.Count;
                var displayedImageIndex = totalImagesCount - i;
                var topLeft = textMesh.GetVertex(vertexCount - (displayedImageIndex * 4));
                var topRight = textMesh.GetVertex(vertexCount - (displayedImageIndex * 4) + 1);
                var bottomRight = textMesh.GetVertex(vertexCount - (displayedImageIndex * 4) + 2);
                var bottomLeft = textMesh.GetVertex(vertexCount - (displayedImageIndex * 4) + 3);
                var pos = textMesh.CanvasRenderer.transform.TransformPoint(topLeft);
                this.CurrentlyDisplayedParametersImage[i].ImageParameter.GUITick(new Rect(new Vector2(pos.x, Screen.height - pos.y),
                    new Vector2(bottomRight.x - bottomLeft.x, topLeft.y - bottomLeft.y)));
                //  GUI.DrawTexture(, this.CurrentlyDisplayedParametersImage[i].ImageParameter);
            }
        }

        public void Clear()
        {
            this.ParametersImage.Clear();
            this.CurrentlyDisplayedParametersImage.Clear();
        }

        public static List<Match> ExtractParameters(string source, string ParameterName)
        {
            List<Match> results = null;
            var matches = BaseParameterRegex.Matches(source);

            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    if (match.Value.Contains(ParameterName))
                    {
                        if (results == null)
                        {
                            results = new List<Match>();
                        }

                        results.Add(match);
                    }
                }
            }

            return results;
        }
    }

    public class ProceduralTextParameterParser
    {
        public Dictionary<ProceduralTextParameterParserKey, string> TextParameter;
        public List<IParameterImage> ImagesParametersOrdered;

        public ProceduralTextParameterParser(Dictionary<ProceduralTextParameterParserKey, string> textParameter, List<IParameterImage> imagesParametersOrdered = null)
        {
            TextParameter = textParameter;
            ImagesParametersOrdered = imagesParametersOrdered;
        }
    }

    public struct ProceduralTextParameterParserKey
    {
        public int ParameterNb;
        public string ParameterType;

        public ProceduralTextParameterParserKey(int parameterNb, string parameterType)
        {
            ParameterNb = parameterNb;
            ParameterType = parameterType;
        }

        public bool Equals(ProceduralTextParameterParserKey other)
        {
            return ParameterNb == other.ParameterNb && ParameterType == other.ParameterType;
        }

        public override bool Equals(object obj)
        {
            return obj is ProceduralTextParameterParserKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ParameterNb * 397) ^ (ParameterType != null ? ParameterType.GetHashCode() : 0);
            }
        }
    }

    public interface IParameterImage
    {
        void GUITick(Rect RenderRect);
    }

    public class ParameterImageDisplayed
    {
        public IParameterImage ImageParameter;
        public LetterVertices ImageVertices;

        public ParameterImageDisplayed(IParameterImage imageParameter, LetterVertices imageVertices)
        {
            this.ImageParameter = imageParameter;
            ImageVertices = imageVertices;
        }
    }
}