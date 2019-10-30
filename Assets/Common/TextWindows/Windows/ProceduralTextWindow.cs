using System;
using TextMesh;
using UnityEngine;
using UnityEngine.UI;

namespace TextWindows
{
    public class ProceduralTextWindow
    {
        public ProceduralTextWindowGameObject ProceduralTextWindowGameObject { get; private set; }
        private ProceduralTextWindowDefinition ProceduralTextWindowDefinition;
        private ProceduralText ProceduralText;
        private float ElapsedTime;

        public ProceduralTextWindow(GameObject parent, string textToDisplay, ProceduralTextWindowDefinition ProceduralTextWindowDefinition, ProceduralTextParametersV2 ProceduralTextParametersV2 = null)
        {
            this.ProceduralTextWindowDefinition = ProceduralTextWindowDefinition;
            this.ProceduralTextWindowGameObject = new ProceduralTextWindowGameObject(parent, ProceduralTextWindowDefinition);
            this.ProceduralText = new ProceduralText(textToDisplay, ProceduralTextWindowDefinition.GeneratedTextDimensionsComponent, this.ProceduralTextWindowGameObject.TextComponent, ProceduralTextParametersV2);
            this.ElapsedTime = 0f;
        }

        public void Play()
        {
            this.CalculateCurrentPage();
        }

        public void Tick(float d)
        {
            if (!this.ProceduralText.IsDisplayEngineFinished())
            {
                this.ElapsedTime += d;
                while (this.ElapsedTime >= this.ProceduralTextWindowDefinition.TextLetterDeltaTime)
                {
                    this.Increment();
                    this.ElapsedTime -= this.ProceduralTextWindowDefinition.TextLetterDeltaTime;
                }
            }
        }

        public void GUITick()
        {
            this.ProceduralText.GUITick();
        }

        private void Increment()
        {
            this.ProceduralText.Increment();
            this.ProceduralTextWindowGameObject.OnIncrement(this.ProceduralText);
        }

        private void CalculateCurrentPage()
        {
            this.ProceduralText.CalculateCurrentPage();
        }

        public void MoveToNextPage()
        {
            this.ProceduralText.MoveToNextPage();
        }

        public void GenerateAndDisplayAllText()
        {
            this.ProceduralText.GenerateAndDisplayAllText();
        }

        public void SetTransformPosition(Vector2 position)
        {
            this.ProceduralTextWindowGameObject.SetTransformPosition(position);
        }
    }

    public class ProceduralTextWindowGameObject
    {
        private GameObject RootGameObject;
        private ContentSizeFitter ContentSizeFitter;
        private HorizontalLayoutGroup RootHorizontalLayoutGroup;
        private Image BackGroundImage;

        private GameObject TextParentObject;
        private GameObject TextGameObject;

        public CanvasRenderer TextCanvasRenderer { get; private set; }
        public Text TextComponent { get; private set; }

        public ProceduralTextWindowGameObject(GameObject parent, ProceduralTextWindowDefinition ProceduralTextWindowDefinition)
        {
            this.RootGameObject = new GameObject("TextWindowRoot", typeof(RectTransform));
            this.RootGameObject.transform.parent = parent.transform;
            this.ContentSizeFitter = this.RootGameObject.AddComponent<ContentSizeFitter>();
            this.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            this.ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            this.RootHorizontalLayoutGroup = this.RootGameObject.AddComponent<HorizontalLayoutGroup>();
            this.RootHorizontalLayoutGroup.childControlHeight = false;
            this.RootHorizontalLayoutGroup.childControlWidth = false;
            this.RootHorizontalLayoutGroup.childScaleHeight = true;
            this.RootHorizontalLayoutGroup.childScaleWidth = true;
            this.RootHorizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            this.ApplyHorizontalLayoutGroupData(this.RootHorizontalLayoutGroup, ProceduralTextWindowDefinition.ProceduralTextWindowMargins);

            //initialize image
            var targetImage = this.ContentSizeFitter.gameObject.AddComponent<Image>();
            this.CopyImageComponent(ProceduralTextWindowDefinition.BackgroundImagePrefab, targetImage);

            this.TextParentObject = new GameObject("TextParent", typeof(RectTransform));
            this.TextParentObject.transform.parent = this.RootHorizontalLayoutGroup.transform;

            this.TextGameObject = new GameObject("TextObject", typeof(RectTransform));
            this.TextGameObject.transform.parent = this.TextParentObject.transform;

            //text object is upper left corner
            var textGameObjectRectTransform = this.TextGameObject.transform as RectTransform;
            textGameObjectRectTransform.anchorMax = new Vector2(0, 1);
            textGameObjectRectTransform.anchorMin = new Vector2(0, 1);
            textGameObjectRectTransform.pivot = new Vector2(0, 1);

            this.TextCanvasRenderer = this.TextGameObject.AddComponent<CanvasRenderer>();
            this.TextComponent = this.TextGameObject.AddComponent<Text>();
            this.CopyTextComponent(ProceduralTextWindowDefinition.TextPrefabDefinition, this.TextComponent);

            RectTransformHelper.SetPivot(this.RootGameObject.transform as RectTransform, ProceduralTextWindowDefinition.WindowPivot);
        }

