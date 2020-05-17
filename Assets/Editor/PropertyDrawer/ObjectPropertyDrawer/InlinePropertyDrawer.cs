using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(Inline))]
public class InlinePropertyDrawer : PropertyDrawer
{
    private bool folded;
    private Editor inlineEditor;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            Inline byEnumProperty = (Inline) attribute;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);

            EditorGUI.BeginChangeCheck();
            this.folded = EditorGUILayout.Foldout(this.folded, property.name, true);
            if (EditorGUI.EndChangeCheck())
            {
                if (!this.folded)
                {
                    if (this.inlineEditor != null)
                    {
                        Editor.DestroyImmediate(inlineEditor);
                        this.inlineEditor = null;
                    }
                }
            }

            if (this.folded)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(property);
                EditorGUI.indentLevel -= 1;

                if (property.objectReferenceValue != null)
                {
                    if (this.inlineEditor == null)
                    {
                        inlineEditor = DynamicEditorCreation.Get().CreateEditor(property.objectReferenceValue);
                    }

                    if (this.inlineEditor != null)
                    {
                        EditorGUI.indentLevel += 1;
                        this.inlineEditor.OnInspectorGUI();
                        EditorGUI.indentLevel -= 1;
                    }
                }
                else
                {
                    if (EditorUtility.IsPersistent(property.serializedObject.targetObject))
                    {
                        if (byEnumProperty.CreateAtSameLevelIfAbsent)
                        {
                            if (GUILayout.Button(new GUIContent("CREATE")))
                            {
                                var fieldType = SerializableObjectHelper.GetPropertyFieldInfo(property).FieldType;
                                if (fieldType.IsAbstract || fieldType.IsInterface)
                                {
                                    //Open popup to select implementation
                                    ClassSelectionEditorWindow.Show(Event.current.mousePosition, fieldType, (Type selectedType) =>
                                    {
                                        var createdAsset = AssetHelper.CreateAssetAtSameDirectoryLevel((ScriptableObject) property.serializedObject.targetObject, selectedType.Name, property.name);
                                        property.objectReferenceValue = (Object) createdAsset;
                                    });
                                }
                                else
                                {
                                    var createdAsset = AssetHelper.CreateAssetAtSameDirectoryLevel((ScriptableObject) property.serializedObject.targetObject, property.type.Replace("PPtr<$", "").Replace(">", ""), property.name);
                                    property.objectReferenceValue = (Object) createdAsset;
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.EndProperty();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void CreateByEnumSO(SerializedProperty property, Inline inlineAttribute)
    {
        this.ClearObject(property);
        var sanitizedType = property.type.Replace("PPtr<$", "").Replace(">", "");
        var instanciatedProperty = ScriptableObject.CreateInstance(sanitizedType);
        instanciatedProperty.name = inlineAttribute.FileName;
        AssetDatabase.AddObjectToAsset(instanciatedProperty, property.serializedObject.targetObject);
        property.objectReferenceValue = instanciatedProperty;
    }

    private void ClearObject(SerializedProperty property)
    {
        List<object> ObjectsByEnum = new List<object>();
        foreach (var field in property.serializedObject.targetObject.GetType().GetFields())
        {
            var inlinesAttribute = field.GetCustomAttributes(typeof(Inline), true).Cast<Inline>().ToArray();
            if (inlinesAttribute != null && inlinesAttribute.Length > 0)
            {
                ObjectsByEnum.Add(field.GetValue(property.serializedObject.targetObject));
            }
        }

        var ActualAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(property.serializedObject.targetObject)).ToList();

        ActualAssets.Remove(property.serializedObject.targetObject);

        foreach (var ActualAsset in ActualAssets)
        {
            if (ActualAsset != null)
            {
                if (!ObjectsByEnum.Contains(ActualAsset))
                {
                    AssetDatabase.RemoveObjectFromAsset(ActualAsset);
                }
            }
        }
    }
}


class ClassSelectionEditorWindow : EditorWindow
{
    private static Vector2 DefaultSize = new Vector2(150, 300);
    private static Dictionary<string, Type> AvailableTypes;
    private static Action<Type> OnSelectionCallback;

    public static void Show(
        Vector2 displayPosition,
        Type abstractType, Action<Type> OnSelectionCallback)
    {
        var allTypes = TypeHelper.GetAllTypeAssignableFrom(abstractType);
        AvailableTypes = new Dictionary<string, Type>();
        foreach (var singleType in allTypes)
        {
            AvailableTypes[singleType.Name] = singleType;
        }

        ClassSelectionEditorWindow.OnSelectionCallback = OnSelectionCallback;
        var window = CreateInstance<ClassSelectionEditorWindow>();
        window.position = new Rect(displayPosition, DefaultSize);
        window.Show();
    }

    private VisualElement TypeSelectionLineParentElement;
    private Dictionary<string, SelectionLine> TypeSelectionLineVisualElements = new Dictionary<string, SelectionLine>();

    private void OnEnable()
    {
        var RootElement = new VisualElement();

        var SearchTextElement = new TextField();
        SearchTextElement.RegisterCallback<ChangeEvent<string>>(this.OnSearchTextChanged);
        RootElement.Add(SearchTextElement);

        this.TypeSelectionLineParentElement = new VisualElement();

        foreach (var availableType in AvailableTypes)
        {
            var element = new SelectionLine(this.TypeSelectionLineParentElement, availableType.Key, availableType.Value, this.OnTypeSelected);
            TypeSelectionLineVisualElements.Add(availableType.Key, element);
        }

        RootElement.Add(this.TypeSelectionLineParentElement);

        this.rootVisualElement.Add(RootElement);
    }


    private void OnSearchTextChanged(ChangeEvent<string> evt)
    {
        foreach (var TypeSelectionLineVisualElement in TypeSelectionLineVisualElements)
        {
            TypeSelectionLineVisualElement.Value.style.display =
                (string.IsNullOrEmpty(evt.newValue) || TypeSelectionLineVisualElement.Key.ToLower().Contains(evt.newValue.ToLower())) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void OnTypeSelected(Type selectedType)
    {
        OnSelectionCallback.Invoke(selectedType);
        this.Close();
    }

    class SelectionLine : Label
    {
        private Action<Type> OnSelectionLineClicked;
        private Type associatedType;

        public SelectionLine(VisualElement parent, string label,
            Type associatedType, Action<Type> OnSelectionLineClicked) : base(label)
        {
            parent.Add(this);
            this.associatedType = associatedType;
            this.OnSelectionLineClicked = OnSelectionLineClicked;
            this.RegisterCallback<MouseUpEvent>(this.OnClicked);
        }

        private void OnClicked(MouseUpEvent MouseUpEvent)
        {
            this.OnSelectionLineClicked.Invoke(this.associatedType);
        }
    }
}