using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(UtilityAI))]
public class UtilityAI_Editor : Editor
{
    private VisualElement root;

    private UtilityAI Target => target as UtilityAI;

    SerializedProperty ActionSetProperty;
    PropertyField ActionSetPropertyField;

    class SerializedShow
    {
        public SerializedProperty ShowProperty;
        public Foldout ShowFoldout;
    }

    class SerializedAction : SerializedShow
    {
        public Button RemoveButton;
        public SerializedProperty ActionNameProperty;
        public TextField ActionNameTextField;

        public SerializedMethods SerializedMethods;
        public SerializedConsideration SerializedConsideration;
    }

    class SerializedMethods : SerializedShow
    {
        public SerializedProperty MethodListProperty;

        public Button AddMethodButton;
        public List<SerializedMethod> SerializedListMethods;
    }

    class SerializedMethod : SerializedShow
    {
        public Button RemoveButton;

        public SerializedProperty ComponentStringProperty;
        public PopupField<string> ComponentPopup;

        public SerializedProperty MethodStringProperty;
        public PopupField<string> MethodPopup;
    }

    class SerializedConsideration : SerializedMethod
    {
        public SerializedProperty AnimationCurveProperty;
        public CurveField CurveField;
    }

    private SerializedShow SerializedActionsShow;
    private Button AddActionButton;
    private List<SerializedAction> SerializedActions = new List<SerializedAction>();

    private SerializedProperty BlackboardProperty;
    private PropertyField BlackboardPropertyField;

    //Action Set Creation
    private Button CreateActionSetButton;
    private SerializedProperty ActionSetNameProperty;
    private TextField ActionSetNameTextField;

    List<string> ComponentsName = new List<string>();

    Dictionary<string, string[]> MethodsName = new Dictionary<string, string[]>();

    public override VisualElement CreateInspectorGUI()
    {
        ComputeComponentsAndMethods();

        FindProperties();
        InitializeEditor();
        Compose();

        return root;
    }

    private void FindProperties()
    {
        ActionSetProperty = serializedObject.FindProperty(nameof(UtilityAI.ActionSet));

        if (Target.ActionSet != null)
            FindActionSetProperties();

        ActionSetNameProperty = serializedObject.FindProperty(nameof(UtilityAI.ActionSetName));
    }

