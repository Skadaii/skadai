using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(UtilityAI))]
public class UtilityAI_Editor : Editor
{
    private VisualElement root;

    private UtilityAI Target => target as UtilityAI;

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
        public TextElement TextElement;
    }

    class SerializedActionSet : SerializedShow
    {
        public SerializedProperty NameProperty;
        public Button RemoveButton;


        public SerializedShow SerializedActionsShow;
        public Button AddActionButton;
        public List<SerializedAction> SerializedActions = new List<SerializedAction>();

        public SerializedProperty BlackboardProperty;
        public PropertyField BlackboardPropertyField;

        public SerializedProperty ActionSetProperty;
        public PropertyField ActionSetPropertyField;

        public SerializedProperty UtilityExecuteInUpdateProperty;
        public PropertyField UtilityExecuteInUpdateField;
    }

    private List<SerializedActionSet> SerializedActionSetList = new List<SerializedActionSet>();
    SerializedProperty ActionSetListProperty;

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
        ActionSetNameProperty = serializedObject.FindProperty(nameof(UtilityAI.ActionSetName));
        ActionSetListProperty = serializedObject.FindProperty(nameof(UtilityAI.ActionSetList));

        FindActionSetListProperties();
    }

    private void FindActionSetListProperties()
    {
        SerializedActionSetList.Clear();

        if (Target.ActionSetList != null && Target.ActionSetList.Count > 0)
        {
            for (int index = 0; index < ActionSetListProperty.arraySize; index++)
            {
                SerializedActionSet serializedActionSet = new SerializedActionSet();
                serializedActionSet.ActionSetProperty = ActionSetListProperty.GetArrayElementAtIndex(index);

                SerializedActionSetList.Add(serializedActionSet);

                FindActionSetProperties(serializedActionSet);
            }
        }
    }

    private void FindActionSetProperties(SerializedActionSet serializedActionSet)
    {
        serializedActionSet.SerializedActions.Clear();

        if (Target.ActionSetList == null)
            return;

        SerializedObject ActionSetSerializedObject = new SerializedObject(serializedActionSet.ActionSetProperty.objectReferenceValue);

        serializedActionSet.NameProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.ActionSetName));
        serializedActionSet.ShowProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Show));

        serializedActionSet.UtilityExecuteInUpdateProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.ExecuteInUpdate));
        serializedActionSet.BlackboardProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Blackboard));

        serializedActionSet.SerializedActionsShow = new SerializedShow();
        serializedActionSet.SerializedActionsShow.ShowProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.ShowActions));

        SerializedProperty actionListProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Actions));

        for (int index = 0; index < actionListProperty.arraySize; index++)
        {
            SerializedAction serializedAction = new SerializedAction();
            SerializedProperty actionProperty = actionListProperty.GetArrayElementAtIndex(index);

            FindActionProperties(serializedAction, actionProperty);

            serializedActionSet.SerializedActions.Add(serializedAction);
        }

    }

    private void FindActionProperties(SerializedAction serializedAction, SerializedProperty actionProperty)
    {
        serializedAction.ShowProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.Show));
        serializedAction.ActionNameProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.actionName));

        serializedAction.SerializedMethods = new SerializedMethods();
        serializedAction.SerializedMethods.ShowProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.ShowMethods));
        serializedAction.SerializedMethods.SerializedListMethods = new List<SerializedMethod>();

        SerializedProperty methodListProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.methods));
        serializedAction.SerializedMethods.MethodListProperty = methodListProperty;

        for (int methodIndex = 0; methodIndex < methodListProperty.arraySize; methodIndex++)
        {
            SerializedProperty methodProperty = methodListProperty.GetArrayElementAtIndex(methodIndex);
            SerializedMethod serializedMethod = new SerializedMethod();

            FindMethodProperties(serializedMethod, methodProperty);

            serializedAction.SerializedMethods.SerializedListMethods.Add(serializedMethod);
        }

        SerializedProperty considerationProperty = actionProperty.FindPropertyRelative(nameof(UAI_Action.consideration));

        serializedAction.SerializedConsideration = new SerializedConsideration();

        serializedAction.SerializedConsideration.ShowProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.Show));
        serializedAction.SerializedConsideration.MethodStringProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.MethodName));
        serializedAction.SerializedConsideration.ComponentStringProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.ComponentName));
        serializedAction.SerializedConsideration.AnimationCurveProperty = considerationProperty.FindPropertyRelative(nameof(UAI_Consideration.AnimationCurve));
    }

    private void FindMethodProperties(SerializedMethod serializedMethod, SerializedProperty methodProperty)
    {
        serializedMethod.ShowProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.Show));
        serializedMethod.MethodStringProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.MethodName));
        serializedMethod.ComponentStringProperty = methodProperty.FindPropertyRelative(nameof(UAI_Method.ComponentName));
    }

        private void InitializeEditor()
    {
        root = new VisualElement();

        #region Create Action Set
        CreateActionSetButton = new Button(AddOrCreateActionSet);
        CreateActionSetButton.text = "Add Or Create Action Set";
        CreateActionSetButton.style.width = new Length(69, LengthUnit.Percent);

        ActionSetNameTextField = new TextField();
        ActionSetNameTextField.BindProperty(ActionSetNameProperty);
        ActionSetNameTextField.style.width = new Length(30, LengthUnit.Percent);
        #endregion

        InitalizeActionSetListEditor();
    }

    private void InitalizeActionSetListEditor()
    {
        if (Target.ActionSetList != null && Target.ActionSetList.Count > 0)
        {
            for (int index = 0; index < ActionSetListProperty.arraySize; index++)
            {
                InitalizeActionSetEditor(SerializedActionSetList[index]);
            }
        }
    }

    private void InitalizeActionSetEditor(SerializedActionSet serializedActionSet)
    {
        string name = serializedActionSet.NameProperty.stringValue;

        serializedActionSet.ShowFoldout = EditorUtils.CreateFoldout(name, 15, Color.white, FlexDirection.Column);
        serializedActionSet.ShowFoldout.BindProperty(serializedActionSet.ShowProperty);

        serializedActionSet.RemoveButton = 
            EditorUtils.CreateFoldoutButton("Remove",
                delegate { RemoveActionSet(serializedActionSet); },
                new Length(25, LengthUnit.Percent),
                Position.Absolute,
                Align.FlexEnd,
                1);

        serializedActionSet.ActionSetPropertyField = new PropertyField();
        serializedActionSet.ActionSetPropertyField.BindProperty(serializedActionSet.ActionSetProperty);
        serializedActionSet.ActionSetPropertyField.SetEnabled(false);
        serializedActionSet.ActionSetPropertyField.label = "Action Set";

        serializedActionSet.BlackboardPropertyField = new PropertyField();
        serializedActionSet.BlackboardPropertyField.BindProperty(serializedActionSet.BlackboardProperty);

        serializedActionSet.UtilityExecuteInUpdateField = new PropertyField();
        serializedActionSet.UtilityExecuteInUpdateField.BindProperty(serializedActionSet.UtilityExecuteInUpdateProperty);

        serializedActionSet.SerializedActionsShow.ShowFoldout = EditorUtils.CreateFoldout("Actions", 15, Color.white, FlexDirection.Column);
        serializedActionSet.SerializedActionsShow.ShowFoldout.BindProperty(serializedActionSet.SerializedActionsShow.ShowProperty);

        serializedActionSet.AddActionButton =
                EditorUtils.CreateFoldoutButton("Add",
                delegate { AddAction(serializedActionSet); },
                new Length(25, LengthUnit.Percent),
                Position.Absolute,
                Align.FlexEnd,
                1);

        foreach (SerializedAction serializedAction in serializedActionSet.SerializedActions)
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
                delegate { RemoveAction(serializedActionSet, serializedAction); }, 
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

            serializedConsideration.CurveField = new CurveField();
            serializedConsideration.CurveField.BindProperty(serializedConsideration.AnimationCurveProperty);
            serializedConsideration.CurveField.style.height = 50;

            serializedConsideration.TextElement = new TextElement();
            serializedConsideration.TextElement.text = "Consideration Curve";
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

        #region Create Action Set
        VisualElement label = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0, 0, 0, 0), FlexDirection.Row);

        label.Add(ActionSetNameTextField);
        label.Add(CreateActionSetButton);

        root.Add(label);
        #endregion

        VisualElement actionSetListLabel = EditorUtils.CreateLabel(1, 5, Color.blue, new Color(0, 0, 0, 0), FlexDirection.Column);
        VisualElement actionSetListLabelText = EditorUtils.CreateLabel(0, 0, Color.white, new Color(0, 0, 0, 0), FlexDirection.Column, "Actions Sets");

        actionSetListLabelText.style.unityTextAlign = TextAnchor.UpperCenter;
        actionSetListLabelText.style.color = Color.white;

        root.Add(EditorUtils.CreateSpace(new Vector2(0, 20)));
        root.Add(actionSetListLabel);

        actionSetListLabel.Add(actionSetListLabelText);
        actionSetListLabel.Add(EditorUtils.CreateSpace(new Vector2(0, 20)));

        foreach (SerializedActionSet serializedActionSet in SerializedActionSetList)
        {
            VisualElement actionSetLabel = EditorUtils.CreateLabel(1, 5, Color.grey, new Color(0, 0, 0, 0), FlexDirection.Column);
            actionSetLabel.style.alignSelf = Align.Stretch;
            actionSetLabel.style.marginTop = 2.5f;
            actionSetLabel.style.marginBottom = 2.5f;

            actionSetListLabel.Add(actionSetLabel);
            actionSetLabel.Add(serializedActionSet.ShowFoldout);
            actionSetLabel.Add(serializedActionSet.RemoveButton);

            ComposeActionSet(serializedActionSet);

            serializedActionSet.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector2(0, 5)));
        }

    }

    private void ComposeActionSet(SerializedActionSet serializedActionSet)
    {
        serializedActionSet.ShowFoldout.Add(serializedActionSet.ActionSetPropertyField);
        serializedActionSet.ShowFoldout.Add(serializedActionSet.BlackboardPropertyField);
        serializedActionSet.ShowFoldout.Add(serializedActionSet.UtilityExecuteInUpdateField);

        VisualElement actionsLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0, 0, 0, 0), FlexDirection.Column);

        serializedActionSet.ShowFoldout.Add(actionsLabel);
        actionsLabel.Add(serializedActionSet.SerializedActionsShow.ShowFoldout);
        actionsLabel.Add(serializedActionSet.AddActionButton);

        serializedActionSet.SerializedActionsShow.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector2(0, 5)));

        foreach (SerializedAction serializedAction in serializedActionSet.SerializedActions)
        {
            VisualElement actionLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.3f,0.3f,0.3f), FlexDirection.Column);
            actionLabel.style.marginBottom = 5;

            serializedActionSet.SerializedActionsShow.ShowFoldout.Add(actionLabel);

            actionLabel.Add(serializedAction.ShowFoldout);
            actionLabel.Add(serializedAction.RemoveButton);

            serializedAction.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector2(0, 5)));
            serializedAction.ShowFoldout.Add(serializedAction.ActionNameTextField);

            #region Methods

            VisualElement methodslabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.2f, 0.2f, 0.2f), FlexDirection.Column);
            methodslabel.style.marginBottom = 2;

            serializedAction.ShowFoldout.Add(methodslabel);
            
            methodslabel.Add(serializedAction.SerializedMethods.ShowFoldout);
            methodslabel.Add(serializedAction.SerializedMethods.AddMethodButton);

            serializedAction.SerializedMethods.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector3(0, 5)));

            foreach (SerializedMethod serializedMethod in serializedAction.SerializedMethods.SerializedListMethods)
            {
                VisualElement methodLabel = EditorUtils.CreateLabel(1, 5, Color.gray, new Color(0.2f, 0.2f, 0.2f), FlexDirection.Column);
                methodLabel.style.marginBottom = 5;

                serializedAction.SerializedMethods.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector3(0, 5)));
                serializedAction.SerializedMethods.ShowFoldout.Add(methodLabel);

                methodLabel.Add(serializedMethod.ShowFoldout);
                methodLabel.Add(serializedMethod.RemoveButton);

                serializedMethod.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector3(0, 5)));
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
            serializedAction.SerializedConsideration.ShowFoldout.Add(EditorUtils.CreateSpace(new Vector2(0, 5)));
            serializedAction.SerializedConsideration.ShowFoldout.Add(serializedAction.SerializedConsideration.TextElement);
            serializedAction.SerializedConsideration.ShowFoldout.Add(serializedAction.SerializedConsideration.CurveField);

            #endregion

        }
    }

    private void RemoveAction(SerializedActionSet serializedActionSet, SerializedAction serializedAction)
    {
        int actionSetIndex = SerializedActionSetList.IndexOf(serializedActionSet);
        int actionIndex = serializedActionSet.SerializedActions.IndexOf(serializedAction);

        Target.ActionSetList[actionSetIndex].Actions.RemoveAt(actionIndex);
        SerializedActionSetList[actionSetIndex].SerializedActions.RemoveAt(actionIndex);

        InitalizeActionSetListEditor();
        Compose();
    }

    private void AddAction(SerializedActionSet serializedActionSet)
    {
        int index = SerializedActionSetList.IndexOf(serializedActionSet);
        Target.ActionSetList[index].Actions.Add(new UAI_Action());

        serializedActionSet.ActionSetProperty.serializedObject.Update();

        SerializedAction serializedAction = new SerializedAction();

        SerializedObject ActionSetSerializedObject = new SerializedObject(serializedActionSet.ActionSetProperty.objectReferenceValue);
        SerializedProperty actionListProperty = ActionSetSerializedObject.FindProperty(nameof(UAI_ActionSet.Actions));
        SerializedProperty actionProperty = actionListProperty.GetArrayElementAtIndex(Target.ActionSetList[index].Actions.Count - 1);

        FindActionProperties(serializedAction, actionProperty);
        
        serializedActionSet.SerializedActions.Add(serializedAction);

        InitalizeActionSetListEditor();
        Compose();
    }

    private void AddMethodToAction(SerializedMethods serializedMethods)
    {
        int index = serializedMethods.SerializedListMethods.Count - 1;
        index = index < 0 ? 0 : index;

        serializedMethods.MethodListProperty.InsertArrayElementAtIndex(index);
        serializedMethods.MethodListProperty.serializedObject.ApplyModifiedProperties();

        SerializedProperty methodProperty = serializedMethods.MethodListProperty.GetArrayElementAtIndex(index);
        SerializedMethod serializedMethod = new SerializedMethod();

        FindMethodProperties(serializedMethod, methodProperty);

        serializedMethods.SerializedListMethods.Add(serializedMethod);

        InitalizeActionSetListEditor();
        Compose();
    }

    private void RemoveMethodFromAction(SerializedMethods serializedMethods, SerializedMethod serializedMethod)
    {
        int index = serializedMethods.SerializedListMethods.IndexOf(serializedMethod);
        serializedMethods.MethodListProperty.DeleteArrayElementAtIndex(index);

        serializedMethods.MethodListProperty.serializedObject.ApplyModifiedProperties();

        serializedMethods.SerializedListMethods.RemoveAt(index);

        InitalizeActionSetListEditor();
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
        MethodsName.Clear();

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

    private void AddOrCreateActionSet()
    {
        string assetPath = "Assets/ScriptableObjects/UAI/Actions/" + Target.ActionSetName + ".asset";

        UAI_ActionSet asset;

        asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UAI_ActionSet)) as UAI_ActionSet;

        if (!asset)
        {
            asset = ScriptableObject.CreateInstance<UAI_ActionSet>();

            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "UAI");
                AssetDatabase.CreateFolder("Assets/ScriptableObjects/UAI", "Actions");
            }

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
        }

        asset.ActionSetName = Target.ActionSetName;
        Target.ActionSetList.Add(asset);

        ActionSetListProperty.serializedObject.Update();

        FindActionSetListProperties();
        InitalizeActionSetListEditor();
        Compose();
    }

    private void RemoveActionSet(SerializedActionSet serializedActionSet)
    {
        int index = SerializedActionSetList.IndexOf(serializedActionSet);
        ActionSetListProperty.DeleteArrayElementAtIndex(index);

        ActionSetListProperty.serializedObject.ApplyModifiedProperties();

        FindActionSetListProperties();
        InitalizeActionSetListEditor();
        Compose();
    }

    private void ActionSetListPropertyChanged(SerializedProperty serializedProperty)
    {
        if (Target.ActionSetList.Count != SerializedActionSetList.Count)
        {
            FindActionSetListProperties();
            InitalizeActionSetListEditor();
            Compose();
        }
    }
}
