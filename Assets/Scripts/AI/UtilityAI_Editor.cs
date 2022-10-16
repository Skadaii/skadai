using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UtilityAI))]
public class UtilityAI_Editor : Editor
{
    SerializedProperty ActionsProperty;

    public string actionsName = "NewActions";

    private void OnEnable()
    {
        ActionsProperty = serializedObject.FindProperty("Actions");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

       serializedObject.Update();

        UAI_Actions uai_actions = ActionsProperty.serializedObject.context as UAI_Actions;

        EditorGUILayout.PropertyField(ActionsProperty);

        if (uai_actions != null)
        {
            foreach (UAI_Action action in uai_actions.actions)
                DrawAction(action);
        }
        else
        {
            actionsName = GUILayout.TextField(actionsName);
            if (GUILayout.Button("Create UAI Actions"))
            {
                CreateActions();
            }
        }
    }

    private void CreateActions()
    {
        UAI_Actions asset = ScriptableObject.CreateInstance<UAI_Actions>();

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "UAI");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects/UAI", "Actions");
        }

        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/UAI/Actions/" + actionsName + ".asset");
        AssetDatabase.SaveAssets();

        UtilityAI utilityAI = target as UtilityAI;
        utilityAI.Actions = asset;
    }

    public void DrawActions(UAI_Actions uai_actions)
    {
        if (uai_actions == null)
            return;

        if (GUILayout.Button("Your ButtonText"))
            CreateAction();

        foreach (UAI_Action action in uai_actions.actions)
            DrawAction(action);
    }

    private void CreateAction()
    {
        UAI_Actions asset = ScriptableObject.CreateInstance<UAI_Actions>();

        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/UAI/Action/UAI_Action.asset");
        AssetDatabase.SaveAssets();

        UtilityAI utilityAI = target as UtilityAI;
        utilityAI.Actions = asset;
    }

    public void DrawAction(UAI_Action action)
    {
        if (EditorGUILayout.Foldout(true, "Action"))
        {
            //SerializedObject serializedObject = new SerializedObject(action);
            //SerializedProperty methodsProperty = serializedObject.FindProperty("methods");
            //SerializedProperty considerationProperty = serializedObject.FindProperty("consideration");
            //
            //EditorGUILayout.PropertyField(methodsProperty);
            //EditorGUILayout.PropertyField(considerationProperty);
        }
    }
}
