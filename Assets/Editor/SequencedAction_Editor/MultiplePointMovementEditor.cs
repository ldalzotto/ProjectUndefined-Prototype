using System;
using System.Collections.Generic;
using System.Reflection;
using SequencedAction_Editor_Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SequencedAction_Editor
{
    public class MultiplePointMovementEditor : EditorWindow
    {
        [MenuItem("EditorTool/MultiplePointMovementEditor")]
        public static void Init()
        {
            GetWindow<MultiplePointMovementEditor>().Show();
        }

        private List<TransformStructSetter> TransformStructSetters = new List<TransformStructSetter>();
        private SelectedScriptableObjectElement SelectedScriptableObjectElement;
        private RootTransformElement RootTransformElement;

        private void OnEnable()
        {
            this.SelectedScriptableObjectElement = new SelectedScriptableObjectElement(rootVisualElement, this.OnSelectedObjectChange);
            this.RootTransformElement = new RootTransformElement(rootVisualElement, this.MoveObjectsByDelta);
        }

        private void OnSelectedObjectChange(ChangeEvent<Object> evt)
        {
            this.TransformStructSetters.Clear();
            if (evt.newValue != null)
            {
                this.ProcessAttributed(evt.newValue, this.TransformStructSetters);
            }
        }

        private void MoveObjectsByDelta(Vector3 delta)
        {
            this.SelectedScriptableObjectElement.SetDirty();
            foreach (var transformStructSetter in this.TransformStructSetters)
            {
                transformStructSetter.Move(delta);
            }
        }

        private void ProcessAttributed(object obj, List<TransformStructSetter> TransformStructSetters)
        {
            var fields = ReflectionHelper.GetAllFields(obj.GetType());
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    var MultiplePointMovementAttributes = field.GetCustomAttributes<MultiplePointMovementAttribute>(true);
                    if (MultiplePointMovementAttributes != null)
                    {
                        foreach (var MultiplePointMovementAttribute in MultiplePointMovementAttributes)
                        {
                            if (MultiplePointMovementAttribute.GetType() == typeof(MultiplePointMovementNested))
                            {
                                this.ProcessAttributed(field.GetValue(obj), TransformStructSetters);
                            }
                            else if (MultiplePointMovementAttribute.GetType() == typeof(MultiplePointMovementAware))
                            {
                                TransformStructSetters.Add(new TransformStructSetter(field, obj));
                            }
                        }
                    }
                }
            }
        }
    }

    class SelectedScriptableObjectElement : VisualElement
    {
        private ObjectField ObjectField;

        public SelectedScriptableObjectElement(VisualElement parent, System.Action<ChangeEvent<Object>> OnSelectedObjectChange)
        {
            this.ObjectField = new ObjectField();
            this.ObjectField.objectType = typeof(ScriptableObject);
            this.ObjectField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                OnSelectedObjectChange(evt);
                evt.StopPropagation();
            });

            this.Add(this.ObjectField);
            parent.Add(this);
        }

        public void SetDirty()
        {
            EditorUtility.SetDirty(this.ObjectField.value);
        }
    }

    class RootTransformElement : VisualElement
    {
        private Action<Vector3> MoveAction;

        public RootTransformElement(VisualElement parent, Action<Vector3> MoveAction)
        {
            this.MoveAction = MoveAction;
            var positionField = new Vector3Field("Position");
            positionField.RegisterCallback<ChangeEvent<Vector3>>(this.OnRootPositionChanged);
            this.Add(positionField);

            var rotationField = new Vector3Field("Rotation");
            positionField.RegisterCallback<ChangeEvent<Vector3>>(this.OnRootRotationChanged);
            this.Add(rotationField);

            parent.Add(this);
        }

        private void OnRootPositionChanged(ChangeEvent<Vector3> evt)
        {
            this.MoveAction.Invoke(evt.newValue - evt.previousValue);
            evt.StopPropagation();
        }

        private void OnRootRotationChanged(ChangeEvent<Vector3> evt)
        {
            //      this.MoveAction.Invoke(evt.newValue);
            evt.StopPropagation();
        }
    }

    class TransformStructSetter
    {
        private FieldInfo Field;
        private object BaseObject;

        public TransformStructSetter(FieldInfo field, object baseObject)
        {
            Field = field;
            BaseObject = baseObject;
        }

        public void Move(Vector3 delta)
        {
            var fieldValue = Field.GetValue(this.BaseObject);
            switch (fieldValue)
            {
                case TransformStruct TransformStruct:
                    TransformStruct.WorldPosition += delta;
                    Field.SetValue(this.BaseObject, TransformStruct);
                    break;
                case Vector3 vector3:
                    vector3 += delta;
                    Field.SetValue(this.BaseObject, vector3);
                    break;
            }
        }
    }
}