﻿using Editor_MainGameCreationWizard;
using InteractiveObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using RangeObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveObjectExplorer : EditorWindow
{
    [MenuItem("InteractiveObject/InteractiveObjectExplorer")]
    public static void Init()
    {
        var wnd = GetWindow<InteractiveObjectExplorer>();
        wnd.Show();
    }

    private CommonGameConfigurations CommonGameConfigurations;

    private VisualElement RootElement;
    private ContextBar ContextBar;
    private TextField SearchTextField;
    private ScrollView InteractiveObjectSelectionScroll;

    private List<MonoBehaviour> InteractiveObjectInitializers = new List<MonoBehaviour>();
    private Dictionary<MonoBehaviour, InterativeObjectInitializerLine> InteractiveObjectInitializerLines = new Dictionary<MonoBehaviour, InterativeObjectInitializerLine>();

    private void OnEnable()
    {
        this.CommonGameConfigurations = new CommonGameConfigurations();
        EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations);

        this.RootElement = new VisualElement();

        this.ContextBar = new ContextBar(this.RootElement);
        this.ContextBar.SetupRefreshButton(this.Refresh);
        this.ContextBar.SetupSelectedAllButton(this.SelectAllGizmo);
        this.ContextBar.SetupUnselectedAllButton(this.UnSelectAllGizmo);

        this.SearchTextField = new TextField();
        this.SearchTextField.RegisterCallback<ChangeEvent<string>>(this.OnSearchStringChange);
        this.RootElement.Add(this.SearchTextField);

        this.InteractiveObjectSelectionScroll = new ScrollView(ScrollViewMode.Vertical);
        this.RootElement.Add(this.InteractiveObjectSelectionScroll);

        rootVisualElement.Add(this.RootElement);

        // rootVisualElement.RegisterCallback<MouseUpEvent>(this.RootMouseDown);

        SceneView.duringSceneGui += this.SceneTick;
    }

    private void RootMouseDown(MouseUpEvent evt)
    {
        evt.StopPropagation();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= this.SceneTick;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.SceneTick;
    }

    private void SceneTick(SceneView sceneView)
    {
        foreach (var InteractiveObjectInitializerLine in InteractiveObjectInitializerLines)
        {
            if (InteractiveObjectInitializerLine.Value.IsGizmoSelected())
            {
                SceneHandlerDrawer.Draw(InteractiveObjectInitializerLine.Key, InteractiveObjectInitializerLine.Key.transform, this.CommonGameConfigurations);
            }
        }
    }

    private void OnSearchStringChange(ChangeEvent<string> evt)
    {
        foreach (var InteractiveObjectInitializerLine in this.InteractiveObjectInitializerLines)
        {
            InteractiveObjectInitializerLine.Value.IsDisplayed.SetValue((string.IsNullOrEmpty(this.SearchTextField.value) || InteractiveObjectInitializerLine.Key.name.ToLower().Contains(this.SearchTextField.value.ToLower())));
        }
    }

    private void Refresh()
    {
        this.InteractiveObjectInitializers =
            GameObject.FindObjectsOfType<InteractiveObjectInitializer>().ToList().ConvertAll(c => c as MonoBehaviour);
        this.InteractiveObjectInitializers.AddRange(GameObject.FindObjectsOfType<RangeObjectInitializer>().ToList().ConvertAll(c => c as MonoBehaviour));

        foreach (var interactiveObjectInitializer in this.InteractiveObjectInitializers)
        {
            if (!this.InteractiveObjectInitializerLines.ContainsKey(interactiveObjectInitializer))
            {
                this.InteractiveObjectInitializerLines[interactiveObjectInitializer] = new InterativeObjectInitializerLine(this.InteractiveObjectSelectionScroll, interactiveObjectInitializer.gameObject, this.OnInteractiveObjectLineClicked);
            }
        }

        foreach (var interactiveObjectInitializerKey in this.InteractiveObjectInitializerLines.Keys.ToList())
        {
            if (!this.InteractiveObjectInitializers.Contains(interactiveObjectInitializerKey))
            {
                this.InteractiveObjectSelectionScroll.Remove(this.InteractiveObjectInitializerLines[interactiveObjectInitializerKey]);
                this.InteractiveObjectInitializerLines.Remove(interactiveObjectInitializerKey);
            }
        }
    }

    private void UnSelectAllGizmo()
    {
        foreach (var interactiveObjectInitializerKey in this.InteractiveObjectInitializerLines)
        {
            interactiveObjectInitializerKey.Value.GizmoIcon.Selected.SetValue(false);
        }
    }

    private void SelectAllGizmo()
    {
        foreach (var interactiveObjectInitializerKey in this.InteractiveObjectInitializerLines)
        {
            if (interactiveObjectInitializerKey.Value.IsDisplayed.GetValue())
            {
                interactiveObjectInitializerKey.Value.GizmoIcon.Selected.SetValue(true);
            }
        }
    }

    private void OnInteractiveObjectLineClicked(InterativeObjectInitializerLine InterativeObjectInitializerLine)
    {
        foreach (var interactiveObjectLine in this.InteractiveObjectInitializerLines.Values)
        {
            interactiveObjectLine.IsSelected.SetValue(interactiveObjectLine == InterativeObjectInitializerLine);
        }
    }

    public void SelectGizmoFromInteractiveObjectComponent(MonoBehaviour InteractiveObjectInitializerComponent)
    {
        this.InteractiveObjectInitializerLines.TryGetValue(InteractiveObjectInitializerComponent, out InterativeObjectInitializerLine InterativeObjectInitializerLine);
        if (InterativeObjectInitializerLine != null)
        {
            InterativeObjectInitializerLine.SelectGizmo();
        }
    }
}