    private void FindActionSetProperties()
    {
        SerializedActions.Clear();

        if (Target.ActionSet == null)
            return;

        SerializedObject ActionSetSerializedObject = new SerializedObject(ActionSetProperty.objectReferenceValue);

        BlackboardProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Blackboard));

        SerializedActionsShow = new SerializedShow();
        SerializedActionsShow.ShowProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Show));

        SerializedProperty actionListProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.actions));

        for (int index = 0; index < Target.ActionSet.actions.Count; index++)
        {
            SerializedAction serializedAction = new SerializedAction();
            SerializedProperty actionProperty = actionListProperty.GetArrayElementAtIndex(index);

            serializedAction.ShowProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.Show));
            serializedAction.ActionNameProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.actionName));

            serializedAction.SerializedMethods = new SerializedMethods();
            serializedAction.SerializedMethods.ShowProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.ShowMethods));
            serializedAction.SerializedMethods.SerializedListMethods = new List<SerializedMethod>();

            SerializedProperty methodListProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.methods));
            serializedAction.SerializedMethods.MethodListProperty = methodListProperty;

            for (int methodIndex = 0; methodIndex < Target.ActionSet.actions[index].methods.Count; methodIndex++)
            {
                SerializedProperty methodProperty = methodListProperty.GetArrayElementAtIndex(methodIndex);

                SerializedMethod serializedMethod = new SerializedMethod();
                serializedMethod.ShowProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.Show));
                serializedMethod.MethodStringProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.MethodName));
                serializedMethod.ComponentStringProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.ComponentName));

                serializedAction.SerializedMethods.SerializedListMethods.Add(serializedMethod);
            }

            SerializedProperty considerationProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.consideration));

            serializedAction.SerializedConsideration = new SerializedConsideration();

            serializedAction.SerializedConsideration.ShowProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.Show));
            serializedAction.SerializedConsideration.MethodStringProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.MethodName));
            serializedAction.SerializedConsideration.ComponentStringProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.ComponentName));
            serializedAction.SerializedConsideration.AnimationCurveProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.AnimationCurve));

            SerializedActions.Add(serializedAction);
        }

    }

    private void InitializeEditor()
    {
        root = new VisualElement();

        #region Create Action Set
        CreateActionSetButton = new Button(CreateActionSet);
        CreateActionSetButton.text = "Create Action Set";
        CreateActionSetButton.style.width = new Length(69, LengthUnit.Percent);

        ActionSetNameTextField = new TextField();
        ActionSetNameTextField.BindProperty(ActionSetNameProperty);
        ActionSetNameTextField.style.width = new Length(30, LengthUnit.Percent);
        #endregion

        ActionSetPropertyField = new PropertyField(ActionSetProperty);
        ActionSetPropertyField.TrackPropertyValue(ActionSetProperty, ActionSetPropertyChanged);

        if (Target.ActionSet != null)
            InitalizeActionSetEditor();
    }

    private void InitalizeActionSetEditor()
    {
        if (Target.ActionSet == null)
            return;

        BlackboardPropertyField = new PropertyField();
        BlackboardPropertyField.BindProperty(BlackboardProperty);

        SerializedActionsShow.ShowFoldout = EditorUtils.CreateFoldout("Actions", 15, Color.white, FlexDirection.Column);
        SerializedActionsShow.ShowFoldout.BindProperty(SerializedActionsShow.ShowProperty);

        AddActionButton =
                EditorUtils.CreateFoldoutButton("Add",
                delegate { AddAction(); },
                new Length(25, LengthUnit.Percent),
                Position.Absolute,
                Align.FlexEnd,
                1);

        foreach (SerializedAction serializedAction in SerializedActions)
        {
            serializedAction.ShowFoldout = EditorUtils.CreateFoldout(serializedAction.ActionNameProperty.stringValue, 15, Color.white, FlexDirection.Column);
            serializedAction.ShowFoldout.BindProperty(serializedAction.ShowProperty);

            serializedAction.ActionNameTextField = new TextField("Action name");
            serializedAction.ActionNameTextField.BindProperty(serializedAction.ActionNameProperty);
            serializedAction.ActionNameTextField.style.width = new Length(99, LengthUnit.Percent);
            serializedAction.ActionNameTextField.TrackPropertyValue(serializedAction.ActionNameProperty, x =>
            {
                serializedAction.ShowFoldout.text = serializedAction.ActionNameProperty.stringValue;
            });

            serializedAction.RemoveButton = 
                EditorUtils.CreateFoldoutButton("Remove", 
                delegate { RemoveAction(serializedAction); }, 
                new Length(25, LengthUnit.Percent), 
                Position.Absolute, 
                Align.FlexEnd, 
                1);

            #region Methods

            SerializedMethods serializedMethods = serializedAction.SerializedMethods;

            serializedMethods.AddMethodButton =
            EditorUtils.CreateFoldoutButton("Add",
            delegate { AddMethodToAction(serializedMethods); },
            new Length(25, LengthUnit.Percent),
            Position.Absolute,
            Align.FlexEnd,
            1);

            serializedMethods.ShowFoldout = EditorUtils.CreateFoldout("Methods", 15, Color.white, FlexDirection.Column);
            serializedMethods.ShowFoldout.BindProperty(serializedMethods.ShowProperty);

            foreach (SerializedMethod serializedMethod in serializedMethods.SerializedListMethods)
            {
                InitalizeMethodFoldoutEditor(serializedMethod);
                
                serializedMethod.RemoveButton =
                EditorUtils.CreateFoldoutButton("Remove",
                delegate { RemoveMethodFromAction(serializedMethods, serializedMethod); },
                new Length(25, LengthUnit.Percent),
                Position.Absolute,
                Align.FlexEnd,
                1);
            }

            #endregion

            #region Consideration

            SerializedConsideration serializedConsideration = serializedAction.SerializedConsideration;

            InitalizeMethodFoldoutEditor(serializedConsideration);

            serializedConsideration.ShowFoldout = EditorUtils.CreateFoldout("Consideration", 15, Color.white, FlexDirection.Column);
            serializedConsideration.ShowFoldout.BindProperty(serializedConsideration.ShowProperty);

            serializedConsideration.CurveField = new CurveField("Consideration Curve");
            serializedConsideration.CurveField.BindProperty(serializedConsideration.AnimationCurveProperty);

            #endregion
        }
    }

    private void InitalizeMethodFoldoutEditor(SerializedMethod serializedMethod)
    {
        serializedMethod.ShowFoldout = EditorUtils.CreateFoldout("Method", 15, Color.white, FlexDirection.Column);
        serializedMethod.ShowFoldout.BindProperty(serializedMethod.ShowProperty);

        serializedMethod.ComponentPopup = new PopupField<string>("Component", ComponentsName, 0);
        serializedMethod.ComponentPopup.BindProperty(serializedMethod.ComponentStringProperty);
        serializedMethod.ComponentPopup.TrackPropertyValue(serializedMethod.ComponentStringProperty, delegate { MethodComponentChanged(serializedMethod); });


        if (!ComponentsName.Contains(serializedMethod.ComponentStringProperty.stringValue))
        {
            serializedMethod.ComponentStringProperty.stringValue = ComponentsName[0];
            serializedMethod.ComponentStringProperty.serializedObject.ApplyModifiedProperties();
        }

        List<string> methodsListName = MethodsName[serializedMethod.ComponentStringProperty.stringValue].ToList();

        serializedMethod.MethodPopup = new PopupField<string>("Method", methodsListName, 0);
        serializedMethod.MethodPopup.BindProperty(serializedMethod.MethodStringProperty);

        if (!methodsListName.Contains(serializedMethod.MethodStringProperty.stringValue))
        {
            if (methodsListName.Count > 0)
                serializedMethod.MethodStringProperty.stringValue = methodsListName[0];
            else
                serializedMethod.MethodStringProperty.stringValue = string.Empty;

            serializedMethod.MethodStringProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    private void Compose()
    {
        root.Clear();

        root.Add(ActionSetPropertyField);

        #region Create Action Set
        if (Target.ActionSet == null)
        {
            VisualElement label = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0,0,0,0), FlexDirection.Row);

            label.Add(ActionSetNameTextField);
            label.Add(CreateActionSetButton);

            root.Add(label);

            return;
        }
        #endregion

        root.Add(BlackboardPropertyField);

        VisualElement actionsLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0, 0, 0, 0), FlexDirection.Column);

        root.Add(actionsLabel);
        actionsLabel.Add(SerializedActionsShow.ShowFoldout);
        actionsLabel.Add(AddActionButton);

        foreach (SerializedAction serializedAction in SerializedActions)
        {
            VisualElement actionLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.3f,0.3f,0.3f), FlexDirection.Column);
            actionLabel.style.marginBottom = 5;

            SerializedActionsShow.ShowFoldout.Add(actionLabel);

            actionLabel.Add(serializedAction.ShowFoldout);
            actionLabel.Add(serializedAction.RemoveButton);
            actionLabel.Add(EditorUtils.CreateSpace(new Vector2(0, 5)));

            serializedAction.ShowFoldout.Add(serializedAction.ActionNameTextField);

            #region Methods

            VisualElement methodslabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.2f, 0.2f, 0.2f), FlexDirection.Column);
            methodslabel.style.marginBottom = 2;

            serializedAction.ShowFoldout.Add(methodslabel);
            
            methodslabel.Add(serializedAction.SerializedMethods.ShowFoldout);
            methodslabel.Add(serializedAction.SerializedMethods.AddMethodButton);
            methodslabel.Add(EditorUtils.CreateSpace(new Vector3(0, 5)));

            foreach (SerializedMethod serializedMethod in serializedAction.SerializedMethods.SerializedListMethods)
            {
                VisualElement methodLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.2f, 0.2f, 0.2f), FlexDirection.Column);
                methodLabel.style.marginBottom = 5;

                serializedAction.SerializedMethods.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector3(0, 5)));
                serializedAction.SerializedMethods.ShowFoldout.Add(methodLabel);

                methodLabel.Add(serializedMethod.ShowFoldout);
                methodLabel.Add(serializedMethod.RemoveButton);

                serializedMethod.ShowFoldout.Add(serializedMethod.ComponentPopup);
                serializedMethod.ShowFoldout.Add(serializedMethod.MethodPopup);
            }

            #endregion Method

            #region Consideration

            VisualElement considerationLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.2f, 0.2f, 0.2f), FlexDirection.Column);

            serializedAction.ShowFoldout.Add(considerationLabel);

            considerationLabel.Add(serializedAction.SerializedConsideration.ShowFoldout);

            serializedAction.SerializedConsideration.ShowFoldout.Add(serializedAction.SerializedConsideration.ComponentPopup);
            serializedAction.SerializedConsideration.ShowFoldout.Add(serializedAction.SerializedConsideration.MethodPopup);
            serializedAction.SerializedConsideration.ShowFoldout.Add(serializedAction.SerializedConsideration.CurveField);

            #endregion

        }
    }

    private void ActionSetPropertyChanged(SerializedProperty property)
    {
        FindProperties();
        InitalizeActionSetEditor();
        Compose();
    }

    private void RemoveAction(SerializedAction serializedAction)
    {
        int index = SerializedActions.IndexOf(serializedAction);
        Target.ActionSet.actions.RemoveAt(index);

        FindActionSetProperties();
        InitalizeActionSetEditor();
        Compose();
    }

    private void AddAction()
    {
        Target.ActionSet.actions.Add(new UAI_Action());

        FindActionSetProperties();
        InitalizeActionSetEditor();
        Compose();
    }

    private void AddMethodToAction(SerializedMethods serializedMethods)
    {
        int index = serializedMethods.SerializedListMethods.Count - 1;
        index = index < 0 ? 0 : index;

        serializedMethods.MethodListProperty.InsertArrayElementAtIndex(index);
        serializedMethods.MethodListProperty.serializedObject.ApplyModifiedProperties();

        FindActionSetProperties();
        InitalizeActionSetEditor();
        Compose();
    }

    private void RemoveMethodFromAction(SerializedMethods serializedMethods, SerializedMethod serializedMethod)
    {
        int index = serializedMethods.SerializedListMethods.IndexOf(serializedMethod);
        serializedMethods.MethodListProperty.DeleteArrayElementAtIndex(index);

        serializedMethods.MethodListProperty.serializedObject.ApplyModifiedProperties();

        FindActionSetProperties();
        InitalizeActionSetEditor();
        Compose();
    }

    private void MethodComponentChanged(SerializedMethod serializedMethod)
    {
        List<string> methodsListName = MethodsName[(serializedMethod.ComponentStringProperty.stringValue)].ToList();
        serializedMethod.MethodPopup.choices = methodsListName;

        if (!methodsListName.Contains(serializedMethod.MethodStringProperty.stringValue))
        {
            if (methodsListName.Count > 0)
                serializedMethod.MethodStringProperty.stringValue = methodsListName[0];
            else
                serializedMethod.MethodStringProperty.stringValue = string.Empty;

            serializedMethod.MethodStringProperty.serializedObject.ApplyModifiedProperties();
        }
    }

    private void ComputeComponentsAndMethods()
    {
        Component[] components = Target.GetComponents(typeof(Component));

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

    private void CreateActionSet()
    {
        UAI_ActionSet asset = ScriptableObject.CreateInstance<UAI_ActionSet>();

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "UAI");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects/UAI", "Actions");
        }

        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/UAI/Actions/" + Target.ActionSetName + ".asset");
        AssetDatabase.SaveAssets();

        Target.ActionSet = asset;
    }
}
