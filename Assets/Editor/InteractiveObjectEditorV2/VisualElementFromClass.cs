using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InteractiveObjects;
using RangeObjects;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementFromClass
{
    public static VisualElement BuildVisualElement(object obj, ref List<IListenableVisualElement> CreatedIListenableVisualElements)
    {
        var root = new VisualElement();
        if (obj != null)
        {
            foreach (var field in ReflectionHelper.GetAllFields(obj.GetType()))
            {
                var continueLoop = false;
                foreach (var customAttribute in field.GetCustomAttributes<A_VEAttribute>())
                {
                    switch (customAttribute)
                    {
                        case VE_Nested vE_Nested:
                            var childElement = BuildVisualElement(field.GetValue(obj), ref CreatedIListenableVisualElements);
                            var element = new FoldableElement(childElement, root);
                            element.text = field.Name;
                            root.Add(element);
                            break;
                        case VE_Ignore vE_Ignore:
                            continueLoop = true;
                            break;
                    }

                    if (continueLoop) break;
                }

                if (continueLoop) continue;

                var IListenableVisualElement = BuildIListenableVisualElementFromMember(obj, field);

                if (IListenableVisualElement != null)
                {
                    CreatedIListenableVisualElements.Add(IListenableVisualElement);
                    root.Add((VisualElement) IListenableVisualElement);
                }
            }
        }


        return root;
    }

    public static IListenableVisualElement BuildIListenableVisualElementFromMember(object obj, FieldInfo field)
    {
        IListenableVisualElement IListenableVisualElement = null;

        if (field.FieldType == typeof(Vector3))
        {
            IListenableVisualElement = new Vector3ListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(bool))
        {
            IListenableVisualElement = new BoolListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(float))
        {
            IListenableVisualElement = new FloatListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (typeof(Enum).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new EnumListenableField(obj, field, SanitizeFieldName(field.Name));
        }
        else if (field.FieldType == typeof(BoolVariable))
        {
            var boolVariable = (BoolVariable) field.GetValue(obj);
            var f = boolVariable.GetType().GetField("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            IListenableVisualElement = new BoolListenableField(boolVariable, f, SanitizeFieldName(field.Name));
        }
        else if (typeof(CoreInteractiveObject).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new CoreInteractiveObjectListenableField(obj, field);
        }
        else if (typeof(RangeObjectV2).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new RangeObjectV2ListenableField(obj, field);
        }
        else if (typeof(IDictionary).IsAssignableFrom(field.FieldType))
        {
            IListenableVisualElement = new IDictionaryListenableField(obj, field);
        }
        else if (field.GetCustomAttribute<VE_Array>() != null)
        {
            IListenableVisualElement = new IEnumerableListenableField(obj, field);
        }

        return IListenableVisualElement;
    }

    public static void RemoveAllIListenableVisualElementNested(VisualElement element, ref List<IListenableVisualElement> removedElements)
    {
        List<VisualElement> elementsToRemove = null;
        var childrenEnumerator = element.Children().GetEnumerator();
        var i = 0;
        while (childrenEnumerator.MoveNext())
        {
            if (childrenEnumerator.Current.childCount > 0) RemoveAllIListenableVisualElementNested(childrenEnumerator.Current, ref removedElements);

            if (typeof(IListenableVisualElement).IsAssignableFrom(childrenEnumerator.Current.GetType()))
            {
                if (elementsToRemove == null) elementsToRemove = new List<VisualElement>();
                elementsToRemove.Add(childrenEnumerator.Current);
            }

            i += 1;
        }

        if (elementsToRemove != null)
            foreach (var elementToRemove in elementsToRemove)
            {
                if (removedElements == null) removedElements = new List<IListenableVisualElement>();
                removedElements.Add(elementToRemove as IListenableVisualElement);
                element.Remove(elementToRemove);
            }
    }

    public static string SanitizeFieldName(string rawFieldName)
    {
        return rawFieldName.Replace("<", "").Replace(">", "").Replace("k__BackingField", "");
    }
}

public interface IListenableVisualElement
{
    void Refresh();
}

public abstract class ListenableVisualElement<T> : VisualElement, IListenableVisualElement
{
    protected FieldInfo field;
    protected object obj;
    protected T value;

    protected ListenableVisualElement(object obj, FieldInfo field)
    {
        this.obj = obj;
        this.field = field;
    }

    public void Refresh()
    {
        var fieldValue = field.GetValue(obj);
        if (fieldValue != null)
        {
            value = (T) fieldValue;
            OnValueChaged();
        }
    }

    protected abstract void OnValueChaged();
}

internal class FoldableElement : Foldout
{
    private VisualElement InnerElement;

    public FoldableElement(VisualElement innerElement, VisualElement parent = null)

    {
        value = false;
        InnerElement = innerElement;

        if (parent != null)
        {
            this.SetParent(parent);
        }

        RegisterCallback<ChangeEvent<bool>>(OnFoldableChange);
        Add(innerElement);
    }

    public void SetParent(VisualElement parent)
    {
        InnerElement.style.marginLeft = parent.style.marginLeft.value.value + 5f;
        parent.Add(this);
    }

    private void OnFoldableChange(ChangeEvent<bool> evt)
    {
        InnerElement.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        evt.StopPropagation();
    }
}

internal class Vector3ListenableField : ListenableVisualElement<Vector3>
{
    private Vector3Field Vector3Field;

    public Vector3ListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        Vector3Field = new Vector3Field(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(Vector3Field);
    }

    protected override void OnValueChaged()
    {
        Vector3Field.value = value;
    }
}

internal class BoolListenableField : ListenableVisualElement<bool>
{
    private VisualElement Text;
    private Toggle Toggle;

    public BoolListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        style.flexDirection = FlexDirection.Row;
        Text = new Label(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(Text);
        Toggle = new Toggle();
        Add(Toggle);
    }

    protected override void OnValueChaged()
    {
        Toggle.value = value;
    }
}

internal class FloatListenableField : ListenableVisualElement<float>
{
    private FloatField FloatField;

    public FloatListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        FloatField = new FloatField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(FloatField);
    }

    protected override void OnValueChaged()
    {
        FloatField.value = value;
    }
}

internal class EnumListenableField : ListenableVisualElement<Enum>
{
    private EnumField EnumField;


    public EnumListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.EnumField = new EnumField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(this.EnumField);
    }

    protected override void OnValueChaged()
    {
        this.EnumField.value = value;
    }
}

