using System;
using System.Collections.Generic;
using System.Linq;

public class EnumPicker : ButtonTreePickerGUI
{
    private List<Type> availableEnums;
    private Type enumType;

    public Type EnumType { get => enumType; }
    public List<Type> AvailableEnums { get => availableEnums;}

    private Action OnEnumSelectedEvent;

    public EnumPicker(string namespaceFilter, Action OnEnumSelectedEvent)
    {
        this.OnEnumSelectedEvent = OnEnumSelectedEvent;
        this.availableEnums = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(l => l.GetTypes().ToList())
               .Select(t => t).Where(t => t.IsEnum && t.Namespace == namespaceFilter)
               .ToList();
        base.BaseInit(this.availableEnums.ConvertAll(t => t.Name), this.OnEnumSelected);
    }

    private void OnEnumSelected()
    {
        this.enumType = this.availableEnums[this.availableChoices.IndexOf(this.selectionPicker.SelectedKey)];
        if (this.OnEnumSelectedEvent != null)
        {
            this.OnEnumSelectedEvent.Invoke();
        }
    }
}
