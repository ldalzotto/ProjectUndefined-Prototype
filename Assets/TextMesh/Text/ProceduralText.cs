using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TextMesh
{
    public class ProceduralText
    {
        public static Regex RichTextQuadRegex = new Regex("<quad.*?>");
        private char[] TrimmedCharForSanitaze = new char[] {' ', '\n'};

        #region State

        private string initialRawText;
        private string transformedInitialRawText;
        private string overlappedText;
        private List<string> linedTruncatedText;

        private TextMesh TextMesh;
        private GeneratedTextDimensions textDimensions;
        private TextPlayerEngine TextPlayerEngine;
        public ProceduralTextParametersV2 ProceduralTextParametersV2 { get; private set; }

        #endregion

        public ProceduralText(string initialRawText, GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent, Text textAreaText, ProceduralTextParametersV2 ProceduralTextParametersV2 = null)
        {
            this.initialRawText = initialRawText;
            this.transformedInitialRawText = Regex.Unescape(this.initialRawText);
            this.ProceduralTextParametersV2 = ProceduralTextParametersV2;

            #region Special Character Image mapping

            if (this.ProceduralTextParametersV2 != null)
            {
                this.transformedInitialRawText = this.ProceduralTextParametersV2.ParseParameters(this.transformedInitialRawText);
            }

            #endregion

            this.textDimensions = new GeneratedTextDimensions(GeneratedTextDimensionsComponent);
            this.TextPlayerEngine = new TextPlayerEngine(GeneratedTextDimensionsComponent, this.textDimensions);

            this.TextMesh = new TextMesh(textAreaText);
        }

        public void GUITick()
        {
            this.TextPlayerEngine.GUITick(this.TextMesh, this.ProceduralTextParametersV2);
        }

        #region Data Retrieval

        public List<string> LinedTruncatedText => this.linedTruncatedText;

        public float GetWindowHeight()
        {
            return this.textDimensions.GetWindowHeight(this.TextMesh);
        }

        public float GetWindowWidth()
        {
            return this.textDimensions.GetWindowWidth(this.TextMesh);
        }

        #endregion

        #region Writing

        public void Increment()
        {
            this.TextPlayerEngine.Increment(this.TextMesh, this.ProceduralTextParametersV2);
        }

        #endregion

        #region Logical Conditions

        public bool IsDisplayEngineFinished()
        {
            return this.TextPlayerEngine.IsDisplayEngineFinished();
        }

        #endregion

        #region External Events

        public void MoveToNextPage()
        {
            this.transformedInitialRawText = this.overlappedText;
            this.overlappedText = string.Empty;
            this.linedTruncatedText.Clear();
            this.TextMesh.Clear();
            this.ProceduralTextParametersV2.Clear();
            this.CalculateCurrentPage();
        }

        #endregion

        #region Entry points

        public void CalculateCurrentPage()
        {
            var generatedText = this.TextMesh.ForceRefreshInternalGeneration(this.transformedInitialRawText, new Vector2(this.textDimensions.GetMaxWindowWidth(), this.textDimensions.GetMaxWindowHeight()));

            var truncatedText = this.transformedInitialRawText.Substring(0, generatedText.characterCountVisible);
            this.overlappedText = this.transformedInitialRawText.Substring(generatedText.characterCountVisible, this.transformedInitialRawText.Length - generatedText.characterCountVisible);

            truncatedText = truncatedText.Trim(this.TrimmedCharForSanitaze);

            generatedText = this.TextMesh.ForceRefreshInternalGeneration(truncatedText, new Vector2(this.textDimensions.GetMaxWindowWidth(), this.textDimensions.GetMaxWindowHeight()));

            this.linedTruncatedText = new List<string>();
            for (int i = 0; i < generatedText.lines.Count; i++)
            {
                int startIndex = generatedText.lines[i].startCharIdx;
                int endIndex = (i == generatedText.lines.Count - 1)
                    ? truncatedText.Length
                    : generatedText.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;

                string lineToAdd = truncatedText.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze);
                this.linedTruncatedText.Add(RichTextQuadRegex.Replace(lineToAdd, "@"));
            }

            this.TextMesh.GenerateFinalMeshFromTextGenerator();
            this.TextPlayerEngine.StartWriting(this);
        }

        public void GenerateAndDisplayAllText()
        {
            this.CalculateCurrentPage();
            this.TextPlayerEngine.RenderEverything(this.TextMesh, this.ProceduralTextParametersV2);
        }

        #endregion
    }

    public class TextPlayerEngine
    {
        #region Trackers

        private int VisibleQuadTotalCounter;

        #endregion

        private GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
        private GeneratedTextDimensions GeneratedTextDimensions;

        public TextPlayerEngine(GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent,
            GeneratedTextDimensions GeneratedTextDimensions)
        {
            this.GeneratedTextDimensionsComponent = GeneratedTextDimensionsComponent;
            this.GeneratedTextDimensions = GeneratedTextDimensions;
            this.VisibleQuadTotalCounter = 0;
        }

        private string targetText;
        private string currentDisplayedTextUnModified;


        public void GUITick(TextMesh TextMesh, ProceduralTextParametersV2 ProceduralTextParametersV2)
        {
            if (ProceduralTextParametersV2 != null)
            {
                ProceduralTextParametersV2.GUITick(TextMesh);
            }
        }

        public void StartWriting(ProceduralText discussionText)
        {
            this.targetText = String.Join("\n", discussionText.LinedTruncatedText.ToArray());
            this.currentDisplayedTextUnModified = String.Empty;
        }

        public void Increment(TextMesh TextMesh, ProceduralTextParametersV2 ProceduralTextParametersV2 = null)
        {
            if (currentDisplayedTextUnModified.Length < targetText.Length)
            {
                var stringToAdd = targetText[currentDisplayedTextUnModified.Length].ToString();

                for (var i = 0; i < stringToAdd.Length; i++)
                {
                    if (stringToAdd[i] != ' ' && stringToAdd[i] != '\n')
                    {
                        TextMesh.IncrementChar(stringToAdd[i]);
                        if (ProceduralTextParametersV2 != null)
                        {
                            ProceduralTextParametersV2.OnIncrement(TextMesh, stringToAdd[i], this.VisibleQuadTotalCounter);
                        }

                        this.VisibleQuadTotalCounter += 1;
                    }

                    currentDisplayedTextUnModified += stringToAdd[i];
                }
            }
        }

        public void RenderEverything(TextMesh TextMesh, ProceduralTextParametersV2 ProceduralTextParametersV2 = null)
        {
            while (currentDisplayedTextUnModified.Length != targetText.Length)
            {
                this.Increment(TextMesh, ProceduralTextParametersV2);
            }
        }

        public bool IsDisplayEngineFinished()
        {
            return currentDisplayedTextUnModified.Length == targetText.Length;
        }
    }

    public class TextMesh
    {
        private CanvasRenderer canvasRenderer;
        private TextGenerationSettings textGenerationSettings;
        private TextGenerator textGenerator;

        private Color textColor;
        private string lastMeshedText;

        private Mesh mesh;
        private Vector3[] meshVerticesCopy;

        private MeshDimensions MeshDimensions;

        public TextMesh(Text text)
        {
            this.canvasRenderer = text.canvasRenderer;
            this.textGenerationSettings = text.GetGenerationSettings(Vector2.zero);
            this.textGenerator = new TextGenerator();
            this.textGenerator.Invalidate();
            this.mesh = new Mesh();
            this.MeshDimensions = new MeshDimensions();
            this.textColor = text.color;

            text.enabled = false;
            this.canvasRenderer.SetMaterial(text.font.material, null);
        }

        public TextGenerator ForceRefreshInternalGeneration(string text, Nullable<Vector2> unmargedExtends = null)
        {
            this.textGenerator.Invalidate();
            if (unmargedExtends.HasValue)
            {
                this.textGenerationSettings.generationExtents = unmargedExtends.Value;
            }

            this.textGenerator.Populate(text, this.TextGenerationSettings);
            return this.textGenerator;
        }

        public void GenerateFinalMeshFromTextGenerator()
        {
            this.TextGenToMesh(this.textGenerator, ref this.mesh);
            this.lastMeshedText = string.Empty;
            this.canvasRenderer.SetMesh(this.mesh);
        }

        public void IncrementChar(char characterAdded)
        {
            this.lastMeshedText += characterAdded;
            var newColors = this.mesh.colors;
            newColors[((this.lastMeshedText.Length - 1) * 4)] = this.textColor;
            newColors[((this.lastMeshedText.Length - 1) * 4) + 1] = this.textColor;
            newColors[((this.lastMeshedText.Length - 1) * 4) + 2] = this.textColor;
            newColors[((this.lastMeshedText.Length - 1) * 4) + 3] = this.textColor;

            this.MeshDimensions.OnLetterVerticesShowed(this.meshVerticesCopy[((this.lastMeshedText.Length - 1) * 4)]);
            this.MeshDimensions.OnLetterVerticesShowed(this.meshVerticesCopy[((this.lastMeshedText.Length - 1) * 4) + 1]);
            this.MeshDimensions.OnLetterVerticesShowed(this.meshVerticesCopy[((this.lastMeshedText.Length - 1) * 4) + 2]);
            this.MeshDimensions.OnLetterVerticesShowed(this.meshVerticesCopy[((this.lastMeshedText.Length - 1) * 4) + 3]);

            this.mesh.colors = newColors;

            this.canvasRenderer.SetMesh(this.mesh);
        }

        public LetterVertices GetLetterAtIndex(int idx)
        {
            return new LetterVertices(new[] {idx * 4, (idx * 4) + 1, (idx * 4) + 2, (idx * 4) + 3});
        }

        public Vector3 GetVertex(int index)
        {
            return this.meshVerticesCopy[index];
        }

        public int GetVertexCount()
        {
            return this.meshVerticesCopy.Length;
        }

        private void TextGenToMesh(TextGenerator generator, ref Mesh mesh)
        {
            var scaleMatrix = Matrix4x4.Scale(new Vector3(this.textGenerationSettings.scaleFactor, this.textGenerationSettings.scaleFactor, this.textGenerationSettings.scaleFactor)).inverse;
            this.meshVerticesCopy = generator.verts.Select(v => scaleMatrix.MultiplyPoint(v.position)).ToArray();
            mesh.vertices = this.meshVerticesCopy;
            mesh.colors32 = new Color32[mesh.vertexCount];
            mesh.uv = generator.verts.Select(v => v.uv0).ToArray();
            var triangles = new int[generator.vertexCount * 6];
            for (var i = 0; i < mesh.vertices.Length / 4; i++)
            {
                var startVerticeIndex = i * 4;
                var startTriangleIndex = i * 6;
                triangles[startTriangleIndex++] = startVerticeIndex;
                triangles[startTriangleIndex++] = startVerticeIndex + 1;
                triangles[startTriangleIndex++] = startVerticeIndex + 2;
                triangles[startTriangleIndex++] = startVerticeIndex;
                triangles[startTriangleIndex++] = startVerticeIndex + 2;
                triangles[startTriangleIndex] = startVerticeIndex + 3;
            }

            mesh.triangles = triangles;
        }

        public Transform Transform
        {
            get { return this.canvasRenderer.transform; }
        }

        public TextGenerationSettings TextGenerationSettings
        {
            get => textGenerationSettings;
        }

        public CanvasRenderer CanvasRenderer
        {
            get => canvasRenderer;
        }

        public float GetMeshWidth()
        {
            return this.MeshDimensions.Width;
        }

        public float GetMeshHeight()
        {
            return this.MeshDimensions.Height;
        }

        public void Clear()
        {
            this.mesh.Clear();
            this.MeshDimensions.Clear();
        }
    }

    public class LetterVertices
    {
        public int[] Indices;

        public LetterVertices(int[] indices)
        {
            Indices = indices;
        }
    }

    class MeshDimensions
    {
        private float MinY;
        private float MaxY;

        private float MinX;
        private float MaxX;

        public float Height { get; private set; }
        public float Width { get; private set; }

        public void OnLetterVerticesShowed(Vector2 verticePosition)
        {
            this.MinX = Mathf.Min(this.MinX, verticePosition.x);
            this.MaxX = Mathf.Max(this.MaxX, verticePosition.x);

            this.Width = Mathf.Abs(MaxX - MinX);

            this.MinY = Mathf.Min(this.MinY, verticePosition.y);
            this.MaxY = Mathf.Max(this.MaxY, verticePosition.y);

            this.Height = Mathf.Abs(MaxY - MinY);
        }

        public void Clear()
        {
            this.Width = 0f;
            this.MinX = 0f;
            this.MaxX = 0f;
            this.Height = 0f;
            this.MinY = 0f;
            this.MaxY = 0f;
        }
    }
}