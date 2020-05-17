#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class FoldableReordableList<T> : ReorderableList
    {

        [SerializeField]
        private bool displayed;

        public FoldableReordableList(List<T> list, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemovebutton, string listTitle, float elementHeightFactor, ElementCallbackDelegate elementCallbackDelegate, ElementHeightCallbackDelegate elementHeightCallBack = null)
                : base(list, typeof(T), draggable, displayHeader, displayAddButton, displayRemovebutton)
        {
            this.drawHeaderCallback = (Rect rect) =>
            {
                rect.x += 15;
                displayed = EditorGUI.Foldout(rect, displayed, listTitle, true, EditorStyles.foldout);
                if (displayed)
                {
                    this.draggable = true;
                    this.displayAdd = true;
                    this.displayRemove = true;
                }
                else
                {
                    this.draggable = false;
                    this.displayAdd = false;
                    this.displayRemove = false;
                }
            };
            this.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (displayed)
                {
                    rect.y += 2;
                    rect.height -= 5;
                    elementCallbackDelegate(rect, index, isActive, isFocused);
                }
            };
            this.elementHeightCallback = elementHeightCallBack;
            if (elementHeightCallback == null)
            {
                this.elementHeightCallback = (int index) =>
                {
                    if (displayed)
                    {
                        return elementHeight * elementHeightFactor;
                    }
                    else
                    {
                        return 0;
                    }
                };
            }
        }

        public bool Displayed { get => displayed; set => displayed = value; }
    }


    [System.Serializable]
    public class FilterFoldableReordableList<T> : FoldableReordableList<T>
    {

        [SerializeField]
        private bool isFilterEnabled;

        public FilterFoldableReordableList(List<T> list, bool draggable, bool displayHeader, bool displayAddButton,
             bool displayRemovebutton, string listTitle, float elementHeightFactor, ElementCallbackDelegate elementCallbackDelegate, Action undoRegisterAction)
             : base(list, draggable, displayHeader, displayAddButton, displayRemovebutton, listTitle, elementHeightFactor, elementCallbackDelegate)
        {
            this.drawHeaderCallback = (Rect rect) =>
            {

                Rect isFilterToggleRect = new Rect(rect);
                isFilterToggleRect.width = 10f;
                isFilterToggleRect.height = 10f;
                isFilterToggleRect.y += 3f;
                EditorGUI.BeginChangeCheck();
                var isFilterEnabled = EditorGUI.Toggle(isFilterToggleRect, this.isFilterEnabled, EditorStyles.miniButton);
                // isFilterEnabled = EditorGUI.Toggle()
                Rect foldoutRect = new Rect(rect);
                foldoutRect.x += 20;
                var displayed = EditorGUI.Foldout(foldoutRect, this.Displayed, listTitle, true);
                if (EditorGUI.EndChangeCheck())
                {
                    if (undoRegisterAction != null)
                    {
                        undoRegisterAction.Invoke();
                    }
                    this.isFilterEnabled = isFilterEnabled;
                    this.Displayed = displayed;
                }
                if (this.Displayed)
                {
                    this.draggable = true;
                    this.displayAdd = true;
                    this.displayRemove = true;
                }
                else
                {
                    this.draggable = false;
                    this.displayAdd = false;
                    this.displayRemove = false;
                }
            };
        }

        public bool IsFilterEnabled { get => isFilterEnabled; set => isFilterEnabled = value; }

        public void DisableFilter()
        {
            this.isFilterEnabled = false;
        }

        internal void ColapseFilter()
        {
            this.Displayed = false;
        }
    }
}

#endif //UNITY_EDITOR