using CoreGame;
using Editor_LevelSceneLoader;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LevelManagement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static VisualElementsHelper;

namespace Editor_LevelSceneExplorer
{
    public class LevelSceneExplorer : EditorWindow
    {
        [MenuItem("Level/LevelSceneExplorer")]
        static void Init()
        {
            LevelSceneLoaderV2 LevelSceneLoaderV2Window = EditorWindow.GetWindow<LevelSceneLoaderV2>();
            LevelSceneLoaderV2Window.Show();
            LevelSceneExplorer window = EditorWindow.GetWindow<LevelSceneExplorer>(new Type[] { typeof(LevelSceneLoaderV2) });
            window.minSize = new Vector2(300, 400);
            window.Show();
        }

        private LevelHierarchyConfiguration levelHierarchyConfiguration;
        private LevelZonesSceneConfiguration levelZonesSceneConfiguration;
        private ChunkZonesSceneConfiguration chunkZonesSceneConfiguration;

        private List<LevelLine> levelLines = new List<LevelLine>();

        private void OnEnable()
        {
            //initialization
            this.levelHierarchyConfiguration = AssetFinder.SafeSingleAssetFind<LevelHierarchyConfiguration>("t:" + typeof(LevelHierarchyConfiguration).Name);
            this.levelZonesSceneConfiguration = AssetFinder.SafeSingleAssetFind<LevelZonesSceneConfiguration>("t:" + typeof(LevelZonesSceneConfiguration).Name);
            this.chunkZonesSceneConfiguration = AssetFinder.SafeSingleAssetFind<ChunkZonesSceneConfiguration>("t:" + typeof(ChunkZonesSceneConfiguration).Name);
            //END

            var root = this.rootVisualElement;

            var searchBar = new ToolbarSearchField();
            searchBar.style.height = 15;
            searchBar.style.justifyContent = Justify.Center;
            searchBar.RegisterValueChangedCallback(this.OnSearchChange);
            searchBar.focusable = true;
            searchBar.Focus();
            root.Add(searchBar);

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            root.Add(scrollView);
            var boundingBox = new Box();
            scrollView.Add(boundingBox);
            //HEADER
            Layout_HeadderLine(boundingBox, VisualElementWithStyle(new Label("Scene path"), HeaderLabelStyle()), VisualElementWithStyle(new Label("Scene ref"), HeaderLabelStyle()));
            //CONTENT
            this.DoDisplayLevels(boundingBox);
        }

        private void DoDisplayLevels(VisualElement container)
        {
            if (this.levelHierarchyConfiguration != null)
            {
                foreach (var levelZoneId in this.levelHierarchyConfiguration.ConfigurationInherentData.Keys)
                {
                    if (this.levelZonesSceneConfiguration != null && this.levelZonesSceneConfiguration.ConfigurationInherentData.ContainsKey(levelZoneId))
                    {
                        var sceneName = this.levelZonesSceneConfiguration.GetSceneNameSafe(levelZoneId);
                        if (!string.IsNullOrEmpty(sceneName))
                        {
                            var sceneAssetObject = this.BuildObjectField(typeof(SceneAsset), this.levelZonesSceneConfiguration.ConfigurationInherentData[levelZoneId].scene);
                            var sceneLine = this.Layout_ContentLine(container, new Label(sceneName), sceneAssetObject);

                            var chunkLines = new List<LevelLine>();
                            var levelLineToAdd = new LevelLine(sceneLine, sceneName, chunkLines);
                            foreach (var chunkZoneId in this.levelHierarchyConfiguration.ConfigurationInherentData[levelZoneId].LevelHierarchy)
                            {
                                if (this.chunkZonesSceneConfiguration != null)
                                {
                                    var chunkSceneName = this.chunkZonesSceneConfiguration.GetSceneName(chunkZoneId);
                                    var chunkSceneAssetObject = this.BuildObjectField(typeof(SceneAsset), this.chunkZonesSceneConfiguration.ConfigurationInherentData[chunkZoneId].scene);
                                    var chunkLine = this.Layout_ContentLine(container, VisualElementWithStyle(new Label(chunkSceneName), IndentLabelStyle(10)), chunkSceneAssetObject);
                                    chunkLines.Add(new LevelLine(chunkLine, chunkSceneName));
                                }
                            }
                            this.levelLines.Add(levelLineToAdd);
                        }
                    }
                }
            }
        }

