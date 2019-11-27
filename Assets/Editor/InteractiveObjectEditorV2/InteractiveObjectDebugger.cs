using System;
using System.Collections.Generic;
using System.Linq;
using Editor_MainGameCreationWizard;
using InteractiveObjects;
using RangeObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveObjectDebugger : EditorWindow
{
    public static InteractiveObjectDebugger Instance;
    private Dictionary<string, VisualElement> ClassHeaderElements = new Dictionary<string, VisualElement>();
    private CommonGameConfigurations CommonGameConfigurations;

    private VisualElement LeftPanel;

    private Dictionary<object, ListenedObjectField> ListenableObjectFields = new Dictionary<object, ListenedObjectField>();

    private ScrollView ObjectFieldParent;

    private VisualElement RootElement;
    private TextField SeachTextElement;
    private SelectedInteractiveObjectDetail SelectedInteractiveObjectDetail;

    [MenuItem("InteractiveObject/InteractiveObjectDebugger")]
    public static void Init()
    {
        var wnd = GetWindow<InteractiveObjectDebugger>();
        Instance = wnd;
        wnd.Show();
    }

    private void OnEnable()
    {
        CommonGameConfigurations = new CommonGameConfigurations();
        EditorInformationsHelper.InitProperties(ref CommonGameConfigurations);

        RootElement = new VisualElement();
        RootElement.style.flexDirection = FlexDirection.Row;

        LeftPanel = new VisualElement();
        LeftPanel.style.flexDirection = FlexDirection.Column;
        LeftPanel.style.alignSelf = Align.FlexStart;

        SeachTextElement = new TextField();
        SeachTextElement.RegisterCallback<ChangeEvent<string>>(OnSearchTextChange);
        LeftPanel.Add(SeachTextElement);

        ObjectFieldParent = new ScrollView(ScrollViewMode.Vertical);
        LeftPanel.Add(ObjectFieldParent);

        RootElement.Add(LeftPanel);
        SelectedInteractiveObjectDetail = new SelectedInteractiveObjectDetail(RootElement);

        rootVisualElement.Add(RootElement);

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.update += Tick;
        SceneView.duringSceneGui += SceneTick;
    }

    private void OnDestroy()
    {
        Instance = null;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.update -= Tick;
        SceneView.duringSceneGui -= SceneTick;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoaded()
    {
        //  Debug.Break();
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode) OnPlayModeEnter();
    }

    private void OnPlayModeEnter()
    {
        InteractiveObjectEventsManagerSingleton.Get().RegisterOnAllInteractiveObjectCreatedEventListener(delegate(CoreInteractiveObject interactiveObject)
        {
            this.OnObjectCreated(interactiveObject);
            interactiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnObjectDestroyed);
        });
        RangeEventsManager.Get().RegisterOnRangeObjectCreatedEventListener(delegate(RangeObjectV2 rangeObject)
        {
            this.OnObjectCreated(rangeObject);
            rangeObject.RegisterOnRangeObjectDestroyedEventListener(this.OnObjectDestroyed);
        });
    }

    private void Tick()
    {
        if (Application.isPlaying)
        {
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            foreach (var interactiveObject in allInteractiveObjects) OnObjectCreated(interactiveObject);

            var allRangeObjects = RangeObjectV2Manager.Get().RangeObjects;
            foreach (var rangeObject in allRangeObjects) OnObjectCreated(rangeObject);

            SelectedInteractiveObjectDetail.OnGui();
        }
    }

    private void SceneTick(SceneView sceneView)
    {
        foreach (var interactiveObjectField in ListenableObjectFields.Values) interactiveObjectField.SceneTick(CommonGameConfigurations);
    }

    private void OnObjectCreated(object interactiveObject)
    {
        ListenableObjectFields.TryGetValue(interactiveObject, out var InteractiveObjectField);
        if (InteractiveObjectField == null)
        {
            InteractiveObjectField = new ListenedObjectField(ObjectFieldParent, interactiveObject,
                OnInteractiveObjectFieldClicked);
            ListenableObjectFields.Add(interactiveObject, InteractiveObjectField);

            ClassHeaderElements.TryGetValue(interactiveObject.GetType().Name, out var header);
            if (header == null)
            {
                header = new VisualElement();
                var headerText = new TextElement()
                {
                    text = interactiveObject.GetType().Name,
                };

                headerText.style.borderTopWidth = 3f;
                headerText.style.borderBottomWidth = 3f;
                headerText.style.unityFontStyleAndWeight = FontStyle.Bold;

                header.Add(headerText);
                ClassHeaderElements[interactiveObject.GetType().Name] = header;
                ObjectFieldParent.Add(header);
            }

            header.Add(ListenableObjectFields[interactiveObject]);
        }
    }

    private void OnObjectDestroyed(object CoreInteractiveObject)
    {
        ListenableObjectFields.TryGetValue(CoreInteractiveObject, out var InteractiveObjectField);
        if (InteractiveObjectField != null)
        {
            if (SelectedInteractiveObjectDetail.CurrentInteracitveObjectFieldSelected == InteractiveObjectField) SelectedInteractiveObjectDetail.ResetElement();
            ClassHeaderElements.TryGetValue(CoreInteractiveObject.GetType().Name, out var header);
            if (header != null) header.Remove(InteractiveObjectField);

            ListenableObjectFields.Remove(CoreInteractiveObject);
        }
    }


    private void OnSearchTextChange(ChangeEvent<string> evt)
    {
        if (Application.isPlaying)
        {
            var allInteractiveObjects = InteractiveObjectV2Manager.Get().InteractiveObjects;
            var allRangeObjects = RangeObjectV2Manager.Get().RangeObjects;

            var allInteractiveObjectsClassname = allInteractiveObjects.ToList()
                .Select(i => i).Where(i => string.IsNullOrEmpty(SeachTextElement.text) || i.InteractiveGameObject.InteractiveGameObjectParent.name.ToLower().Contains(SeachTextElement.text.ToLower())).ToList()
                .ConvertAll(i => i.GetType().Name)
                .Union(
                    allRangeObjects.Select(r => r).Where(r => string.IsNullOrEmpty(SeachTextElement.text) || r.RangeGameObjectV2.RangeGameObject.name.ToLower().Contains(SeachTextElement.text.ToLower())).ToList()
                        .ConvertAll(r => r.GetType().Name)
                )
                .ToList();
            foreach (var classHeaderElement in ClassHeaderElements) classHeaderElement.Value.style.display = allInteractiveObjectsClassname.Contains(classHeaderElement.Key) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void OnInteractiveObjectFieldClicked(MouseDownEvent MouseDownEvent, ListenedObjectField InteractiveObjectField)
    {
        foreach (var interactiveObjectField in ListenableObjectFields.Values)
        {
            interactiveObjectField.SetIsSelected(interactiveObjectField == InteractiveObjectField);
            SelectedInteractiveObjectDetail.OnInteractiveObjectSelected(InteractiveObjectField);
        }
    }
}

internal class ListenedObjectField : VisualElement
{
    private Color InitialBackGroundColor;
    private BoolVariable IsSelected;
    private ObjectFieldIconBar ObjectFieldIconBar;

    private Label ObjectLabel;

    private Action<MouseDownEvent, ListenedObjectField> OnInteractiveObjectFieldClicked;

    public ListenedObjectField(VisualElement parent, object listenedField, Action<MouseDownEvent, ListenedObjectField> OnInteractiveObjectFieldClicked = null)
    {
        ListenedObjectRef = listenedField;

        switch (listenedField)
        {
            case CoreInteractiveObject coreInteractiveObject:
                ObjectReference = coreInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent;
                break;
            case RangeObjectV2 rangeObjectV2:
                ObjectReference = rangeObjectV2.RangeGameObjectV2.RangeGameObject;
                break;
        }

        this.OnInteractiveObjectFieldClicked = OnInteractiveObjectFieldClicked;
        IsSelected = new BoolVariable(false, OnInteractiveObjectSelected, OnInteractiveObjetDeSelected);

        InitialBackGroundColor = style.backgroundColor.value;

        ObjectFieldIconBar = new ObjectFieldIconBar(this);

        ObjectLabel = new Label(ObjectReference.name);
        ObjectLabel.style.marginLeft = 10f;

        Add(ObjectLabel);


        parent.Add(this);

        style.flexDirection = FlexDirection.Row;

        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseOutEvent>(OnMouseExit);
        RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    public GameObject ObjectReference { get; private set; }
    public object ListenedObjectRef { get; private set; }

    public void SetIsSelected(bool value)
    {
        IsSelected.SetValue(value);
    }

    public void SceneTick(CommonGameConfigurations CommonGameConfigurations)
    {
        if (ObjectFieldIconBar.IsSceneHandleEnabled() && ObjectReference != null) SceneHandlerDrawer.Draw(ListenedObjectRef, ObjectReference.transform, CommonGameConfigurations);
    }


    private void OnMouseDown(MouseDownEvent MouseDownEvent)
    {
        Selection.activeGameObject = ObjectReference;
        if (OnInteractiveObjectFieldClicked != null) OnInteractiveObjectFieldClicked.Invoke(MouseDownEvent, this);
        MouseDownEvent.StopPropagation();
    }

    private void OnMouseEnter(MouseEnterEvent MouseEnterEvent)
    {
        if (!IsSelected.GetValue()) style.backgroundColor = Color.gray;
    }

    private void OnMouseExit(MouseOutEvent MouseOutEvent)
    {
        if (!IsSelected.GetValue()) style.backgroundColor = InitialBackGroundColor;
    }

    private void OnInteractiveObjectSelected()
    {
        style.backgroundColor = Color.cyan;
    }

    private void OnInteractiveObjetDeSelected()
    {
        style.backgroundColor = InitialBackGroundColor;
    }
}

internal class ObjectFieldIconBar : VisualElement
{
    private VisualElement Root;

    private ObjectFieldSelectionIcon SceneHandleSelection;

    public ObjectFieldIconBar(VisualElement parent)
    {
        Root = new VisualElement();
        Root.style.alignSelf = Align.FlexEnd;
        Root.style.flexDirection = FlexDirection.Row;
        SceneHandleSelection = new ObjectFieldSelectionIcon(Root, "G");
        Add(Root);
        style.marginLeft = 5f;
        parent.Add(this);
    }

    public bool IsSceneHandleEnabled()
    {
        return SceneHandleSelection.Selected.GetValue();
    }
}

public class ObjectFieldSelectionIcon : VisualElement
{
    private Color initialBackgroundColor;

    public ObjectFieldSelectionIcon(VisualElement parent, string label)
    {
        Selected = new BoolVariable(false, OnSelected, OnUnSelected);
        initialBackgroundColor = style.backgroundColor.value;
        var text = new Label(label);
        Add(text);
        RegisterCallback<MouseDownEvent>(OnClicked);
        parent.Add(this);
    }

    public BoolVariable Selected { get; private set; }

    private void OnClicked(MouseDownEvent evt)
    {
        Selected.SetValue(!Selected.GetValue());
        evt.StopPropagation();
    }

    private void OnSelected()
    {
        style.backgroundColor = Color.yellow;
    }

    private void OnUnSelected()
    {
        style.backgroundColor = initialBackgroundColor;
    }
}

internal class SelectedInteractiveObjectDetail : VisualElement
{
    private VisualElement CurrentElement;
    private List<IListenableVisualElement> CurrentIListenableVisualElementRefrerences = new List<IListenableVisualElement>();

    public SelectedInteractiveObjectDetail(VisualElement parent)
    {
        parent.Add(this);
        style.flexGrow = 2f;
    }

    public ListenedObjectField CurrentInteracitveObjectFieldSelected { get; private set; }

    public void OnGui()
    {
        foreach (var IListenableVisualElement in CurrentIListenableVisualElementRefrerences) IListenableVisualElement.Refresh();
    }

    public void ResetElement()
    {
        CurrentInteracitveObjectFieldSelected = null;
        CurrentIListenableVisualElementRefrerences.Clear();
        if (CurrentElement != null)
        {
            Remove(CurrentElement);
            CurrentElement = null;
        }
    }

    public void OnInteractiveObjectSelected(ListenedObjectField interactiveObjectField)
    {
        ResetElement();
        CurrentInteracitveObjectFieldSelected = interactiveObjectField;

        var elem = VisualElementFromClass.BuildVisualElement(interactiveObjectField.ListenedObjectRef, ref CurrentIListenableVisualElementRefrerences);
        Add(elem);
        CurrentElement = elem;
    }
}

internal class NoSpaceToggle : Toggle
{
    public NoSpaceToggle(VisualElement parent, string label = "")
    {
        this.label = label;
        if (!string.IsNullOrEmpty(label)) this.Q<Label>().style.minWidth = 0f;
        parent.Add(this);
    }
}