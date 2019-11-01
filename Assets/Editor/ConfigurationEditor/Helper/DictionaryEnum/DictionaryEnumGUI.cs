using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Linq;

namespace ConfigurationEditor
{
    public interface IDictionaryEnumGUI<K, V> where K : Enum where V : ScriptableObject
    {
        void GUITick(ref Dictionary<K, V> dictionaryEditorValues);
        void SetSearchFilter(string searchString);
    }

    [Serializable]
    public class DictionaryEnumGUI<K, V> : IDictionaryEnumGUI<K, V> where K : Enum where V : ScriptableObject
    {
        #region Add Entry

        private K idToAdd;
        [SerializeField] private string keySearchString;
        private Regex keySearchRegex;
        private String keySearchRegexErrorMessage;
        private GUIStyle keySearchRegexErrorMessageStyle;

        #endregion

        #region  Sensitive Operations

        [SerializeField] private bool sensitiveOperationsEnabled;

        #endregion

        #region Search Field

        private SearchField keySearchField;

        private GUIStyle keySearchFieldStyle;
        [SerializeField] private bool alphabeticalOrder;
        [SerializeField] private Vector2 filtersScrollPosition;

        #endregion

        #region Search Result

        [SerializeField] private Vector2 searchResultScrollPosition;

        [SerializeField] private DictionaryEnumGUIResultPagination DictionaryEnumGUIResultPagination;

        #endregion

        #region Modifying Entry

        private Dictionary<K, V> valuesToSet = new Dictionary<K, V>();
        private Dictionary<K, V> valuesToRemove = new Dictionary<K, V>();

        #endregion

        #region Entry Look

        [SerializeField] private List<K> valuesToLook;
        [SerializeField] private bool forceLookOfAll;
        private GUIStyle removeButtonDeactivatedStyle;
        private Dictionary<K, Editor> cachedEditors;
        private bool clearCacheRequested;

        #endregion

        #region External Events

        public void RequestClearEditorCache()
        {
            this.clearCacheRequested = true;
        }

        public void SetSearchFilter(string searchFilter)
        {
            this.keySearchString = searchFilter;
        }

        #endregion