internal class IDictionaryListenableField : ListenableVisualElement<IDictionary>
{
    private FoldableElement DictionaryFoldoutElement;
    private VisualElement DictionaryValuesElement;

    private Dictionary<object, object> AddedVisualElementsObjects = new Dictionary<object, object>();
    private Dictionary<object, VisualElement> KeyToFoldoutElement = new Dictionary<object, VisualElement>();
    private Dictionary<VisualElement, List<IListenableVisualElement>> DictionaryEntryElements = new Dictionary<VisualElement, List<IListenableVisualElement>>();

    public IDictionaryListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        this.DictionaryValuesElement = new VisualElement();
        this.DictionaryFoldoutElement = new FoldableElement(this.DictionaryValuesElement, this);
        this.DictionaryFoldoutElement.text = string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label;
        this.DictionaryFoldoutElement.Add(this.DictionaryValuesElement);
        this.Add(this.DictionaryFoldoutElement);
    }

    protected override void OnValueChaged()
    {
        /// Creation of new keys

        foreach (var key in this.value.Keys)
        {
            if (!this.AddedVisualElementsObjects.ContainsKey(key))
            {
                this.AddedVisualElementsObjects[key] = this.value[key];

                VisualElement entryElement = new VisualElement();
                FoldableElement entryFoldableElement = new FoldableElement(entryElement, this.DictionaryValuesElement);
                List<IListenableVisualElement> valueListenabledVisualElements = new List<IListenableVisualElement>();
                VisualElementFromClass.BuildVisualElement(this.value[key], ref valueListenabledVisualElements);
                foreach (var valueListenabledVisualElement in valueListenabledVisualElements)
                {
                    entryElement.Add(valueListenabledVisualElement as VisualElement);
                }

                this.DictionaryEntryElements[entryFoldableElement] = valueListenabledVisualElements;

                this.KeyToFoldoutElement[key] = entryFoldableElement;
                this.DictionaryValuesElement.Add(entryFoldableElement);
                entryFoldableElement.text = key.ToString();
            }

            foreach (var dictionaryEntryElementsValues in DictionaryEntryElements.Values)
            {
                foreach (var dictionaryEntryElementsValue in dictionaryEntryElementsValues)
                {
                    dictionaryEntryElementsValue.Refresh();
                }
            }
        }


        /// Destruction of keys removed
        var referenceKeys = this.value.Keys.Cast<object>().ToList();
        var veKeysToRemove = this.AddedVisualElementsObjects.Keys.ToList();
        veKeysToRemove.RemoveAll(
            delegate(object o) { return referenceKeys.Contains(o); }
        );

        foreach (var veKeyToRemove in veKeysToRemove)
        {
            this.AddedVisualElementsObjects.Remove(veKeyToRemove);
            this.DictionaryValuesElement.Remove(this.KeyToFoldoutElement[veKeyToRemove]);
            this.DictionaryEntryElements.Remove(this.KeyToFoldoutElement[veKeyToRemove]);
            
            this.KeyToFoldoutElement.Remove(veKeyToRemove);
        }
    }
}

