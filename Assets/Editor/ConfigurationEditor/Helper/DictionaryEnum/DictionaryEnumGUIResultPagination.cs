#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class DictionaryEnumGUIResultPagination
    {
        [SerializeField]
        private int currentPageSelected;

        [SerializeField]
        private int maxElementPerPage = 50;

        private GUIStyle paginationStyle;

        public void GUITick()
        {
            if (this.paginationStyle == null)
            {
                this.paginationStyle = new GUIStyle();
                this.paginationStyle.alignment = TextAnchor.MiddleCenter;
            }

            EditorGUILayout.BeginHorizontal(this.paginationStyle, GUILayout.Width(100));
            if (GUILayout.Button(new GUIContent("◄"), EditorStyles.miniButton))
            {
                this.currentPageSelected -= 1;
                this.currentPageSelected = Mathf.Max(this.currentPageSelected, 0);
            }
            GUILayout.Label(this.currentPageSelected.ToString(), EditorStyles.miniLabel);
            if (GUILayout.Button(new GUIContent("►"), EditorStyles.miniButton))
            {
                this.currentPageSelected += 1;
                this.currentPageSelected = Mathf.Max(this.currentPageSelected, 0);
            }
            EditorGUI.BeginChangeCheck();
            var maxElementPerPage = EditorGUILayout.IntField(new GUIContent("Max nb per page : "), this.maxElementPerPage);
            if (EditorGUI.EndChangeCheck())
            {
                this.maxElementPerPage = maxElementPerPage;
            }
            EditorGUILayout.EndHorizontal();
        }


        public int StartElementNb()
        {
            return currentPageSelected * maxElementPerPage;
        }
        public int EndElementNp()
        {
            return (currentPageSelected + 1) * maxElementPerPage;
        }
    }
}
#endif //UNITY_EDITOR