        public void GUITick(ref Dictionary<K, V> dictionaryEditorValues)
        {
            this.DoClearEditorCache();

            DoInit(ref dictionaryEditorValues);
            DoAddEntry(ref dictionaryEditorValues);

            EditorGUILayout.LabelField(typeof(V).Name + " : ", EditorStyles.boldLabel);

            DoSearchFilter(ref dictionaryEditorValues);

            var dictionaryEditorEntryValues = dictionaryEditorValues.ToList();
            if (this.alphabeticalOrder)
            {
                dictionaryEditorEntryValues = dictionaryEditorValues.OrderBy(kv => kv.Key.ToString()).ToList();
            }

            this.searchResultScrollPosition = EditorGUILayout.BeginScrollView(this.searchResultScrollPosition, GUILayout.MaxHeight(1000), GUILayout.MinHeight(100));
            EditorGUILayout.BeginVertical(EditorStyles.textField);
            EditorGUILayout.LabelField("Results : ", EditorStyles.boldLabel);
            this.DictionaryEnumGUIResultPagination.GUITick();

            int displayedCounter = 0;
            int startElementNb = this.DictionaryEnumGUIResultPagination.StartElementNb();
            int endElementNb = this.DictionaryEnumGUIResultPagination.EndElementNp();
            foreach (var dictionaryEditorEntry in dictionaryEditorEntryValues)
            {
                //search filter
                bool isKeyMatchingRegex = false;
                if (this.keySearchRegex != null)
                {
                    var keySearchRegexMatch = this.keySearchRegex.Matches(dictionaryEditorEntry.Key.ToString());
                    if (keySearchRegexMatch != null && keySearchRegexMatch.Count > 0)
                    {
                        isKeyMatchingRegex = true;
                    }
                }


                if ((this.keySearchString == null || this.keySearchString == "" || isKeyMatchingRegex /* dictionaryEditorEntry.Key.ToString().ToLower().Contains(keySearchString.ToLower())*/)
                )
                {
                    if (displayedCounter >= startElementNb && displayedCounter < endElementNb)
                    {
                        DoDisplayEntry(ref dictionaryEditorValues, dictionaryEditorEntry);
                        if (this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                        {
                            if (dictionaryEditorEntry.Value != null)
                            {
                                if (!this.cachedEditors.ContainsKey(dictionaryEditorEntry.Key))
                                {
                                    this.cachedEditors.Add(dictionaryEditorEntry.Key, DynamicEditorCreation.Get().CreateEditor(dictionaryEditorEntry.Value));
                                }

                                this.cachedEditors[dictionaryEditorEntry.Key].OnInspectorGUI();
                                EditorGUILayout.Space();
                            }
                        }
                    }

                    displayedCounter += 1;
                }
            }

            if (displayedCounter == 0)
            {
                EditorGUILayout.LabelField("No elements found.");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            if (valuesToSet.Count > 0)
            {
                foreach (var valueToSet in valuesToSet)
                {
                    dictionaryEditorValues[valueToSet.Key] = valueToSet.Value;
                }

                valuesToSet.Clear();
            }

            if (valuesToRemove.Count > 0)
            {
                foreach (var valueToRemove in valuesToRemove)
                {
                    dictionaryEditorValues.Remove(valueToRemove.Key);
                }

                valuesToRemove.Clear();
            }
        }

        private void DoClearEditorCache()
        {
            if (this.clearCacheRequested)
            {
                this.cachedEditors = new Dictionary<K, Editor>();
            }

            this.clearCacheRequested = false;
        }

        private void DoInit(ref Dictionary<K, V> dictionaryEditorValues)
        {
            if (this.valuesToLook == null)
            {
                this.valuesToLook = new List<K>();
            }

            if (dictionaryEditorValues == null)
            {
                dictionaryEditorValues = new Dictionary<K, V>();
            }

            if (keySearchField == null)
            {
                keySearchField = new SearchField();
                this.keySearchFieldStyle = new GUIStyle();
                this.keySearchFieldStyle.padding = EditorStyles.miniButton.padding;
            }

            if (removeButtonDeactivatedStyle == null)
            {
                removeButtonDeactivatedStyle = new GUIStyle(EditorStyles.miniButtonRight);
                removeButtonDeactivatedStyle.normal = EditorStyles.miniButtonRight.active;
            }

            if (this.DictionaryEnumGUIResultPagination == null)
            {
                this.DictionaryEnumGUIResultPagination = new DictionaryEnumGUIResultPagination();
            }

            if (this.valuesToSet == null)
            {
                this.valuesToSet = new Dictionary<K, V>();
            }

            if (this.valuesToRemove == null)
            {
                this.valuesToRemove = new Dictionary<K, V>();
            }

            if (this.cachedEditors == null)
            {
                this.cachedEditors = new Dictionary<K, Editor>();
            }

            if (this.keySearchRegex == null && !(this.keySearchString == String.Empty && this.keySearchString == null))
            {
                this.UpdateSearchStringRegex();
            }

            if (this.keySearchRegexErrorMessageStyle == null)
            {
                this.keySearchRegexErrorMessageStyle = new GUIStyle(EditorStyles.label);
                this.keySearchRegexErrorMessageStyle.normal.textColor = Color.red;
            }
        }

        private void DoAddEntry(ref Dictionary<K, V> dictionaryEditorValues)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Add " + typeof(V).Name + " configuration : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            var s = new GUIStyle();
            s.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.BeginVertical(s, GUILayout.Width(30));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft))
            {
                if (!dictionaryEditorValues.ContainsKey(this.idToAdd))
                {
                    dictionaryEditorValues.Add(this.idToAdd, null);
                }
            }