class ContextBar : VisualElement
{
    private ContextButton RefreshButton;
    private ContextButton SelectAllButton;
    private ContextButton UnselectAllButton;

    public ContextBar(VisualElement parent)
    {
        this.style.flexDirection = FlexDirection.Row;
        parent.Add(this);
    }

    public void SetupRefreshButton(Action OnSelected)
    {
        this.RefreshButton = new ContextButton(this, "Refresh", OnSelected);
    }

    public void SetupSelectedAllButton(Action OnSelected)
    {
        this.SelectAllButton = new ContextButton(this, "S_All", OnSelected);
    }

    public void SetupUnselectedAllButton(Action OnSelected)
    {
        this.UnselectAllButton = new ContextButton(this, "U_All", OnSelected);
    }
}

class ContextButton : Button
{
    private BoolVariable IsSelected;
    private Action OnSelectionActionListener;

    public ContextButton(VisualElement parent, string label, Action OnSelectionActionListener)
    {
        this.OnSelectionActionListener = OnSelectionActionListener;
        this.IsSelected = new BoolVariable(false, this.OnSelected, this.OnUnSelected);
        this.text = label;
        parent.Add(this);
        this.RegisterCallback<MouseUpEvent>(this.OnClicked);
    }

    private void OnClicked(MouseUpEvent evt)
    {
        this.OnSelectionActionListener.Invoke();
        evt.StopPropagation();
    }

    private void OnSelected()
    {
    }

    private void OnUnSelected()
    {
    }
}

class InterativeObjectInitializerLine : VisualElement
{
    public BoolVariable IsSelected { get; private set; }
    public BoolVariable IsDisplayed { get; private set; }
    private Color InitialBackGroundColor;
    private Color SelectedBackgroundColor;

    private Action<InterativeObjectInitializerLine> OnClickedExtern;
    private GameObject GameObjectReference;

    public ObjectFieldSelectionIcon GizmoIcon { get; private set; }
    private Label Label;

    public bool IsGizmoSelected()
    {
        return this.GizmoIcon.Selected.GetValue();
    }

    public void SelectGizmo()
    {
        this.GizmoIcon.Selected.SetValue(true);
    }

    public InterativeObjectInitializerLine(VisualElement parent, GameObject GameObject, Action<InterativeObjectInitializerLine> OnClickedExtern)
    {
        this.GameObjectReference = GameObject;
        this.OnClickedExtern = OnClickedExtern;
        this.IsSelected = new BoolVariable(false, this.OnSelected, this.OnUnSelected);
        this.IsDisplayed = new BoolVariable(true, this.OnDisplayed, this.OnHidded);

        this.style.flexDirection = FlexDirection.Row;
        this.GizmoIcon = new ObjectFieldSelectionIcon(this, "G");
        this.Label = new Label(GameObject.name);
        this.Add(this.Label);
        parent.Add(this);

        this.RegisterCallback<MouseEnterEvent>(this.OnMouseEnter);
        this.RegisterCallback<MouseOutEvent>(this.OnMouseExit);
        this.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent MouseDownEvent)
    {
        if (MouseDownEvent.button == (int) MouseButton.LeftMouse)
        {
            Selection.activeGameObject = this.GameObjectReference;
            this.OnClickedExtern.Invoke(this);
        }
        else if (MouseDownEvent.button == (int) MouseButton.RightMouse)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Only This"), false, () => { });
        }

        MouseDownEvent.StopPropagation();
    }

    private void OnMouseEnter(MouseEnterEvent MouseEnterEvent)
    {
        if (!this.IsSelected.GetValue())
        {
            this.style.backgroundColor = Color.gray;
        }
    }

    private void OnMouseExit(MouseOutEvent MouseOutEvent)
    {
        if (!this.IsSelected.GetValue())
        {
            this.style.backgroundColor = this.InitialBackGroundColor;
        }
    }

    private void OnSelected()
    {
        this.style.backgroundColor = Color.cyan;
    }

    private void OnUnSelected()
    {
        this.style.backgroundColor = this.InitialBackGroundColor;
    }

    private void OnDisplayed()
    {
        this.style.display = DisplayStyle.Flex;
    }

    private void OnHidded()
    {
        this.style.display = DisplayStyle.None;
    }
}