internal class CoreInteractiveObjectListenableField : ListenableVisualElement<CoreInteractiveObject>
{
    private ObjectField ObjectField;

    public CoreInteractiveObjectListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(ObjectField);
    }

    protected override void OnValueChaged()
    {
        ObjectField.value = value.InteractiveGameObject.InteractiveGameObjectParent;
    }
}

internal class RangeObjectV2ListenableField : ListenableVisualElement<RangeObjectV2>
{
    private ObjectField ObjectField;

    public RangeObjectV2ListenableField(object obj, FieldInfo field) : base(obj, field)
    {
        ObjectField = new ObjectField(VisualElementFromClass.SanitizeFieldName(field.Name));
        Add(ObjectField);
    }

    protected override void OnValueChaged()
    {
        ObjectField.value = value.RangeGameObjectV2.RangeGameObject;
    }
}

internal class GameObjectListenableField : ListenableVisualElement<GameObject>
{
    private ObjectField ObjectField;

    public GameObjectListenableField(object obj, FieldInfo field, string label = "") : base(obj, field)
    {
        ObjectField = new ObjectField(string.IsNullOrWhiteSpace(label) ? VisualElementFromClass.SanitizeFieldName(field.Name) : label);
        Add(ObjectField);
    }

    protected override void OnValueChaged()
    {
        ObjectField.value = value;
    }
}

internal class IEnumerableListenableField : ListenableVisualElement<IEnumerable>
{
    private Dictionary<object, VisualElement> EnumerableElements = new Dictionary<object, VisualElement>();
    private List<IListenableVisualElement> listenableVisualElements = new List<IListenableVisualElement>();
    private VisualElement RootElement;

    private List<object> ValueIenumerableAsList = new List<object>();

    public IEnumerableListenableField(object obj, FieldInfo field) : base(obj, field)
    {
        RootElement = new VisualElement();
        RootElement.style.marginLeft = 5f;
        Add(RootElement);
    }

    private void ProcessIEnumerable(IEnumerable IEnumerable)
    {
        ValueIenumerableAsList.Clear();
        var enumerator = IEnumerable.GetEnumerator();
        var i = 0;
        while (enumerator.MoveNext())
        {
            var obj = enumerator.Current;
            ValueIenumerableAsList.Add(obj);
            if (!EnumerableElements.ContainsKey(obj))
            {
                var ve = VisualElementFromClass.BuildVisualElement(obj, ref listenableVisualElements);
                var foldable = new FoldableElement(ve, RootElement);
                foldable.text = field.Name + "_" + i;
                EnumerableElements.Add(obj, foldable);
            }

            i += 1;
        }

        foreach (var EnumerableElement in EnumerableElements.Keys.ToList())
            if (!ValueIenumerableAsList.Contains(EnumerableElement))
            {
                List<IListenableVisualElement> RemovedIListenableVisualElements = null;
                VisualElementFromClass.RemoveAllIListenableVisualElementNested(EnumerableElements[EnumerableElement], ref RemovedIListenableVisualElements);
                if (RemovedIListenableVisualElements != null)
                    foreach (var RemovedIListenableVisualElement in RemovedIListenableVisualElements)
                        listenableVisualElements.Remove(RemovedIListenableVisualElement);

                RootElement.Remove(EnumerableElements[EnumerableElement]);
                EnumerableElements.Remove(EnumerableElement);
            }
    }

    protected override void OnValueChaged()
    {
        ProcessIEnumerable(value);
        foreach (var listenableVisualElement in listenableVisualElements) listenableVisualElement.Refresh();
    }
}