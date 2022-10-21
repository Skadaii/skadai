using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

public class CustomDropdown : PropertyAttribute
{
    public Type MyType;
    public string PropertyName;

    public CustomDropdown(Type myType, string propertyName)
    {
        MyType = myType;
        PropertyName = propertyName;
    }
}

[CustomPropertyDrawer(typeof(CustomDropdown))]
public class CustomDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CustomDropdown customDropdown = attribute as CustomDropdown;
        List<string> stringList = null;

        FieldInfo dropdownFieldInfo = customDropdown.MyType.GetField(customDropdown.PropertyName);
        if (dropdownFieldInfo != null)
        {
            stringList = dropdownFieldInfo.GetValue(customDropdown.MyType) as List<string>;
        }

        if (stringList != null && stringList.Count != 0)
        {
            int selectedIndex = Mathf.Max(stringList.IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList.ToArray()); 
            property.stringValue = stringList[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
