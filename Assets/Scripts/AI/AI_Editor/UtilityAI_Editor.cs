using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using static System.Collections.Specialized.BitVector32;

public class CustomUI
{
    public static bool Foldout(string title, bool display, RectOffset offset)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.label).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, - 2f);
        style.margin = offset;

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }
}

[CustomEditor(typeof(UtilityAI))]
public class UtilityAI_Editor : Editor
{
    SerializedProperty ActionsProperty;
    SerializedProperty BlackboardProperty;

    private string ActionSetName = "NewActionSet";
    private bool ShowActionSet = false;

    private UAI_Action ToRemoveAction = null;
    private UAI_Method ToRemoveMethod = null;

    List<string> ComponentsName = new List<string>();

    Dictionary<string, string[]> MethodsName = new Dictionary<string, string[]>();

    private void OnEnable()
    {
        UtilityAI utilityAI = target as UtilityAI;

        if (utilityAI == null)
            return;

        ActionsProperty = serializedObject.FindProperty("Actions");
        SerializedObject ActionsObject = new SerializedObject(ActionsProperty.objectReferenceValue);
        BlackboardProperty = ActionsObject.FindProperty("Blackboard");

        ComputeComponentsAndMethods(utilityAI);
    }

    private void ComputeComponentsAndMethods(UtilityAI utilityAI)
    {
        Component[] components = utilityAI.GetComponents(typeof(Component));

        ComponentsName.Clear();

        if (components.Length != 0)
        {
            foreach (Component component in components)
            {
                string name = component.GetType().Name;
                ComponentsName.Add(name);

                MethodInfo[] methodsInfo = component.GetType().GetMethods(BindingFlags.Public | 
                    BindingFlags.Instance | BindingFlags.DeclaredOnly);

                List<string> methodsName = new List<string>();
                foreach (MethodInfo method in methodsInfo)
                    methodsName.Add(method.Name);

                MethodsName.Add(name, methodsName.ToArray());
            }
        }
    }

    public override void OnInspectorGUI()
    {
        UtilityAI utilityAI = target as UtilityAI;

        if (utilityAI == null)
            return;

        serializedObject.Update();

        EditorGUILayout.PropertyField(ActionsProperty);

        if (utilityAI.Actions != null)
        {
            if (ToRemoveAction != null)
            {
                utilityAI.Actions.actions.Remove(ToRemoveAction);
                ToRemoveAction = null;
            }

            EditorGUILayout.PropertyField(BlackboardProperty);
            DrawActions(utilityAI.Actions);
        }
        else
        {
            ActionSetName = GUILayout.TextField(ActionSetName);
            if (GUILayout.Button("Create UAI Action Set"))
            {
                CreateActions();
            }
        }

        BlackboardProperty.serializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
    }

    public void SaveActions(UAI_ActionSet uai_actions)
    {
        EditorUtility.SetDirty(uai_actions);
        AssetDatabase.SaveAssets();
    }

    private void CreateActions()
    {
        UAI_ActionSet asset = ScriptableObject.CreateInstance<UAI_ActionSet>();

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "UAI");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects/UAI", "Actions");
        }

        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/UAI/Actions/" + ActionSetName + ".asset");
        AssetDatabase.SaveAssets();

        UtilityAI utilityAI = target as UtilityAI;
        utilityAI.Actions = asset;
    }

    public void DrawActions(UAI_ActionSet uai_actions)
    {
        if (uai_actions == null)
            return;

        if (GUILayout.Button("Save"))
            SaveActions(uai_actions);

        if (GUILayout.Button("Create action"))
            CreateAction(uai_actions);

        //showActions = CustomUI.Foldout("Actions:", showActions, new RectOffset());
        //if (showActions)
        //{       
            foreach (UAI_Action action in uai_actions.actions)
                DrawAction(action);
        //}
    }

    private void CreateAction(UAI_ActionSet uai_actions)
    {
        uai_actions.actions.Add(new UAI_Action());
    }

    public void DrawAction(UAI_Action action)
    {
        action.Show = CustomUI.Foldout(action.actionName, action.Show, new RectOffset());

        if (action.Show)
        {
            if (GUILayout.Button("Remove action"))
                ToRemoveAction = action;

            action.actionName = EditorGUILayout.TextField("Action name:", action.actionName);

            if (GUILayout.Button("Add method"))
                CreateMethod(action);

            foreach (UAI_Method method in action.methods)
                DrawMethod(method);

            if (ToRemoveMethod != null)
            {
                action.methods.Remove(ToRemoveMethod);
                ToRemoveMethod = null;
            }

            DrawConsideration(action.consideration);
        }
    }

    private void CreateMethod(UAI_Action uai_action)
    {
        uai_action.methods.Add(new UAI_Method());
    }

    public void DrawMethod(UAI_Method method)
    {
        method.Show = CustomUI.Foldout("Method", method.Show, new RectOffset());

        if (method.Show)
        {
            if (GUILayout.Button("Remove method"))
                ToRemoveMethod = method;

            UtilityAI utilityAI = target as UtilityAI;

            EditorGUI.BeginChangeCheck();

            method.ComponentIndex = EditorGUILayout.Popup("Component: ", method.ComponentIndex, ComponentsName.ToArray());
            method.MethodIndex = EditorGUILayout.Popup("Method: ", method.MethodIndex, MethodsName.ElementAt(method.ComponentIndex).Value);
            
            if (EditorGUI.EndChangeCheck())
            {
                method.UpdateMethodInfo(ComponentsName[method.ComponentIndex],
                    MethodsName.ElementAt(method.ComponentIndex).Value.ElementAt(method.MethodIndex),
                    utilityAI);
            }
        }
    }

    public void DrawConsideration(UAI_Consideration consideration)
    {
        consideration.Show = CustomUI.Foldout("Consideration", consideration.Show, new RectOffset());

        if (consideration.Show)
        {
            UtilityAI utilityAI = target as UtilityAI;

            EditorGUI.BeginChangeCheck();

            consideration.ComponentIndex = EditorGUILayout.Popup("Component: ", consideration.ComponentIndex, ComponentsName.ToArray());
            consideration.MethodIndex = EditorGUILayout.Popup("Consideration Method: ", consideration.MethodIndex, MethodsName.ElementAt(consideration.ComponentIndex).Value);

            if (EditorGUI.EndChangeCheck())
            {
                consideration.UpdateMethodInfo(ComponentsName[consideration.ComponentIndex],
                    MethodsName.ElementAt(consideration.ComponentIndex).Value.ElementAt(consideration.MethodIndex),
                    utilityAI);
            }

            EditorGUILayout.CurveField("Curve", consideration.AnimationCurve);
        }
    }
}
