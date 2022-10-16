using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;

//[CustomEditor(typeof(UAI_Methods))]
public class UAI_Methods_Editor : Editor
{
    /*private SerializedProperty ComponentName;
    private SerializedProperty MethodName;

    private bool ComponentChanged = false;

    public void OnEnable()
    {
        UAI_Methods uai_methods = target as UAI_Methods;
    
        if (uai_methods == null)
            return;

        ComponentName = serializedObject.FindProperty("ComponentName");
        MethodName = serializedObject.FindProperty("MethodName");

        Component[] Components = uai_methods.GetComponents(typeof(Component));

        uai_methods.ComponentsName.Clear();

        if (Components.Length != 0)
        {
            foreach (Component component in Components)
                uai_methods.ComponentsName.Add(component.GetType().Name);
        }
    }

    public override void OnInspectorGUI()
    {
        UpdateMethodsList();

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(ComponentName);
        if (EditorGUI.EndChangeCheck())
            ComponentChanged = true;

        EditorGUILayout.PropertyField(MethodName);
        serializedObject.ApplyModifiedProperties();
    }

    public void UpdateMethodsList()
    {
        if (!ComponentChanged)
            return;

        ComponentChanged = false;

        UAI_Methods uai_methods = target as UAI_Methods;

        if (uai_methods == null)
            return;

        uai_methods.MethodsName.Clear();

        if (uai_methods.ComponentsName.Count == 0)
            return;

        Component selectedComponent = uai_methods.GetComponent(uai_methods.ComponentName);

        if (selectedComponent == null)
            return;

        MethodInfo[] methodsInfo = selectedComponent.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (MethodInfo method in methodsInfo)
            uai_methods.MethodsName.Add(method.Name);

        Debug.Log("Updated list !");
    }*/
}
