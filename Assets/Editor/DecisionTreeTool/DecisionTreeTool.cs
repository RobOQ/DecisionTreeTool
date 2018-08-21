using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DecisionTreeTool : EditorWindow
{
	List<NodeGraphElement> elements;
	GUIStyle defaultStyle;

	[MenuItem("Window/Decision Tree Tool")]
	static void OpenWindow()
	{
		DecisionTreeTool dtt = GetWindow<DecisionTreeTool>();
		dtt.titleContent = new UnityEngine.GUIContent("Decision Tree Tool");
	}

	private void OnEnable()
	{
		defaultStyle = new GUIStyle();
		defaultStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		defaultStyle.border = new RectOffset(12, 12, 12, 12);
	}

	void OnGUI()
	{
		Draw();
		ProcessEvents(Event.current);

		if (GUI.changed)
		{
			Repaint();
		}
	}

	void Draw()
	{
		if(elements != null)
		{
			foreach(var element in elements)
			{
				element.Draw();
			}
		}
	}

	void ProcessEvents(Event e)
	{
		switch(e.type)
		{
			case EventType.MouseDown:
				if(e.button == 1)
				{
					ProcessContextMenu(e.mousePosition);
				}
				break;
			default:
				break;
		}
	}

	void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu genericMenu = new GenericMenu();
		genericMenu.AddItem(new GUIContent("Add Simple Node"), false, () => OnClickAddNode(mousePosition));
		genericMenu.ShowAsContext();
	}

	void OnClickAddNode(Vector2 mousePosition)
	{
		if(elements == null)
		{
			elements = new List<NodeGraphElement>();
		}

		elements.Add(new SimpleNode(mousePosition, 200, 50, defaultStyle));
	}
}
