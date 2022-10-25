using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

public class EditorUtils
{
    public static VisualElement CreateLabel(float borderSize, float borderRadius, Color borderColor, Color backgroundColor, FlexDirection flexDirection)
    {
        Label label = new Label();

        label.style.borderBottomWidth = borderSize;
        label.style.borderTopWidth = borderSize;
        label.style.borderLeftWidth = borderSize;
        label.style.borderRightWidth = borderSize;

        label.style.borderBottomLeftRadius = borderRadius;
        label.style.borderBottomRightRadius = borderRadius;
        label.style.borderTopLeftRadius = borderRadius;
        label.style.borderTopRightRadius = borderRadius;

        label.style.borderRightColor = borderColor;
        label.style.borderBottomColor = borderColor;
        label.style.borderTopColor = borderColor;
        label.style.borderLeftColor = borderColor;
        label.style.borderRightColor = borderColor;

        label.style.flexDirection = flexDirection;

        label.style.backgroundColor = backgroundColor;

        return label;
    }

    public static Foldout CreateFoldout(string text, float marginLeft, Color textColor, FlexDirection flexDirection)
    {
        Foldout foldout = new Foldout();
        foldout.style.marginLeft = marginLeft;
        foldout.style.color = textColor;

        foldout.text = text;
        foldout.style.flexDirection = flexDirection;


        return foldout;
    }

    public static Button CreateFoldoutButton(string text, Action action, StyleLength width, Position position, Align alignSelf, StyleLength marginRight)
    {
        Button button = new Button(action);
        button.text = text;
        button.style.width = width;
        button.style.position = position;
        button.style.alignSelf = alignSelf;
        button.style.right = marginRight;

        return button;
    }

    public static VisualElement CreateSpace(Vector2 spaceSize)
    {
        Label label = new Label();

        label.style.width = spaceSize.x;
        label.style.height = spaceSize.y;

        return label;
    }

    /*public static bool Foldout(string title, bool display, RectOffset offset)
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
    }*/
}