        #region Internal Events
        private void OnSearchChange(ChangeEvent<string> changeEvent)
        {
            if (!string.IsNullOrEmpty(changeEvent.newValue))
            {
                var reg = new Regex(changeEvent.newValue);
                foreach (var levelLine in this.levelLines)
                {
                    if (reg.Match(levelLine.SceneName).Success)
                    {
                        levelLine.Display();
                    }
                    else
                    {
                        levelLine.Hide();
                    }
                }
            }
            else
            {
                foreach (var levelLine in this.levelLines)
                {
                    levelLine.Display();
                }
            }
        }
        #endregion

        private void Layout_HeadderLine(VisualElement parent, VisualElement leftElement, VisualElement rightElement)
        {
            var line = new VisualElement();
            line.style.flexDirection = FlexDirection.Row;

            var col1 = new VisualElement();
            col1.style.flexDirection = FlexDirection.Column;

            var col2 = new VisualElement();
            col2.style.flexDirection = FlexDirection.Column;

            col1.style.marginLeft = new StyleLength(StyleKeyword.Auto);
            col1.style.marginRight = new StyleLength(StyleKeyword.Auto);
            col2.style.marginLeft = new StyleLength(StyleKeyword.Auto);
            col2.style.marginRight = new StyleLength(StyleKeyword.Auto);

            parent.Add(line);
            line.Add(col1);
            line.Add(col2);

            col1.Add(leftElement);
            col2.Add(rightElement);
        }
        private VisualElement Layout_ContentLine(VisualElement parent, VisualElement leftElement, VisualElement rightElement)
        {
            var line = new VisualElement();
            line.style.flexDirection = FlexDirection.Row;

            var col1 = new VisualElement();
            col1.style.flexDirection = FlexDirection.Column;

            var col2 = new VisualElement();
            col2.style.flexDirection = FlexDirection.Column;
            col2.style.minWidth = 200;

            col1.style.marginRight = new StyleLength(StyleKeyword.Auto);

            parent.Add(line);
            line.Add(col1);
            line.Add(col2);

            col1.Add(leftElement);
            col2.Add(rightElement);

            return line;
        }



        private ObjectField BuildObjectField(Type objectType, UnityEngine.Object objectValue)
        {
            var of = new ObjectField();
            of.objectType = objectType;
            of.value = objectValue;
            return of;
        }


        #region Styles

        private static Action<IStyle> HeaderLabelStyle()
        {
            return (style) =>
            {
                style.fontSize = 15;
                style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            };
        }
        private static Action<IStyle> IndentLabelStyle(float margin)
        {
            return (style) =>
            {
                style.marginLeft = margin;
            };
        }


        #endregion

    }

    public class LevelLine
    {
        private VisualElement lineElement;
        private List<LevelLine> nestedChunkLines;
        private string sceneName;

        public string SceneName { get => sceneName; }

        public LevelLine(VisualElement visualElement, string sceneName, List<LevelLine> nestedChunkLines)
        {
            this.lineElement = visualElement;
            this.sceneName = sceneName;
            this.nestedChunkLines = nestedChunkLines;
        }

        public LevelLine(VisualElement visualElement, string sceneName)
        {
            this.lineElement = visualElement;
            this.sceneName = sceneName;
            this.nestedChunkLines = new List<LevelLine>();
        }

        public void Hide()
        {
            this.lineElement.style.display = DisplayStyle.None;
            foreach (var chunkLine in this.nestedChunkLines)
            {
                chunkLine.Hide();
            }
        }

        public void Display()
        {
            this.lineElement.style.display = DisplayStyle.Flex;
            foreach (var chunkLine in this.nestedChunkLines)
            {
                chunkLine.Display();
            }
        }
    }

}