            if (GUILayout.Button(new GUIContent("+*", "Add all remaining enumerations."), EditorStyles.miniButtonRight))
            {
                if (typeof(K).IsEnum)
                {
                    var alreadyAddedEnums = dictionaryEditorValues.Keys;
                    foreach (int i in Enum.GetValues(typeof(K)))
                    {
                        var currentEnum = (K) Enum.Parse(typeof(K), i.ToString());
                        if (!alreadyAddedEnums.Contains(currentEnum))
                        {
                            dictionaryEditorValues.Add(currentEnum, null);
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            var parsedIdToAdd = (Enum) Enum.Parse(typeof(K), idToAdd.ToString());
            this.idToAdd = (K) Enum.Parse(typeof(K), EditorGUILayout.EnumPopup(parsedIdToAdd).ToString());
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        private void DoSearchFilter(ref Dictionary<K, V> dictionaryEditorValues)
        {
            EditorGUILayout.BeginVertical(this.keySearchFieldStyle);

            EditorGUI.BeginChangeCheck();
            string keySearchString = this.keySearchField.OnGUI(this.keySearchString);
            if (EditorGUI.EndChangeCheck())
            {
                this.keySearchString = keySearchString;
                this.UpdateSearchStringRegex();
            }

            if (this.keySearchRegexErrorMessage != null && this.keySearchRegexErrorMessage != String.Empty)
            {
                EditorGUILayout.LabelField(this.keySearchRegexErrorMessage, this.keySearchRegexErrorMessageStyle);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(30f));
            EditorGUI.BeginChangeCheck();
            bool sensitiveOperationsEnabled = GUILayout.Toggle(this.sensitiveOperationsEnabled, new GUIContent("!", "Authorize sensitive operations."), EditorStyles.miniButtonLeft);
            bool alphabeticalOrder = GUILayout.Toggle(this.alphabeticalOrder, new GUIContent("A↑", "Alphabetical order."), EditorStyles.miniButtonMid);
            bool forceLookAll = GUILayout.Toggle(this.forceLookOfAll, new GUIContent("L*", "Show all elements detail."), EditorStyles.miniButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                if (forceLookAll)
                {
                    this.valuesToLook = dictionaryEditorValues.Keys.ToList();
                }
                else
                {
                    this.valuesToLook.Clear();
                }

                this.sensitiveOperationsEnabled = sensitiveOperationsEnabled;
                this.alphabeticalOrder = alphabeticalOrder;
                this.forceLookOfAll = forceLookAll;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DoDisplayEntry(ref Dictionary<K, V> dictionaryEditorValues, KeyValuePair<K, V> dictionaryEditorEntry)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            var parsedEntry = (Enum) Enum.Parse(typeof(K), dictionaryEditorEntry.Key.ToString());
            var selectedNewKey = (K) Enum.Parse(typeof(K), EditorGUILayout.EnumPopup(parsedEntry).ToString());
            if (EditorGUI.EndChangeCheck())
            {
                if (!dictionaryEditorValues.ContainsKey(selectedNewKey))
                {
                    valuesToSet[selectedNewKey] = dictionaryEditorEntry.Value;
                    valuesToRemove[dictionaryEditorEntry.Key] = dictionaryEditorEntry.Value;
                }
            }

            EditorGUI.BeginChangeCheck();
            var configurationObjectField = EditorGUILayout.ObjectField(dictionaryEditorEntry.Value, typeof(V), false) as V;
            if (EditorGUI.EndChangeCheck())
            {
                this.RequestClearEditorCache();
                valuesToSet[dictionaryEditorEntry.Key] = configurationObjectField;
            }

            var lookPressed = (this.valuesToLook.Contains(dictionaryEditorEntry.Key));
            lookPressed = GUILayout.Toggle(lookPressed, new GUIContent("L", "View element details."), EditorStyles.miniButtonLeft, GUILayout.Width(20f));

            if (!this.forceLookOfAll)
            {
                if (lookPressed)
                {
                    if (!this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                    {
                        this.valuesToLook.Add(dictionaryEditorEntry.Key);
                    }
                }
                else
                {
                    if (this.valuesToLook.Contains(dictionaryEditorEntry.Key))
                    {
                        this.valuesToLook.Remove(dictionaryEditorEntry.Key);
                    }
                }
            }

            GUIStyle buttonStyle = EditorStyles.miniButtonRight;
            if (!sensitiveOperationsEnabled)
            {
                buttonStyle = this.removeButtonDeactivatedStyle;
            }

            if (GUILayout.Button(new GUIContent("-", "Delete element."), buttonStyle, GUILayout.Width(20f)))
            {
                if (sensitiveOperationsEnabled)
                {
                    valuesToRemove[dictionaryEditorEntry.Key] = configurationObjectField;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateSearchStringRegex()
        {
            try
            {
                this.keySearchRegex = new Regex(this.keySearchString, RegexOptions.IgnoreCase);
                this.keySearchRegexErrorMessage = String.Empty;
            }
            catch (Exception e)
            {
                this.keySearchRegexErrorMessage = e.Message;
                this.keySearchRegex = null;
            }
        }
    }
}

#endif //UNITY_EDITOR