using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ComponentDropdown : AdvancedDropdown
{
    private readonly List<Type> componentTypes;
    private readonly Action<Type> onSelected;

    public ComponentDropdown(AdvancedDropdownState state, Action<Type> onSelected) : base(state)
    {
        this.onSelected = onSelected;
        componentTypes = TypeCache.GetTypesDerivedFrom<Component>().Where(t => !t.IsAbstract).ToList();
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Select Component");

        foreach (var type in componentTypes)
        {
            root.AddChild(new AdvancedDropdownItem(type.Name) { id = componentTypes.IndexOf(type) });
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        Type selectedType = componentTypes[item.id];
        onSelected?.Invoke(selectedType);
    }
}