        public void OnIncrement(ProceduralText ProceduralText)
        {
            (this.TextGameObject.transform as RectTransform).sizeDelta = new Vector2(ProceduralText.GetWindowWidth(), ProceduralText.GetWindowHeight());


            var TextParentObjectTransform = this.TextParentObject.transform as RectTransform;

            var oldAnchorMax = TextParentObjectTransform.anchorMax;
            var oldAnchorMin = TextParentObjectTransform.anchorMin;

            TextParentObjectTransform.anchorMax = new Vector2(0, 1);
            TextParentObjectTransform.anchorMin = new Vector2(0, 1);

            TextParentObjectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ProceduralText.GetWindowWidth());
            TextParentObjectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ProceduralText.GetWindowHeight());

            TextParentObjectTransform.anchorMax = oldAnchorMax;
            TextParentObjectTransform.anchorMin = oldAnchorMin;

            this.ContentSizeFitter.enabled = false;
            this.ContentSizeFitter.enabled = true;
        }

        public void SetTransformPosition(Vector2 position)
        {
            this.RootGameObject.transform.position = position;
        }

        private void CopyTextComponent(Text source, Text target)
        {
            target.alignment = source.alignment;
            target.font = source.font;
            target.resizeTextForBestFit = source.resizeTextForBestFit;
            target.fontSize = source.fontSize;
            target.fontStyle = source.fontStyle;
            target.horizontalOverflow = source.horizontalOverflow;
            target.lineSpacing = source.lineSpacing;
            target.resizeTextMaxSize = source.resizeTextMaxSize;
            target.resizeTextMinSize = source.resizeTextMinSize;
            target.supportRichText = source.supportRichText;
            target.verticalOverflow = source.verticalOverflow;
            target.alignByGeometry = source.alignByGeometry;
            target.color = source.color;
        }

        private void ApplyHorizontalLayoutGroupData(HorizontalLayoutGroup HorizontalLayoutGroup, ProceduralTextWindowMargins ProceduralTextWindowMargins)
        {
            HorizontalLayoutGroup.padding.bottom = ProceduralTextWindowMargins.MarginDown;
            HorizontalLayoutGroup.padding.top = ProceduralTextWindowMargins.marginUp;
            HorizontalLayoutGroup.padding.left = ProceduralTextWindowMargins.MarginLeft;
            HorizontalLayoutGroup.padding.right = ProceduralTextWindowMargins.MarginRight;
        }

        private void CopyImageComponent(Image source, Image target)
        {
            target.material = source.material;
            target.sprite = source.sprite;
            target.type = source.type;
            target.fillAmount = source.fillAmount;
            target.fillCenter = source.fillCenter;
            target.fillClockwise = source.fillClockwise;
            target.fillMethod = source.fillMethod;
            target.fillOrigin = source.fillOrigin;
            target.color = source.color;
        }
    }

    [Serializable]
    public class ProceduralTextWindowDefinition
    {
        public PivotType WindowPivot;
        public Text TextPrefabDefinition;
        public Image BackgroundImagePrefab;
        public float TextLetterDeltaTime;
        public GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
        public ProceduralTextWindowMargins ProceduralTextWindowMargins;
    }

    [Serializable]
    public class ProceduralTextWindowMargins
    {
        public int MarginLeft;
        public int MarginRight;
        public int marginUp;
        public int MarginDown;
    }
}